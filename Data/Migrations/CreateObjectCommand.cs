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
    public class CreateObjectCommand
    {
        private Dictionary<string, string> _findFields;
        private List<string> _childTables;

        public CreateObjectCommand(string tableName, string name, string description) {
            _findFields = new Dictionary<string, string>();
            _childTables = new List<string>();
            TableName = tableName;
            Name = name;
            Description = description;
            CanFind = false;
            CanCancel = false;
            CanClose = false;
            CanDelete = false;
            CanArchive = false;
            ManageSeries = false;
            CanYearTransfor = false;
            CanLog = false;
            UniqueFormType = false;
        }

        public Dictionary<string, string> FindFields {
            get { return _findFields; }
        }

        public List<string> ChildTables {
            get { return _childTables; }
        }

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public string TableName { get; protected set; }

        public BoUDOObjType ObjectType { get; protected set; }

        public bool CanFind { get; protected set; }

        public bool CanCancel { get; protected set; }

        public bool CanClose { get; protected set; }

        public bool CanDelete { get; protected set; }

        public bool CanYearTransfor { get; protected set; }

        public bool CanLog { get; protected set; }

        public bool CanArchive { get; protected set; }

        public bool ManageSeries { get; protected set; }

        public bool UniqueFormType { get; protected set; }

        public CreateObjectCommand HasLog() {
            CanLog = true;
            return this;
        }

        public CreateObjectCommand NotLog() {
            CanLog = false;
            return this;
        }

        public CreateObjectCommand HasFind() {
            CanFind = true;
            return this;
        }

        public CreateObjectCommand NotFind() {
            CanFind = false;
            return this;
        }

        public CreateObjectCommand HasCancel() {
            CanCancel = true;
            return this;
        }

        public CreateObjectCommand NotCancel() {
            CanCancel = false;
            return this;
        }

        public CreateObjectCommand HasClose() {
            CanClose = true;
            return this;
        }

        public CreateObjectCommand NotClose() {
            CanClose = false;
            return this;
        }

        public CreateObjectCommand HasDelete() {
            CanDelete = true;
            return this;
        }

        public CreateObjectCommand NotDelete() {
            CanDelete = false;
            return this;
        }

        public CreateObjectCommand HasManageSeries() {
            ManageSeries = true;
            return this;
        }

        public CreateObjectCommand UseUniqueFormType() {
            UniqueFormType = true;
            return this;
        }

        public CreateObjectCommand NotUseUniqueFormType() {
            UniqueFormType = false;
            return this;
        }

        public CreateObjectCommand NotManageSeries() {
            ManageSeries = false;
            return this;
        }

        public CreateObjectCommand WithType(BoUDOObjType obType) {
            ObjectType = obType;
            return this;
        }

        public CreateObjectCommand AddFind(string fieldName) {
            FindFields.Add(fieldName, "");
            return this;
        }

        public CreateObjectCommand AddFind(string fieldName, string fieldDescription) {
            FindFields.Add(fieldName, fieldDescription);
            return this;
        }

        public CreateObjectCommand AddChild(string tableName) {
            ChildTables.Add(tableName);
            return this;
        }
    }
}
