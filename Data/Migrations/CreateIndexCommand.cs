using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework.Data.Migrations
{
    public class CreateIndexCommand
    {
        private List<string> _fieldsName;

        public string TableName { get; protected set; }
        public bool IsUnique { get; protected set; }
        public string IndexName { get; protected set; }
        public List<string> FieldsName {
            get {
                return _fieldsName;
            }
        }

        public CreateIndexCommand(string tableName, string name) {
            _fieldsName = new List<string>();
            this.TableName = tableName;
            this.IndexName = name;
            IsUnique = false;
        }

        public void AddField(string fieldName) {
            _fieldsName.Add(fieldName);
        }

        public void Unique() {
            IsUnique = true;
        }
    }
}
