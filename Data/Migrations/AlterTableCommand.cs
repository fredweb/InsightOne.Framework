//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework.Data.Migrations
{
    public class AlterTableCommand : TableCommand
    {
        public AlterTableCommand(string name)
            : base(name, "") {

        }

        public AlterTableCommand Column(string columnName, string columnDescription, Action<CreateColumnCommand> column) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public AlterTableCommand Column(string columnName, string columnDescription, BoFieldTypes dbType, Action<CreateColumnCommand> column = null) {
            var command = new CreateColumnCommand(Name, columnName, columnDescription);
            command.WithType(dbType);
            if (column != null) {
                column(command);
            }
            TableCommands.Add(command);
            NeedIndex(command);
            return this;
        }

        public AlterTableCommand Alpha(string columnName, string columnDescription, int length, Action<CreateColumnCommand> column = null) {
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

        public AlterTableCommand Integer(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
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

        public AlterTableCommand Price(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
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

        public AlterTableCommand Quantity(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
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

        public AlterTableCommand Percent(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
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

        public AlterTableCommand Date(string columnName, string columnDescription, Action<CreateColumnCommand> column = null) {
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

        public AlterTableCommand ValidValues(string columnName, string columnDescription, Dictionary<string, string> values, Action<CreateColumnCommand> column = null) {
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

        public AlterTableCommand CreateIndex(string indexName, Action<CreateIndexCommand> index) {
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
