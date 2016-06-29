//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using insightOne.SB1.Framework.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insightOne.SB1.Framework.Data.Migrations
{
    public abstract class B1Migration : IDataMigration
    {
        private B1Application _app;
        private const string MigrationTableName = "PRV_MGRT";
        private const string MigrationTableDesription = "Configurações de migração";

        private IDataMigrationInterpreter _interpreter;
        public SchemaBuilder SchemaBuilder { get; set; }

        //TODO: Melhorar a inicialização do VersionID pois a pessoa pode esquecer na sua classe base
        /// <summary>
        /// ID de versão do addon que será executado a migração
        /// </summary>
        public string VersionID { get; set; }

        public B1Application App {
            get { return _app; }
            set { _app = value; }
        }

        public B1Migration() {
            _app = B1Application.Current;
            _interpreter = new SBOCommandInterpreter();
            SchemaBuilder = new SchemaBuilder(_interpreter);
        }

        private void CreateMigrationTableIfNotExists() {
            if (!SchemaBuilder.TableExists(MigrationTableName)) {
                SchemaBuilder.CreateTable(
                    MigrationTableName, 
                    MigrationTableDesription,
                    table=>table
                        .Column("Version", "Version number", col=>col
                            .WithType(SAPbobsCOM.BoFieldTypes.db_Numeric)
                            .WithDefault("0")
                            .NotNull()
                            )
                        );
            }
        }

        protected void SetVersion(string versionId, int versionNumber) {            
            var table = _app.SBOCompany.UserTables.Item(MigrationTableName);
            try {
                int exec = 0;
                if (table.GetByKey(versionId)) {
                    table.UserFields.Fields.Item("U_Version").Value = versionNumber;
                    exec = table.Update();
                } else {
                    table.Code = versionId;
                    table.Name = versionId;
                    table.UserFields.Fields.Item("U_Version").Value = versionNumber;
                    exec = table.Add();
                }
                if (exec != 0) {
                    string error = _app.SBOCompany.GetLastErrorDescription();
                    throw new B1CoreException(string.Format("Error: {0}", error));
                }
            }
            finally {
                _app.BusinessObjects.ReleaseObject(table);
            }
        }

        protected int GetVersion(string versionId) {
            int ver = 0;
            var table = _app.SBOCompany.UserTables.Item(MigrationTableName);
            try {
                if (table.GetByKey(versionId)) {
                    ver = Convert.ToInt32(table.UserFields.Fields.Item("U_Version").Value);
                }                
            }
            finally {
                _app.BusinessObjects.ReleaseObject(table);
            }
            return ver;
        }

        public void CreateOrUpgrade() {
            CreateMigrationTableIfNotExists();
            var version = GetVersion(VersionID);
            var newVersion = Create(version);
            if (version != newVersion) {
                SetVersion(VersionID, newVersion);
            }
        }

        protected virtual int Create(int version) {
            return 0;
        }
    }
}
