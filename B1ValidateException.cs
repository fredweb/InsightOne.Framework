using insightOne.SB1.Framework.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework
{
    public class B1ValidateException : B1Exception
    {
        public B1ValidateException(string message, Exception innerException)
            : base(message, innerException) {
                
        }

        public B1ValidateException(string message)
            : base(message) {

        }

        public B1ValidateException(string message, bool showInMessageBox)
            : base(message) {
            if (showInMessageBox) {
                B1Application.Current.SBOApplication.MessageBox(message);
            }
        }
    }
}
