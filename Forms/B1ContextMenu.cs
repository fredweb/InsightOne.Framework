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
using System.Text;

namespace insightOne.SB1.Framework.Forms
{
    public class B1ContextMenu
    {
        private Dictionary<string, MenuItemHelper> _menuItems;
        private B1Form _parentForm;

        public B1ContextMenu(B1Form parentForm) {
            Check.IsNotNull("parentForm", parentForm);
            _parentForm = parentForm;
            _menuItems = new Dictionary<string, MenuItemHelper>();
        }

        public void OnRightClickBefore(ContextMenuInfo menuContext) {
            foreach (var menu in _menuItems.Values) {
                if (string.IsNullOrEmpty(menu.ControlContext) || menu.ControlContext == menuContext.ItemUID) {
                    if (menu.IsSystemMenu) {
                        _parentForm.APIForm.EnableMenu(menu.MenuID, true);
                    }
                    else {
                        _parentForm.App.Menus.Add(menu.ParentID, menu.MenuID, menu.Caption, BoMenuType.mt_STRING, menu.Position);
                    }
                }
            }            
        }

        public void OnRightClickAfter(ContextMenuInfo menuContext) {
            foreach (var menu in _menuItems.Values) {
                if (string.IsNullOrEmpty(menu.ControlContext) || menu.ControlContext == menuContext.ItemUID) {
                    if (menu.IsSystemMenu) {
                        if (!string.IsNullOrEmpty(menu.ControlContext))
                            _parentForm.APIForm.EnableMenu(menu.MenuID, false);
                    }
                    else {
                        _parentForm.App.Menus.Remove(menu.MenuID);
                    }
                }
            }
        }

        public void Remove(string menuID) {
            _menuItems.Remove(menuID);
        }

        public void Add(string menuID, string caption, int position, string controlContext) {
            var menu = new MenuItemHelper() {
                MenuID = menuID,
                ParentID = "1280",
                Caption = caption,
                Position = position,
                ControlContext = controlContext,
                IsSystemMenu = false
            };
            _menuItems.Add(menu.MenuID, menu);
        }        

        public void Add(string menuID, string caption, int position) {
            Add(menuID, caption, position, null);
        }

        public void Enable(string menuID, string controlContext) {
            var menu = new MenuItemHelper() {
                MenuID = menuID,                
                ControlContext = controlContext,
                IsSystemMenu = true
            };
            _menuItems.Add(menu.MenuID, menu);
        }

        public void Enable(string menuID) {
            Enable(menuID, String.Empty);
        }

        class MenuItemHelper
        {
            public string MenuID { get; set; }

            public string ParentID { get; set; }

            public int Position { get; set; }

            public string Caption { get; set; }

            public string ControlContext { get; set; }

            public bool IsSystemMenu { get; set; }
        }
    }
}
