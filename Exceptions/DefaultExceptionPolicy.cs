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

namespace insightOne.SB1.Framework.Exceptions
{
    class DefaultExceptionPolicy : IExceptionPolicy
    {
        private const string _newLine = "\r\n";
        private const string _title = "Provider";
        public bool HandleException(Exception ex) {
            if (ex is B1Exception) {
                return true;
            }
            var message = ex.ToString();
            message = _newLine + _newLine + message;
            System.Windows.Forms.MessageBox.Show(message, _title, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return false;
        }
    }
}
