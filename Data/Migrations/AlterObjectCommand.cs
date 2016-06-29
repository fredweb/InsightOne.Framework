using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework.Data.Migrations
{
    public class AlterObjectCommand
    {
        private Dictionary<string, string> _findFields;
        private List<string> _childTables;

        public AlterObjectCommand(string objectName)
        {
            Name = objectName;
            _findFields = new Dictionary<string, string>();
            _childTables = new List<string>();
        }

        public Dictionary<string, string> FindFields
        {
            get { return _findFields; }
        }

        public List<string> ChildTables
        {
            get { return _childTables; }
        }

        public string Name { get; protected set; }

        public AlterObjectCommand AddFind(string fieldName)
        {
            FindFields.Add(fieldName, "");
            return this;
        }

        public AlterObjectCommand AddFind(string fieldName, string fieldDescription)
        {
            FindFields.Add(fieldName, fieldDescription);
            return this;
        }

        public AlterObjectCommand AddChild(string tableName)
        {
            ChildTables.Add(tableName);
            return this;
        }
    }
}
