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
using System.Text;

namespace insightOne.SB1.Framework.Data.Migrations
{
    public class SBOCommandInterpreter : IDataMigrationInterpreter
    {
        private B1Application _app;

        public SBOCommandInterpreter() {
            _app = B1Application.Current;
            Check.IsNotNull("app", _app);
        }

        private BoYesNoEnum YesNo(bool value) {
            return value ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
        }

        public void Visit(CreateTableCommand command) {
            var tableCreator = _app.BusinessObjects.GetBusinessObject<UserTablesMD>(BoObjectTypes.oUserTables);
            try {
                tableCreator.TableName = command.Name;
                tableCreator.TableDescription = command.Description;
                tableCreator.TableType = command.TableType;
                var created = tableCreator.Add();
                if (created != 0) {
                    string error = _app.BusinessObjects.GetLastErrorDescription();
                    throw new B1CoreException(string.Format("Error: {0} - tabela: {1}", error, command.Name));
                }                
            }
            finally {
                _app.BusinessObjects.ReleaseObject(tableCreator);
            }
            foreach (var columnCommand in command.TableCommands) {
                Visit(columnCommand);
            }
            foreach (var indexCommand in command.IndexCommands.Values) {
                Visit(indexCommand);
            }
            //Mostra a mensagem antes de criar o User Object, pois este é um comando bonus e pode ser
            //criado separadamente
            _app.SetStatusBarSucess(string.Format("Tabela {0} criada com sucesso.", command.Name));

            if (command.HasObject) {
                Visit(command.ObjectCommand);
            }            
        }

        public void Visit(AlterTableCommand command) {
            foreach (var columnCommand in command.TableCommands) {
                Visit(columnCommand);
            }
            foreach (var indexCommand in command.IndexCommands.Values) {
                Visit(indexCommand);
            }
            _app.SetStatusBarSucess(string.Format("Tabela {0} alterada com sucesso.", command.Name));
        }

        public void Visit(CreateColumnCommand command) {
            var columnCreator = _app.BusinessObjects.GetBusinessObject<UserFieldsMD>(BoObjectTypes.oUserFields);
            try {
                columnCreator.TableName = command.TableName;
                columnCreator.Name = command.Name;
                columnCreator.Description = command.Description;
                columnCreator.Type = command.ColumnType;
                columnCreator.SubType = command.SubType;
                columnCreator.Mandatory = YesNo(command.IsMandatory);
                columnCreator.LinkedTable = command.LinkedTable;
                columnCreator.EditSize = command.Length;
                columnCreator.DefaultValue = command.DefaultValue;
                foreach (var val in command.ValidValues) {
                    columnCreator.ValidValues.Value = val.Key;
                    columnCreator.ValidValues.Description = val.Value;
                    columnCreator.ValidValues.Add();
                }
                var created = columnCreator.Add();
                if (created != 0) {
                    string error = _app.BusinessObjects.GetLastErrorDescription();
                    throw new B1CoreException(string.Format("Error: {0} - campo: {1}", error, command.Name));
                }
            }
            finally {
                _app.BusinessObjects.ReleaseObject(columnCreator);
            }
        }
        public void Visit(AlterObjectCommand command)
        {
            var objectCreator = _app.BusinessObjects.GetBusinessObject<UserObjectsMD>(BoObjectTypes.oUserObjectsMD);
            try
            {
                if (!objectCreator.GetByKey(command.Name))
                {
                    throw new B1CoreException("Object not found.");
                }

                foreach (var name in command.ChildTables)
                {
                    objectCreator.ChildTables.TableName = name;
                    objectCreator.ChildTables.Add();
                }

                foreach (var field in command.FindFields)
                {
                    objectCreator.FindColumns.ColumnAlias = field.Key;
                    if (!string.IsNullOrEmpty(field.Value))
                    {
                        objectCreator.FindColumns.ColumnDescription = field.Value;
                    }
                    objectCreator.FindColumns.Add();
                }

                var created = objectCreator.Update();
                if (created != 0)
                {
                    string error = _app.BusinessObjects.GetLastErrorDescription();
                    throw new B1CoreException(string.Format("Error: {0} - alterar objeto: {1}", error, command.Name));
                }
                _app.SetStatusBarSucess(string.Format("Objeto de usuário {0} alterado com sucesso.", command.Name));
            }
            finally
            {
                _app.BusinessObjects.ReleaseObject(objectCreator);
            }
        }
        public void Visit(CreateObjectCommand command) {
            var objectCreator = _app.BusinessObjects.GetBusinessObject<UserObjectsMD>(BoObjectTypes.oUserObjectsMD);
            try {
                objectCreator.Code = command.Name;
                objectCreator.Name = command.Description;
                objectCreator.TableName = command.TableName;
                objectCreator.ObjectType = command.ObjectType;
                objectCreator.CanFind = YesNo(command.CanFind);
                objectCreator.CanCancel = YesNo(command.CanCancel);
                objectCreator.CanClose = YesNo(command.CanClose);
                objectCreator.CanDelete = YesNo(command.CanDelete);
                objectCreator.CanLog = YesNo(command.CanLog);
                objectCreator.CanYearTransfer = YesNo(command.CanYearTransfor);
                objectCreator.CanArchive = YesNo(command.CanArchive);
                objectCreator.ManageSeries = YesNo(command.ManageSeries);
                objectCreator.UseUniqueFormType = YesNo(command.UniqueFormType);

                foreach (var name in command.ChildTables) {
                    objectCreator.ChildTables.TableName = name;
                    objectCreator.ChildTables.Add();
                }

                foreach (var field in command.FindFields) {
                    objectCreator.FindColumns.ColumnAlias = field.Key;
                    if (!string.IsNullOrEmpty(field.Value)) {
                        objectCreator.FindColumns.ColumnDescription = field.Value;
                    }
                    objectCreator.FindColumns.Add();
                }

                var created = objectCreator.Add();
                if (created != 0) {
                    string error = _app.BusinessObjects.GetLastErrorDescription();
                    throw new B1CoreException(string.Format("Error: {0} - objeto: {1}", error, command.Name));
                }
                _app.SetStatusBarSucess(string.Format("Objeto de usuário {0} criado com sucesso.", command.Name));
            }
            finally {
                _app.BusinessObjects.ReleaseObject(objectCreator);
            }
        }

        public void Visit(CreateIndexCommand command) {
            var indexCreator = _app.BusinessObjects.GetBusinessObject<UserKeysMD>(BoObjectTypes.oUserKeys);
            try {
                indexCreator.TableName = command.TableName;
                indexCreator.KeyName = command.IndexName;
                int elementLine = 0;
                foreach (var col in command.FieldsName) {
                    if (elementLine > 0) {
                        indexCreator.Elements.Add();
                        indexCreator.Elements.SetCurrentLine(elementLine);
                    }                    
                    indexCreator.Elements.ColumnAlias = col;
                    elementLine++;
                }
                indexCreator.Unique = YesNo(command.IsUnique);
                var created = indexCreator.Add();
                if (created != 0) {
                    string error = _app.BusinessObjects.GetLastErrorDescription();
                    throw new B1CoreException(string.Format("Error: {0} - index: {1}", error, command.FieldsName));
                }
            }
            finally {
                _app.BusinessObjects.ReleaseObject(indexCreator);
            }
        }        

        public bool TableExists(string tableName) {
            var exists = false;
            var tableCreator = _app.BusinessObjects.GetBusinessObject<UserTablesMD>(BoObjectTypes.oUserTables);
            try {
                exists = tableCreator.GetByKey(tableName);
            }
            finally {
                _app.BusinessObjects.ReleaseObject(tableCreator);
            }
            return exists;
        }

        /// <summary>
        /// Remove um user object se ele existir no SAP
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public bool DropObject(string objName) {
            bool drop = false;
            var objectCreator = _app.BusinessObjects.GetBusinessObject<UserObjectsMD>(BoObjectTypes.oUserObjectsMD);
            try {
                drop = objectCreator.GetByKey(objName);

                if (drop) {
                    if (objectCreator.Remove() != 0) {
                        string error = _app.BusinessObjects.GetLastErrorDescription();
                        throw new B1CoreException(string.Format("Error: {0} - objeto: {1}", error, objName));
                    }
                    _app.SetStatusBarSucess(string.Format("Objeto {0} excluido.", objName));
                }
            }
            finally {
                _app.BusinessObjects.ReleaseObject(objectCreator);
            }
            return drop;
        }

        public bool DropTable(string tableName) {
            bool drop = false;
            var tableCreator = _app.BusinessObjects.GetBusinessObject<UserTablesMD>(BoObjectTypes.oUserTables);
            try {
                drop = tableCreator.GetByKey(tableName);

                if (drop) {
                    if (tableCreator.Remove() != 0) {
                        string error = _app.BusinessObjects.GetLastErrorDescription();
                        throw new B1CoreException(string.Format("Error: {0} - table: {1}", error, tableName));
                    }
                    _app.SetStatusBarSucess(string.Format("Tabela {0} excluida.", tableName));
                }
            }
            finally {
                _app.BusinessObjects.ReleaseObject(tableCreator);
            }
            return drop;
        }
    }
}
