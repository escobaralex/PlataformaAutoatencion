using Api.DBUtils;
using Api.POCOS;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Api.Models.bfc
{
    public class EmpresaModels
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<Empresa> GetEmpresas()
        {
            
            List<Empresa> result = null;
            Empresa empresa = null;
            DataSet dsEmpresas = null;
            SqlUtils su = new SqlUtils();
            dsEmpresas = su.GetEmpresas();
            if (DataSetUtils.hasRows(dsEmpresas))
            {
                result = new List<Empresa>();
                
                foreach (DataRow dr in dsEmpresas.Tables[0].Rows)
                {
                    empresa = new Empresa();
                    
                    try
                    {
                        empresa.Id = Convert.ToInt32(dr["EM_CODIGO"]);
                        empresa.Nombre = dr["EM_NOMBRE"].ToString();
                        empresa.AnoAct = Convert.ToInt32(dr["EM_ANOACT"]);
                        empresa.MesAct = Convert.ToInt32(dr["EM_MESACT"]);
                        result.Add(empresa);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            return result;
        }

        internal object GetFormasCobro()
        {
            throw new NotImplementedException();
        }
    }
}