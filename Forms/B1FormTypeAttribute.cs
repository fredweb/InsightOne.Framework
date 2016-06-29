//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework.Forms
{
    [AttributeUsage(AttributeTargets.Class)]
    public class B1FormTypeAttribute : Attribute
    {
        public virtual string FormType { get; private set; }

        public virtual bool IsSystemForm { get; set; }

        public virtual string ResourceName { get; set; }

        public virtual bool IsMenuAuto { get; set; }

        public virtual bool IsPermissionAuto { get; set; }

        public virtual bool CreateInMenu { get; set; }

        public virtual string MenuParent { get; set; }

        public virtual string MenuCaption { get; set; }

        public virtual int MenuPosition { get; set; }

        public virtual bool IsModal { get; set; }
               
        public B1FormTypeAttribute(string formType)
        {
            FormType = formType;
            IsSystemForm = false;
            ResourceName = "";
            IsMenuAuto = false;
            CreateInMenu = false;
            IsModal = false;
            IsPermissionAuto = false;
            MenuParent = "";
            MenuCaption = "";
            MenuPosition = 0;
        }        
    }
}
