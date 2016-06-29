//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework.Forms
{
    public abstract class B1Form : IB1Form
    {
        private bool _disposed = false;
        private B1Application _application;
        private string _formID;
        private string _formType;
        private Form _apiForm;
        private bool _isSystemForm = false;
        private bool _isModal = false;
        private bool _isPermissionAuto = false;
        private string _resourceName = "";
        private B1FormTypeAttribute _formTypeAttribute;
        private B1ContextMenu _contextMenu;
        private B1MenuClickArgs _menuContextArgs = null;

        public Form APIForm {
            get { return _apiForm; }
        }

        public B1Application App {
            get { return _application; }
        }

        public B1ContextMenu ContextMenu {
            get { return _contextMenu; }
        }

        public B1Form() {
            _application = B1Application.Current;
            if (_application == null) {
                throw new Exception("A aplicação do SAP Business One não foi inicializada");
            }
            _contextMenu = new B1ContextMenu(this);
            _formTypeAttribute = (B1FormTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(B1FormTypeAttribute));
            if (_formTypeAttribute != null) {
                _formType = _formTypeAttribute.FormType;
                _isSystemForm = _formTypeAttribute.IsSystemForm;
                _resourceName = _formTypeAttribute.ResourceName;
                _isModal = _formTypeAttribute.IsModal;
                _isPermissionAuto = _formTypeAttribute.IsPermissionAuto;

                if (_isSystemForm && _isModal) {
                    throw new Exception("O formulário não pode ser modal e formulário do sistema.");
                }
            }
            if (!_isSystemForm) {
                InitializeUserForm();
            }            
        }

        private void InitializeUserForm(){
            if (_isPermissionAuto && !App.PermissionManager.HasPermission(_formType)) {
                throw new B1Exception("Usuário não tem permissão para acessar esta funcionalidade.");
            }
            _formID = _application.GetNewFormID();
            if (_formTypeAttribute != null) {
                string xmlActions = "";
                if (!string.IsNullOrEmpty(_resourceName)) {
                    string typeNamespace = this.GetType().Namespace;
                    string firstNamespace = typeNamespace;
                    int index = typeNamespace.IndexOf(".");
                    if (index > 0) {
                        firstNamespace = typeNamespace.Substring(0, index + 1);
                    }
                    string resourceToFind = _resourceName;
                    if (!resourceToFind.Contains(firstNamespace)) {
                        resourceToFind = "." + resourceToFind;
                    }

                    var resourceNames = this.GetType().Assembly.GetManifestResourceNames();
                    string fullResourceName = resourceNames.Where(c => c.Contains(resourceToFind)).SingleOrDefault();                    
                    using (var stream = this.GetType().Assembly.GetManifestResourceStream(fullResourceName))
                    using (var reader = new StreamReader(stream)) {
                        xmlActions = reader.ReadToEnd();
                    }
                }
                var formParams = (FormCreationParams)_application.SBOApplication.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
                formParams.FormType = _formType;
                formParams.UniqueID = _formID;
                formParams.XmlData = xmlActions;
                _application.AddForm(this);
                _apiForm = _application.SBOApplication.Forms.AddEx(formParams);
            }
            else {
                _apiForm = _application.SBOApplication.Forms.Add(_formID);
                _formType = _apiForm.TypeEx;
                _application.AddForm(this);
            }            
            OnLoad(this, EventArgs.Empty);
        }


        #region IB1Form ...

        public string FormID {
            get { return _formID; }
        }

        public string FormType {
            get { return _formType; }
        }

        public bool IsInitialized {
            get { return true; }
        }

        public bool IsSystemForm {
            get { return _isSystemForm; }
        }

        public bool IsModal {
            get { return _isModal; }
        }

        public string ResourceName {
            get { return _resourceName; }
        }

        protected virtual void OnItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;
        }

        protected virtual void OnFormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent) {
            BubbleEvent = true;
        }

        protected virtual void OnMenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent) {
            BubbleEvent = true;
            if (pVal.BeforeAction && _menuContextArgs != null) {
                _menuContextArgs.MenuID = pVal.MenuUID;
                OnContextMenuClick(this, ref _menuContextArgs);
                BubbleEvent = !_menuContextArgs.Cancel;
            }
        }

        protected virtual void OnRightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent) {
            BubbleEvent = true;
            if (eventInfo.BeforeAction) {
                _menuContextArgs = new B1MenuClickArgs() {
                    FormID = eventInfo.FormUID,
                    ItemID = eventInfo.ItemUID,
                    ColID = eventInfo.ColUID,
                    Row = eventInfo.Row,
                    Cancel = false
                };
                _contextMenu.OnRightClickBefore(eventInfo);
            }
            else {
                try {
                    _contextMenu.OnRightClickAfter(eventInfo);
                }
                finally {
                    _menuContextArgs = null;
                }
            }
        }

        protected virtual void OnContextMenuClick(object sender, ref B1MenuClickArgs e) {

        }

        private void Load(SAPbouiCOM.Form sboForm) {
            if (IsSystemForm) {
                Check.IsNotNull("sboForm", sboForm);
                _apiForm = sboForm;
                _formID = sboForm.UniqueID;
                _formType = sboForm.TypeEx;
            }
            OnLoad(this, EventArgs.Empty);
        }        

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        public void Show() {
            APIForm.Visible = true;
        }

        #endregion

        public Item GetItem(string itemID) {
            return APIForm.Items.Item(itemID);
        }

        public T GetItem<T>(string itemID) {
            return (T)GetItem(itemID).Specific;
        }

        public Item AddItem(string itemID, BoFormItemTypes itemType) {
            return APIForm.Items.Add(itemID, itemType);
        }

        public T AddItem<T>(string itemID, BoFormItemTypes itemType) {
            var item = APIForm.Items.Add(itemID, itemType);
            return (T)item.Specific;
        }

        public void CenterForm() {
            CenterForm(true, true);
        }

        public void CenterForm(bool inVertical, bool inHorizontal) {
            if (inHorizontal) {
                APIForm.Left = (App.SBOApplication.Desktop.Width / 2) - (APIForm.Width / 2);
            }
            if (inVertical) {
                APIForm.Top = (App.SBOApplication.Desktop.Height / 2) - (APIForm.Height / 2) - 80;
            }
        }

        public void SetItemLeft(Item leftItm, Item rightItm, int margin = 5) {
            leftItm.Top = rightItm.Top;
            leftItm.Left = rightItm.Left - leftItm.Width - margin;
        }

        public void SetItemLeft(string leftItem, string rightItem, int margin = 5) {
            var lItm = GetItem(leftItem);
            var rItm = GetItem(rightItem);
            SetItemLeft(lItm, rItm, margin);
        }

        public void SetItemRight(Item leftItm, Item rightItm, int margin = 5) {
            rightItm.Top = leftItm.Top;
            rightItm.Left = leftItm.Left + leftItm.Width + margin;
        }

        public void SetItemRight(string leftItm, string rightItm, int margin = 5) {
            var lItm = GetItem(leftItm);
            var rItm = GetItem(rightItm);
            SetItemRight(lItm, rItm, margin);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    // Free other state (managed objects).
                }
                _apiForm = null;
                _disposed = true;
            }
        }

        protected virtual void OnLoad(object sender, EventArgs e) {

        }

        string IB1Form.FormID {
            get { return FormID; }
        }

        string IB1Form.FormType {
            get { return FormType; }
        }

        bool IB1Form.IsInitialized {
            get { return IsInitialized; }
        }

        bool IB1Form.IsSystemForm {
            get { return IsSystemForm; }
        }

        bool IB1Form.IsModal {
            get { return IsModal; }
        }

        string IB1Form.ResourceName {
            get { return ResourceName; }
        }

        void IB1Form.OnItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent) {
            OnItemEvent(FormUID, ref pVal, out BubbleEvent);
        }

        void IB1Form.OnFormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent) {
            OnFormDataEvent(ref BusinessObjectInfo, out BubbleEvent);
        }

        void IB1Form.OnMenuEvent(ref MenuEvent pVal, out bool BubbleEvent) {
            OnMenuEvent(ref pVal, out BubbleEvent);
        }

        void IB1Form.OnRightClickEvent(ref ContextMenuInfo eventInfo, out bool BubbleEvent) {
            OnRightClickEvent(ref eventInfo, out BubbleEvent);
        }

        void IB1Form.Load(Form sboForm) {
            Load(sboForm);
        }

        void IB1Form.Show() {
            Show();
        }

        void IB1Form.Select() {
            APIForm.Select();
        }

        void IDisposable.Dispose() {
            Dispose();
        }
    }
}
