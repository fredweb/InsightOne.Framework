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
    public class CreateTableCommand : TableCommand
    {
        public bool HasObject { get; protected set; }

        public CreateObjectCommand ObjectCommand { get; protected set; }

        public CreateTableCommand(string name, string description)
        : base(name, description) {            
            HasObject = false;
            ObjectCommand = null;            
        }

        public CreateTableCommand WithType(BoUTBTableType tableType) {
            TableType = tableType;
            return this;
        }

        public CreateTableCommand WithObject(string objectName, string objectDescription, Action<CreateObjectCommand> userObject = null) {
            HasObject = true;
            var command = new CreateObjectCommand(Name, objectName, objectDescription);
            switch (TableType) {
                case BoUTBTableType.bott_Document:
                    command.WithType(BoUDOObjType.boud_Document);
                    break;
                case BoUTBTableType.bott_MasterData:
                    command.WithType(BoUDOObjType.boud_MasterData);
                break;
            }
            if (userObject != null) {
                userObject(command);
            }
            ObjectCommand = command;
            return this;
        }

        public CreateTableCommand WithObject(string objectName, Action<CreateObjectCommand> userObject) {
            HasObject = true;
            var command = new CreateObjectCommand(Name, objectName, Description);
            switch (TableType) {
                case BoUTBTableType.bott_Document:
                    command.WithType(BoUDOObjType.boud_Document);
                    break;
                case BoUTBTableType.bott_MasterData:
                    command.WithType(BoUDOObjType.boud_MasterData);
                    break;
            }
            if (userObject != null) {
                userObject(command);
            }
            ObjectCommand = command;
            return this;
        }

        public CreateTableCommand Column(string columnName, string columnDescription, Action<CreateColumnCommand> column) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand Column(string columnName, string columnDescription, BoFieldTypes dbType, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command.WithType(dbType);
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand Alpha(string columnName, string columnDescription, int length, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command.WithType(BoFieldTypes.db_Alpha);
            command.WithLength(length);
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand Integer(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command
                .WithType(BoFieldTypes.db_Numeric)
                .WithSubType(BoFldSubTypes.st_None)
                .WithLength(11);
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand Price(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command
                .WithType(BoFieldTypes.db_Float)
                .WithSubType(BoFldSubTypes.st_Price)
                .WithLength(0)
                .WithDefault("0");
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand Quantity(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command
                .WithType(BoFieldTypes.db_Float)
                .WithSubType(BoFldSubTypes.st_Quantity)
                .WithLength(0)
                .WithDefault("0");
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand Percent(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command
                .WithType(BoFieldTypes.db_Float)
                .WithSubType(BoFldSubTypes.st_Percentage)
                .WithLength(0)
                .WithDefault("0");
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand Date(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command
                .WithType(BoFieldTypes.db_Date)
                .WithLength(0);
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand ValidValues(string columnName, string columnDescription, Dictionary<string,string> values, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command
                .WithType(BoFieldTypes.db_Alpha)
                .WithLength(30);
            foreach (var val in values) {
                command.AddValidValue(val.Key, val.Value);
            }
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public CreateTableCommand CreateIndex(string indexName, Action<CreateIndexCommand> index) {
            var command = new CreateIndexCommand(Name, indexName);
            index(command);
            IndexCommands.Add(indexName, command);
            return this;
        }

        public void NeedIndex(CreateColumnCommand command) {
            if (command.IsIndex) {
                bool hasIndex = true;
                CreateIndexCommand indexCommand = null;
                if (!IndexCommands.TryGetValue(command.IndexName, out indexCommand)) {
                    indexCommand = new CreateIndexCommand(Name, command.IndexName);
                    hasIndex = false;
                }
                indexCommand.AddField(command.Name);
                if (command.IsUnique) {
                    indexCommand.Unique();
                }
                if (!hasIndex) {
                    IndexCommands.Add(indexCommand.IndexName, indexCommand);
                }
            }
        }
    }
}
