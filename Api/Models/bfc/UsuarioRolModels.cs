using Api.DBUtils;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;

namespace Api.Models.bfc
{
    public class UsuarioRolModels
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public UsuarioRolModels()
        {
        }
               
        public List<UsuarioRol> GetUsuarioRol()
        {
            List<UsuarioRol> result = null;
            UsuarioRol usuarioRol = null;
            DataSet dsEmpresas = null;
            SqlUtils su = new SqlUtils();
            dsEmpresas = su.GetUsuarioRol();
            if (DataSetUtils.hasRows(dsEmpresas))
            {
                result = new List<UsuarioRol>();

                foreach (DataRow dr in dsEmpresas.Tables[0].Rows)
                {
                    usuarioRol = new UsuarioRol();

                    try
                    {
                        usuarioRol.IdUsuario = dr["UserId"].ToString();
                        usuarioRol.IdRol = dr["RoleId"].ToString();
                        result.Add(usuarioRol);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return result;
        }
    }
}