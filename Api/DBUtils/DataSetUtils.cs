using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Api.DBUtils
{
    public class DataSetUtils
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static bool hasRows(DataSet ds)
        {
            bool result = false;
            try
            {
                DataRow dr = ds.Tables[0].Rows[0];
                result = true;
            }
            catch (Exception ex)
            {
                //Log.Debug("El DataSet no contiene registros",ex);
            }
            return result;
        }
        public static bool IsEmpty(DataSet ds)
        {
            bool result = true;
            try
            {
                if (ds.Tables[0] !=null)
                {
                    result = false;
                }
            }
            catch (Exception)
            {                
            }
            return result;
        }
    }
}