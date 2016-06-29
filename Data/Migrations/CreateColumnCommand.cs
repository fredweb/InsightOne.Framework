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
    public class CreateColumnCommand
    {
        private Dictionary<string, string> _validValues;

        public string TableName { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool IsMandatory { get; protected set; }
        public bool IsIndex { get; protected set; }
        public string IndexName { get; protected set; }
        public bool IsUnique { get; protected set; }
        public BoFieldTypes ColumnType { get; protected set; }
        public BoFldSubTypes SubType { get; protected set; }
        public int Length { get; protected set; }
        public string DefaultValue { get; protected set; }
        public string LinkedTable { get; protected set; }
        public Dictionary<string, string> ValidValues { get { return _validValues; } }

        public CreateColumnCommand(string tableName, string name, string description) {
            _validValues = new Dictionary<string, string>();
            TableName = tableName;
            Name = name;
            Description = description;
            DefaultValue = "";
            LinkedTable = "";
            ColumnType = BoFieldTypes.db_Alpha;
            SubType = BoFldSubTypes.st_None;
            Length = 0;
        }

        public CreateColumnCommand NotNull() {
            IsMandatory = true;
            IsUnique = false;
            return this;
        }

        public CreateColumnCommand Nullable() {
            IsMandatory = false;
            return this;
        }

        public CreateColumnCommand Index(string indexName) {
            IsIndex = true;
            IndexName = indexName;
            return this;
        }

        public CreateColumnCommand WithLength(int length) {
            Length = length;
            return this;
        }

        public CreateColumnCommand WithType(BoFieldTypes fieldType) {
            ColumnType = fieldType;
            return this;
        }

        public CreateColumnCommand WithSubType(BoFldSubTypes subType) {
            SubType = subType;
            return this;
        }

        public CreateColumnCommand WithLinkedTable(string tableName) {
            LinkedTable = tableName;
            return this;
        }

        public CreateColumnCommand Unique(string indexName) {
            IsIndex = true;
            IsUnique = true;
            IsMandatory = false;
            this.IndexName = indexName;
            return this;
        }

        public CreateColumnCommand NotUnique() {
            IsUnique = false;
            return this;
        }

        public CreateColumnCommand WithDefault(string value) {
            DefaultValue = value;
            return this;
        }

        public CreateColumnCommand AddValidValue(string key, string value) {
            ValidValues.Add(key, value);
            return this;
        }
    }
}
