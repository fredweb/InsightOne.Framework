//   insightOne.SB1.Framework.Framework
// ****************************************************************************
// 
//   Copyright 2015 (c) Provider
//   Autor:     George Santos
// 
// ****************************************************************************

using insightOne.SB1.Framework.Forms;
using insightOne.SB1.Framework.Extensions;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace insightOne.SB1.Framework.BusinessOne
{
    /// <summary>
    /// Gerencia a árvore de permissão dentro do SAP
    /// </summary>
    public class B1PermissionManager
    {
        private readonly B1Application _app;
        private readonly Company _diCompany;

        public B1Application App { get { return _app; } }

        public Company DICompany { get { return _diCompany; } }

        public B1PermissionManager(B1Application app) {
            Check.IsNotNull("app", app);
            _app = app;
            _diCompany = _app.SBOCompany;
        }

        /// <summary>
        /// Cria uma permissão na árvore de permissões do SAP
        /// </summary>
        /// <param name="parentID">ID do nó pai onde a permissão será criada</param>
        /// <param name="permissionID">ID da permissão</param>
        /// <param name="name">Título da permissão exibida pelo SAP</param>
        /// <param name="opts">Opções de acesso <seealso cref="BoUPTOptions"/></param>
        /// <returns>Retorna o código do erro ao criar a permissão ou 0 quando criado com sucesso.</returns>
        public int CreatePermission(string parentID, string permissionID, string name, BoUPTOptions opts) {
            int result = 0;
            var upt = _app.BusinessObjects.GetBusinessObject<IUserPermissionTree>(BoObjectTypes.oUserPermissionTree);

            try {
                if (!string.IsNullOrEmpty(parentID))
                    upt.ParentID = parentID;
                upt.PermissionID = permissionID;
                upt.Name = name;
                upt.Options = opts;
                result = upt.Add();
            }
            finally {
                _app.BusinessObjects.ReleaseObject(upt);
            }
            return result;
        }

        /// <summary>
        /// Cria uma permissão na árvore de permissões do SAP
        /// </summary>
        /// <param name="permissionID">ID da permissão</param>
        /// <param name="name">Título da permissão exibida pelo SAP</param>
        /// <param name="opts">Opções de acesso <seealso cref="BoUPTOptions"/></param>
        /// <returns>Retorna o código do erro ao criar a permissão ou 0 quando criado com sucesso.</returns>
        public int CreatePermission(string permissionID, string name, BoUPTOptions opts) {
            return CreatePermission(null, permissionID, name, opts);
        }

        /// <summary>
        /// Obtem a permissão do usuário através do código do usuário
        /// </summary>
        /// <param name="userSign">Código do usuário (USERID)</param>
        /// <param name="permissionID">Código da permissão</param>
        /// <returns>F - Total, Y - Permitido, N - Negado</returns>
        public string GetPermission(int userSign, string permissionID) {
            string result = "N";
            var rs = App.BusinessObjects.GetRecordset();
            try {
                rs.DoQuery(string.Format("SELECT SUPERUSER  FROM [OUSR] WHERE USERID = {0}", userSign));
                bool isSuper = (!rs.EoF && rs.GetValueAsString("SUPERUSER") == "Y");

                if (isSuper) {
                    result = "F";
                }
                else {
                    rs.DoQuery(
                        string.Format(
                            "SELECT Permission FROM [USR3] WHERE UserLink = {0} AND PermId = '{1}'", 
                            userSign, 
                            permissionID));
                    result = rs.EoF ? "N" : rs.GetValueAsString("Permission");
                }
            }
            finally {
                rs.ReleaseComObject();
            }

            return result;
        }

        /// <summary>
        /// Obtem a permissão do usuário atual logado na empresa
        /// </summary>
        /// <param name="permissionID">Código da permissão</param>
        /// <returns>F - Total, Y - Permitido, N - Negado</returns>
        public string GetPermission(string permissionID) {
            return GetPermission(App.SBOCompany.UserSignature, permissionID);
        }

        public bool HasPermission(int userSign, string permissionID) {
            var perm = GetPermission(userSign, permissionID);
            return (perm == "F" || perm == "Y");
        }

        public bool HasPermission(string permissionID) {
            return HasPermission(App.SBOCompany.UserSignature, permissionID);
        }
    }
}
