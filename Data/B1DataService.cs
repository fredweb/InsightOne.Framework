using insightOne.SB1.Framework.Forms;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace insightOne.SB1.Framework.Data
{
    public class B1DataService : IDisposable
    {
        private bool _disposed = false;
        private readonly B1Application _app;
        private CompanyService _companyService;
        private GeneralService _generalService;

        public string UDOName { get; protected set; }

        public B1DataService(B1Application app, string udoName) {
            Check.IsNotNull("app", app);
            Check.IsNotNullOrEmpty("udoName", udoName);
            _app = app;
            UDOName = udoName;
            _companyService = _app.SBOCompany.GetCompanyService();
            _generalService = _companyService.GetGeneralService(udoName);
        }
        public B1DataService(string udoName) : this(B1Application.Current, udoName) {

        }

        public GeneralService SBOGeneralService {
            get { return _generalService; }
        }

        /// <summary>
        /// <para>Get an data interface provided by the SAPbobsCOM.GeneralServiceDataInterfaces</para>
        /// <para>Can be:</para>
        /// <para>gsGeneralDataCollection:    SAPbobsCOM.GeneralDataCollection</para>
        /// <para>gsGeneralData:              SAPbobsCOM.GeneralData</para>
        /// <para>gsGeneralCollectionParams:  SAPbobsCOM.GeneralCollectionParams</para>
        /// <para>gsGeneralDataParams:        SAPbobsCOM.GeneralDataParams</para>
        /// <para>gsInvokeParams:             SAPbobsCOM.InvokeParams</para>
        /// </summary>
        /// <typeparam name="T">SAPbobsCOM.GeneralDataCollection; SAPbobsCOM.GeneralData; SAPbobsCOM.GeneralCollectionParams; SAPbobsCOM.GeneralDataParams; SAPbobsCOM.InvokeParams</typeparam>
        /// <param name="enumItf">GeneralServiceDataInterfaces</param>
        /// <returns>One classe base on the interface selected</returns>
        public T GetDataInterface<T>(GeneralServiceDataInterfaces enumItf) {
            return (T)_generalService.GetDataInterface(enumItf);
        }

        /// <summary>
        /// Get a GeneralData to add data in the database
        /// </summary>
        /// <returns>Returns a SAPbobsCOM.GeneralData interface</returns>
        public GeneralData GetData() {            
            return _generalService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData) as GeneralData;
        }

        /// <summary>
        /// Return an UDO data by an providerd param. Eg.: GetDataByKey("Code", "A0001")
        /// </summary>
        /// <param name="paramName">Name of the field to find the data</param>
        /// <param name="paramValue">Value of the field to find the data</param>
        /// <returns>Return a SAPbobsCOM.GeneralData find in values provided</returns>
        public GeneralData GetDataByKey(string paramName, object paramValue) {
            var generalParams = GetDataInterface<GeneralDataParams>(GeneralServiceDataInterfaces.gsGeneralDataParams);
            generalParams.SetProperty(paramName, paramValue);
            return _generalService.GetByParams(generalParams);
        }

        public bool TryGetDataByKey(string paramName, object paramValue, out GeneralData pData) {
            try {
                var generalParams = GetDataInterface<GeneralDataParams>(GeneralServiceDataInterfaces.gsGeneralDataParams);
                generalParams.SetProperty(paramName, paramValue);
                pData = _generalService.GetByParams(generalParams);
                return true;
            }
            catch (Exception ex) {
                //TODO: LOG THE ERROR
                pData = null;
            }
            return false;
        }

        /// <summary>
        /// Return an UDO data by an providerd keys
        /// </summary>
        /// <param name="keyValues">Contains the field name and value</param>
        /// <param name="pData">return the GeneralData with values</param>
        /// <returns>true if success false otherwise</returns>
        public bool TryGetDataByKeys(IDictionary<string, object> keyValues, out GeneralData pData) {
            try {
                var generalParams = GetDataInterface<GeneralDataParams>(GeneralServiceDataInterfaces.gsGeneralDataParams);
                foreach (var key in keyValues.Keys) {
                    generalParams.SetProperty(key, keyValues[key]);
                }
                pData = _generalService.GetByParams(generalParams);
            }
            catch (Exception ex) {
                //TODO: LOG THE ERROR
                pData = null;
            }

            return false;
        }

        /// <summary>
        /// Add data in database
        /// </summary>
        /// <param name="gData">Data to add</param>
        /// <returns>Return the key of the data: DocEntry or Code depends of the type of the UDO</returns>
        public GeneralDataParams Add(GeneralData gData) {
            return _generalService.Add(gData);
        }

        /// <summary>
        /// Update the data to the database
        /// </summary>
        /// <param name="gData">Data to update</param>
        public void Update(GeneralData gData) {
            _generalService.Update(gData);
        }

        /// <summary>
        /// Set the status of the UDO to closed. This operations is valid only for document-type UDOs
        /// </summary>
        /// <param name="paramName">Name of the key of the UDO: "DocEntry"</param>
        /// <param name="paramValue">Value of the key of the UDO</param>
        public void Close(string paramName, object paramValue) {
            var generalParams = GetDataInterface<GeneralDataParams>(GeneralServiceDataInterfaces.gsGeneralDataParams);
            generalParams.SetProperty(paramName, paramValue);
            _generalService.Close(generalParams);
        }

        /// <summary>
        /// Deletes a row in the database table of the current UDO.
        /// </summary>
        /// <param name="paramName">Name of the key of the UDO: "DocEntry" or "Code"</param>
        /// <param name="paramValue">Value of the key of the UDO</param>
        public void Delete(string paramName, object paramValue) {
            var generalParams = GetDataInterface<GeneralDataParams>(GeneralServiceDataInterfaces.gsGeneralDataParams);
            generalParams.SetProperty(paramName, paramValue);
            _generalService.Delete(generalParams);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    Marshal.ReleaseComObject(_generalService);
                    Marshal.ReleaseComObject(_companyService);
                }
                _generalService = null;
                _companyService = null;
                _disposed = true;
            }
        }

    }
}
