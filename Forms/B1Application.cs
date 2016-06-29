//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using insightOne.SB1.Framework.BusinessOne;
using insightOne.SB1.Framework.Data.Migrations;
using insightOne.SB1.Framework.Exceptions;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace insightOne.SB1.Framework.Forms
{
    public class B1Application
    {
        private static B1Application _currentApplication = null;
        private readonly SboGuiApi _sboGuiApi;
        private readonly Application _sboApplication;
        private readonly SAPbobsCOM.Company _sboCompany;
        private readonly ServiceLocator _systemForms;
        private readonly ServiceLocator _userForms;
        private readonly Dictionary<string, IB1Form> _forms;
        private readonly IExceptionPolicy _errorHandler;
        private readonly B1AppMenu _menus;
        private readonly List<string> _modalForms;
        private readonly B1BusinessObjects _businessObjects;
        private readonly B1PermissionManager _permissionManager;

        private IB1Form _modalForm = null;

        public event EventHandler AppShutDown;
        public event EventHandler AppCompanyChanged;
        public event EventHandler AppServerTermination;

        /// <summary>
        /// Execute before application register forms. This place is good for create manual menus
        /// </summary>
        public event EventHandler BeforeRegisterForms;

        private bool _autoMigrate = false;
        private bool _initialized = false;

        public Application SBOApplication {
            get { return _sboApplication; }
        }

        public SAPbobsCOM.Company SBOCompany {
            get { return _sboCompany; }
        }

        public B1AppMenu Menus {
            get { return _menus; }
        }

        public B1BusinessObjects BusinessObjects {
            get { return _businessObjects; }
        }

        public B1PermissionManager PermissionManager {
            get { return _permissionManager; }
        }

        public static B1Application Current {
            get { return _currentApplication; }
        }

        public bool Initialized {
            get { return _initialized; }
        }

        public B1Application(string connectionString, bool autoMigrate = false) {
            Check.IsNotNullOrEmpty("connectionString", connectionString);
            _systemForms = new ServiceLocator();
            _userForms = new ServiceLocator();
            _forms = new Dictionary<string,IB1Form>();
            _errorHandler = new DefaultExceptionPolicy();
            _modalForms = new List<string>();
            _sboGuiApi = new SboGuiApi();
            _sboGuiApi.Connect(connectionString);
            _sboApplication = _sboGuiApi.GetApplication();
            _sboCompany = _sboApplication.Company.GetDICompany() as SAPbobsCOM.Company;            
            _sboApplication.AppEvent += new _IApplicationEvents_AppEventEventHandler(OnApplicationEvent);
            _sboApplication.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(OnAppItemEvent);
            _sboApplication.MenuEvent += new _IApplicationEvents_MenuEventEventHandler(OnAppMenuEvent);
            _sboApplication.FormDataEvent += new _IApplicationEvents_FormDataEventEventHandler(OnAppFormDataEvent);
            _sboApplication.RightClickEvent += new _IApplicationEvents_RightClickEventEventHandler(OnAppRightClick);
            _menus = new B1AppMenu(this);
            _businessObjects = new B1BusinessObjects(this);
            _permissionManager = new B1PermissionManager(this);
            _autoMigrate = autoMigrate;
            _currentApplication = this;
        }

        private void LoadFormTypes() {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IB1Form).IsAssignableFrom(t))
                    );
            foreach (Type t in types) {
                var att = (B1FormTypeAttribute)Attribute.GetCustomAttribute(t, typeof(B1FormTypeAttribute));
                if (att != null) {
                    string typeName = att.FormType;
                    if (att.IsSystemForm) {
                        _systemForms.Register(t, typeof(IB1Form), typeName);
                    }
                    else {
                        if (att.IsModal) {
                            _modalForms.Add(att.FormType);
                        }
                        if (att.IsMenuAuto) {
                            _userForms.Register(t, typeof(IB1Form), typeName);
                        }
                        if (att.CreateInMenu && !string.IsNullOrEmpty(att.MenuParent)) {
                            if (PermissionManager.HasPermission(att.FormType)) {
                                _menus.Add(att.MenuParent, att.FormType, att.MenuCaption, att.MenuPosition);
                            }
                        }
                    }
                }
            }
        }

        private void RunAutoMigration() {
            var migrationTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IDataMigration).IsAssignableFrom(t))
                    );
            try {
                foreach (var m in migrationTypes) {
                    var migration = (IDataMigration)Activator.CreateInstance(m);
                    migration.CreateOrUpgrade();
                }
            }
            catch (Exception ex) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Não foi possível atualizar o addon da provider.");
                sb.AppendLine("Recomendamos que remova todos os objetos vinculados ao AddOn e tente novamente.");
                sb.AppendLine("Caso o erro persista, entre em contato com a Provider");
#if DEBUG
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(ex.ToString());
#endif

                SBOApplication.MessageBox(sb.ToString());
                Exit();
            }
        }

        protected void OnAppRightClick(ref ContextMenuInfo eventInfo, out bool BubbleEvent) {
            BubbleEvent = true;
            try {
                if (eventInfo.BeforeAction) {
                    if (_modalForm != null && _modalForm.FormID != eventInfo.FormUID) {
                        BubbleEvent = false;
                        return;
                    }
                }
                
                IB1Form theForm = null;
                if (_forms.TryGetValue(eventInfo.FormUID, out theForm)) {
                    theForm.OnRightClickEvent(ref eventInfo, out BubbleEvent);
                }
            }
            catch (Exception ex) {
                var handled = _errorHandler.HandleException(ex);
                if (!handled) {
                    Exit();
                }
            }
        }

        protected void OnAppFormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent) {
            BubbleEvent = true;
            try {
                IB1Form theForm = null;
                if (_forms.TryGetValue(BusinessObjectInfo.FormUID, out theForm)) {
                    theForm.OnFormDataEvent(ref BusinessObjectInfo, out BubbleEvent);
                }
            }
            catch (Exception ex) {
                var handled = _errorHandler.HandleException(ex);
                if (!handled) {
                    Exit();
                }
            }
        }

        protected void OnAppMenuEvent(ref MenuEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            try {
                Form activeForm = null;
                try {
                    activeForm = _sboApplication.Forms.ActiveForm;
                }
                catch (COMException) {
                    activeForm = null;
                }

                if (pVal.BeforeAction && activeForm != null) {
                    if (_modalForm != null && _modalForm.FormID != activeForm.UniqueID) {
                        var runInModal = true;
                        switch (pVal.MenuUID) {
                            case "1292":
                            case "1293":
                            case "1299":
                            case "1294":
                            case "1281":
                            case "1282":
                            case "1288":
                            case "1289":
                            case "1290":
                            case "1291":
                            case "1283":
                            case "1284":
                            case "1285":
                            case "1286":
                            case "1287":
                                runInModal = false;
                                break;
                        }
                        if (runInModal) {
                            BubbleEvent = false;
                            _modalForm.Select();
                            return;
                        }
                    }
                }
                
                IB1Form theForm = null;
                if (activeForm != null && _forms.TryGetValue(activeForm.UniqueID, out theForm)) {
                    theForm.OnMenuEvent(ref pVal, out BubbleEvent);
                }                
                if (!pVal.BeforeAction) {
                    var userForm = _userForms.TryResolve<IB1Form>(pVal.MenuUID);
                    if (userForm != null) {
                        userForm.Show();
                    }
                }
            }
            catch (Exception ex) {
                var handled = _errorHandler.HandleException(ex);
                if (!handled) {
                    Exit();
                }
            }
        }

        protected void OnAppItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            try {
                if (_modalForm != null && _modalForm.FormID != FormUID) {
                    var runInModal = true;
                    switch (pVal.EventType) {
                        case BoEventTypes.et_FORM_LOAD:
                        case BoEventTypes.et_FORM_DRAW:
                        case BoEventTypes.et_FORM_UNLOAD:
                        case BoEventTypes.et_FORM_VISIBLE:
                        case BoEventTypes.et_FORM_DEACTIVATE:
                        case BoEventTypes.et_CHOOSE_FROM_LIST:
                            runInModal = false;
                            break;
                    }

                    switch (pVal.FormTypeEx) {
                        case "0": //MESSAGE BOX
                        case "10000075": //CALENDARIO
                        case "10000076": //CALCULADORA
                            runInModal = false;
                            break;
                    }

                    if (runInModal) {
                        if (pVal.BeforeAction) {
                            BubbleEvent = false;
                        }
                        else {
                            _modalForm.Select();
                        }
                        return;
                    }
                }                

                if (pVal.EventType == BoEventTypes.et_FORM_LOAD && pVal.BeforeAction) {
                    OnFormLoadBefore(FormUID, pVal);
                }                

                IB1Form theForm = null;
                if (_forms.TryGetValue(FormUID, out theForm)) {
                    if ((pVal.EventType == BoEventTypes.et_FORM_ACTIVATE || pVal.EventType == BoEventTypes.et_FORM_LOAD) && !pVal.BeforeAction && theForm.IsModal) {
                        _modalForm = theForm;
                    }

                    theForm.OnItemEvent(FormUID, ref pVal, out BubbleEvent);

                    if (pVal.EventType == BoEventTypes.et_FORM_CLOSE && pVal.BeforeAction && FormUID == theForm.FormID) {
                        _modalForm = null;
                    }
                }                

                if (pVal.EventType == BoEventTypes.et_FORM_UNLOAD && !pVal.BeforeAction) {
                    OnFormUnloadAfter(FormUID, pVal);
                }
            }
            catch (Exception ex) {
                var handled = _errorHandler.HandleException(ex);
                if (!handled) {
                    Exit();                    
                }
                else {
                    if (pVal.BeforeAction) {
                        BubbleEvent = false;
                    }
                }
            }
        }

        protected void OnApplicationEvent(BoAppEventTypes EventType) {
            switch (EventType) {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    OnAppShutDown(this, EventArgs.Empty);
                    Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    OnAppCompanyChanged(this, EventArgs.Empty);
                    Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    OnAppServerTermination(this, EventArgs.Empty);
                    Exit();
                    break;
            }
        }

        protected void OnAppShutDown(object sender, EventArgs e) {
            if (AppShutDown != null) {
                AppShutDown(sender, e);
            }
        }

        protected void OnAppCompanyChanged(object sender, EventArgs e) {
            if (AppCompanyChanged != null) {
                AppCompanyChanged(sender, e);
            }
        }

        protected void OnAppServerTermination(object sender, EventArgs e) {
            if (AppServerTermination != null) {
                AppServerTermination(sender, e);
            }
        }

        protected void OnFormLoadBefore(string formID, ItemEvent pVal) {
            if (!_forms.ContainsKey(formID)) {
                var theForm = _systemForms.TryResolve<IB1Form>(pVal.FormTypeEx);
                if (theForm != null) {
                    var apiForm = _sboApplication.Forms.Item(formID);
                    theForm.Load(apiForm);
                    _forms.Add(formID, theForm);
                }
            }            
        }

        protected void OnFormUnloadAfter(string formID, ItemEvent pVal) {
            IB1Form theForm = null;
            if (_forms.TryGetValue(formID, out theForm)) {
                _forms.Remove(formID);
                theForm.Dispose();
                theForm = null;                
            }
        }

        internal void AddForm(IB1Form form) {
            _forms.Add(form.FormID, form);
        }

        public void SetStatusBarError(string message) {
            SBOApplication.SetStatusBarMessage(message, BoMessageTime.bmt_Medium, true);
        }

        public void SetStatusBarSucess(string message) {
            SBOApplication.StatusBar.SetText(message, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Success);
        }

        public void Execute() {
            if (_autoMigrate) {
                RunAutoMigration();
            }
            if (BeforeRegisterForms != null) {
                BeforeRegisterForms(this, EventArgs.Empty);
            }
            LoadFormTypes();
            _initialized = true;
            System.Windows.Forms.Application.Run();
        }

        internal string GetNewFormID() {
            Random rdn = new Random();
            return string.Format("PRV{0:00000}{1}", SBOApplication.Forms.Count + 1, rdn.Next(0, 999));
        }

        public static void Run() {
            if (Current == null) {
                throw new B1CoreException("Application not initialized.");
            }
            if (!Current.Initialized) {
                Current.Execute();
            }            
        }

        public static void Exit(int exitCode) {
            System.Environment.Exit(exitCode);
        }

        public static void Exit() {
            Exit(0);
        }
    }
}
