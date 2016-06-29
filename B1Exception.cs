//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using insightOne.SB1.Framework.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework
{
    public class B1Exception : Exception
    {

        public B1Exception(string message, Exception innerException)
            : base(message, innerException) {
                if (B1Application.Current != null) {
                    B1Application.Current.SetStatusBarError(message);
                }
        }

        public B1Exception(string message)
            : base(message) {
                if (B1Application.Current != null) {
                    B1Application.Current.SetStatusBarError(message);
                }
        }
    }
}
