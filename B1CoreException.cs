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

namespace insightOne.SB1.Framework
{
    public class B1CoreException : Exception
    {
        public B1CoreException(string message)
            : base(message) {

        }

        public B1CoreException(string message, Exception innerException)
            : base(message, innerException) {

        }

    }
}
