using System;
using System.Collections.Generic;
using Api.POCOS;
using System.Data;
using Api.DBUtils;
using log4net;

namespace Api.Models.bfc
{
    internal class RolModels
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public RolModels()
        {
        }

        
        public List<Rol> GetRoles()
        {
            List<Rol> result = null;
            Rol rol = null;
            DataSet dsRoles = null;
            SqlUtils su = new SqlUtils();
            dsRoles = su.GetRoles();
            if (DataSetUtils.hasRows(dsRoles))
            {
                result = new List<Rol>();

                foreach (DataRow dr in dsRoles.Tables[0].Rows)
                {
                    rol = new Rol();

                    try
                    {
                        rol.Id = dr["Id"].ToString();
                        rol.Name = dr["NAME"].ToString();
                        result.Add(rol);
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