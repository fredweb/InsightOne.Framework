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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace insightOne.SB1.Framework.Forms
{
    public class B1AppMenu
    {
        private readonly B1Application _app;
        public B1AppMenu(B1Application app) {
            Check.IsNotNull("app", app);
            _app = app;
        }

        public B1AppMenu() : this(B1Application.Current) {
            
        }

        public B1Application App {
            get { return _app; }
        }

        public Menus APIMenus {
            get { return _app.SBOApplication.Menus; }
        }

        public MenuItem Add(string parentID, string menuID, string caption, BoMenuType menuType, int position) {
            var subMenu = _app.SBOApplication.Menus.Item(parentID).SubMenus;
            try {
                return subMenu.Add(menuID, caption, menuType, position);
            }
            catch (COMException) {
                //FIX: Não pode ser assim pois acoberta a exeção do menu
                return subMenu.Item(menuID);
            }
        }

        public MenuItem Add(string parentID, string menuID, string caption, int position) {
            var subMenu = _app.SBOApplication.Menus.Item(parentID).SubMenus;
            try {
                return subMenu.Add(menuID, caption, BoMenuType.mt_STRING, position);
            }
            catch (COMException) {
                return subMenu.Item(menuID);
            }
        }

        public MenuItem Add(string parentID, string menuID, string caption) {
            var subMenu = _app.SBOApplication.Menus.Item(parentID).SubMenus;
            try {
                return subMenu.Add(menuID, caption, BoMenuType.mt_STRING, subMenu.Count);
            }
            catch (COMException) {
                return subMenu.Item(menuID);
            }
        }

        public MenuItem Add(string parentID, MenuCreationParams menuParams) {
            var subMenu = _app.SBOApplication.Menus.Item(parentID).SubMenus;
            try {                
                return subMenu.AddEx(menuParams);
            }
            catch (COMException) {
                return subMenu.Item(menuParams.UniqueID);
            }
        }

        public MenuCreationParams GetMenuCreationParams() {
            return (SAPbouiCOM.MenuCreationParams)_app.SBOApplication.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
        }

        public void Remove(string menuID) {
            try {
                _app.SBOApplication.Menus.RemoveEx(menuID);
            }
            catch (COMException) {
                //não faz nada
            }
        }
    }
}
