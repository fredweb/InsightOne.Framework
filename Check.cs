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
    static class Check
    {
        public static void IsNotNull(string paramName, object argument) {
            if (null == argument) {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void IsNotNullOrEmpty(string paramName, string argument) {
            if (string.IsNullOrEmpty(argument)) {
                throw new ArgumentException("O parâmetro não pode ser nulo ou vazio", paramName);
            }
        }
    }
}
