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
    public class FormaCobroModels
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<FormaCobro> GetFormaCobro()
        {
            List<FormaCobro> result = null;
            FormaCobro formaCobro = null;
            DataSet dsFormaCobro = null;
            SqlUtils su = new SqlUtils();
            dsFormaCobro = su.GetFormasCobroXml();
            if (DataSetUtils.hasRows(dsFormaCobro))
            {
                result = new List<FormaCobro>();

                foreach (DataRow dr in dsFormaCobro.Tables[0].Rows)
                {
                    formaCobro = new FormaCobro();

                    try
                    {
                        formaCobro.Id = Convert.ToInt32(dr["ID"]);
                        formaCobro.Nombre = dr["NOMBRE"].ToString();                        
                        result.Add(formaCobro);
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