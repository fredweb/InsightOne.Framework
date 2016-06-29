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
    public class SchemaBuilder
    {
        private IDataMigrationInterpreter _interpreter;
        

        public SchemaBuilder(IDataMigrationInterpreter interpreter) {
            Check.IsNotNull("interpreter", interpreter);
            _interpreter = interpreter;
        }        

        public SchemaBuilder CreateTable(string name, string description, Action<CreateTableCommand> table = null) {
            var createTable = new CreateTableCommand(name, description);
            if (table != null) {
                table(createTable);
            }
            Run(createTable);
            return this;
        }

        public SchemaBuilder CreateDocumentLines(string name, string description, Action<CreateTableCommand> table) {
            var createTable = new CreateTableCommand(name, description);
            createTable.WithType(BoUTBTableType.bott_DocumentLines);
            table(createTable);
            Run(createTable);
            return this;
        }

        public SchemaBuilder CreateDocument(string name, string description, Action<CreateTableCommand> table) {
            var createTable = new CreateTableCommand(name, description);
            createTable.WithType(BoUTBTableType.bott_Document);
            table(createTable);
            Run(createTable);
            return this;
        }

        public SchemaBuilder CreateMaster(string name, string description, Action<CreateTableCommand> table) {
            var createTable = new CreateTableCommand(name, description);
            createTable.WithType(BoUTBTableType.bott_MasterData);
            table(createTable);
            Run(createTable);
            return this;
        }

        public SchemaBuilder CreateMasterLines(string name, string description, Action<CreateTableCommand> table) {
            var createTable = new CreateTableCommand(name, description);
            createTable.WithType(BoUTBTableType.bott_MasterDataLines);
            table(createTable);
            Run(createTable);
            return this;
        }

        public SchemaBuilder CreateObject(string tableName, string name, string description, Action<CreateObjectCommand> userObject) {
            var command = new CreateObjectCommand(tableName, name, description);
            userObject(command);
            Run(command);
            return this;            
        }

        public SchemaBuilder AlterTable(string name, Action<AlterTableCommand> table = null) {
            var alterTable = new AlterTableCommand(name);
            if (table != null) {
                table(alterTable);
            }
            Run(alterTable);
            return this;
        }

        public SchemaBuilder AlterObject(string name, Action<AlterObjectCommand> userObject = null)
        {
            var userObjectCommand = new AlterObjectCommand(name);
            if (userObject != null)
            {
                userObject(userObjectCommand);
            }
            Run(userObjectCommand);
            return this;
        }

        protected void Run(CreateTableCommand command) {
            _interpreter.Visit(command);
        }

        protected void Run(CreateObjectCommand command) {
            _interpreter.Visit(command);
        }

        protected void Run(AlterTableCommand command) {
            _interpreter.Visit(command);
        }

        protected void Run(AlterObjectCommand command)
        {
            _interpreter.Visit(command);
        }

        public bool TableExists(string tableName) {
            return _interpreter.TableExists(tableName);
        }

        public bool DropTable(string tableName) {
            return _interpreter.DropTable(tableName);
        }

        public bool DropObject(string objName) {
            return _interpreter.DropObject(objName);
        }
        
    }
}
