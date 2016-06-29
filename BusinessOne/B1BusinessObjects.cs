//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using insightOne.SB1.Framework.Forms;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace insightOne.SB1.Framework.BusinessOne
{
    public class B1BusinessObjects
    {
        private readonly B1Application _app;
        private readonly Company _diCompany;

        public B1Application App { get { return _app; } }

        public Company DICompany { get { return _diCompany; } }

        public B1BusinessObjects(B1Application app) {
            Check.IsNotNull("app", app);
            _app = app;
            _diCompany = _app.SBOCompany;
        }

        public void StartTransaction() {
            DICompany.StartTransaction();
        }

        public void CommitTransaction() {
            if (DICompany.InTransaction) {
                DICompany.EndTransaction(BoWfTransOpt.wf_Commit);
            }
        }

        public void RollbackTransaction() {
            if (DICompany.InTransaction) {
                DICompany.EndTransaction(BoWfTransOpt.wf_RollBack);
            }
        }

        public void EndTransaction(BoWfTransOpt opt) {
            DICompany.EndTransaction(opt);
        }

        public string GetNewObjectCode() {
            string code = "";
            DICompany.GetNewObjectCode(out code);
            return code;
        }

        public string GetNewObjectKey() {
            return DICompany.GetNewObjectKey();
        }

        public string GetNewObjectType() {
            return DICompany.GetNewObjectType();
        }

        public string GetLastErrorDescription() {
            return DICompany.GetLastErrorDescription();
        }

        public int GetLastErrorCode() {
            return DICompany.GetLastErrorCode();
        }

        public void GetLastError(out int code, out string message) {
            DICompany.GetLastError(out code, out message);
        }

        public void ThrowLastErrorException() {
            string message = "";
            int code = 0;
            GetLastError(out code, out message);
            throw new B1Exception(string.Format("Error: {0} - {1}", code, message));
        }

        public void ReleaseObject(object obj) {
            Marshal.ReleaseComObject(obj);
        }

        public T GetBusinessObject<T>(BoObjectTypes objType) {
            return (T)DICompany.GetBusinessObject(objType);
        }        

        public Recordset GetRecordset() {
            return GetBusinessObject<Recordset>(BoObjectTypes.BoRecordset);
        }
    }
}
