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
    public class TableCommand
    {
        private List<CreateColumnCommand> _tableCommands;

        private Dictionary<string, CreateIndexCommand> _indexCommands;

        public List<CreateColumnCommand> TableCommands {
            get { return _tableCommands; }
        }

        public Dictionary<string, CreateIndexCommand> IndexCommands {
            get { return _indexCommands; }
        }

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        public BoUTBTableType TableType { get; protected set; }

        public TableCommand(string name, string description) {
            _tableCommands = new List<CreateColumnCommand>();
            _indexCommands = new Dictionary<string, CreateIndexCommand>();
            Name = name;
            Description = description;
            TableType = BoUTBTableType.bott_NoObject;
        }
    }
}
