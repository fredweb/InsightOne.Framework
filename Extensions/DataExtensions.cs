using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace insightOne.SB1.Framework.Extensions
{
    public static class RecordsetExtensions
    {
        public static object GetValue(this Recordset recordset, string fieldName) {
            return recordset.Fields.Item(fieldName).Value;
        }

        public static string GetValueAsString(this Recordset recordset, string fieldName) {
            return Convert.ToString(recordset.Fields.Item(fieldName).Value);
        }

        public static double GetValueAsDouble(this Recordset recordset, string fieldName) {
            return Convert.ToDouble(recordset.Fields.Item(fieldName).Value);
        }

        public static int GetValueAsInt(this Recordset recordset, string fieldName) {
            return Convert.ToInt32(recordset.Fields.Item(fieldName).Value);
        }

        public static void SetValue(this Recordset recordset, string fieldName, object fieldValue) {
            recordset.Fields.Item(fieldName).Value = fieldValue;
        }

        public static void DoQueryFormat(this Recordset recordset, string format, object arg1) {
            recordset.DoQuery(string.Format(format, arg1));
        }

        public static void DoQueryFormat(this Recordset recordset, string format, object arg1, object arg2) {
            recordset.DoQuery(string.Format(format, arg1, arg2));
        }

        public static void DoQueryFormat(this Recordset recordset, string format, params object[] args) {
            recordset.DoQuery(string.Format(format, args));
        }

        public static void ReleaseComObject(this Recordset recordset) {
            Marshal.ReleaseComObject(recordset);
        }
    }

    public static class DataTableExtensions
    {
        public static string GetValueAsString(this DataTable dataTable, string fieldName, int rowIndex) {
            return Convert.ToString(dataTable.GetValue(fieldName, rowIndex));
        }

        public static double GetValueAsDouble(this DataTable dataTable, string fieldName, int rowIndex) {
            return Convert.ToDouble(dataTable.GetValue(fieldName, rowIndex));
        }

        public static int GetValueAsInt32(this DataTable dataTable, string fieldName, int rowIndex) {
            return Convert.ToInt32(dataTable.GetValue(fieldName, rowIndex));
        }

        public static void SetValueOffset(this DataTable dataTable, string column, object value) {
            dataTable.SetValue(column, dataTable.Rows.Offset, value);
        }
    }

    public static class DBDataSourceExtensions
    {
        public static string Val(this DBDataSource dataTable, string field) {
            return dataTable.GetValue(field, dataTable.Offset);
        }

        public static double GetValueAsDouble(this DBDataSource dataTable, string fieldName, int rowIndex) {
            return Convert.ToDouble(dataTable.GetValue(fieldName, rowIndex), CultureInfo.InvariantCulture);
        }        

        public static DateTime GetValueAsDateTime(this DBDataSource dataTable, string field, int rowIndex) {
            DateTime returnDate = DateTime.MinValue;
            if (!DateTime.TryParseExact(dataTable.GetValue(field, rowIndex), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out returnDate)) {
                return DateTime.MinValue;
            }

            return returnDate;
        }
    }
}
