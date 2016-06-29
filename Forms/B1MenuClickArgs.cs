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
    public class B1MenuClickArgs : EventArgs
    {
        public string MenuID { get; set; }
        public string FormID { get; set; }
        public string ItemID { get; set; }
        public string ColID { get; set; }
        public int Row { get; set; }
        public bool Cancel { get; set; }
    }
}
