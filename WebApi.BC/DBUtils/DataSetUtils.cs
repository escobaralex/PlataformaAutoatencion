using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WebApi_BFC.DBUtils
{
    public class DataSetUtils
    {
        public static bool hasRows(DataSet ds)
        {
            bool result = false;
            try
            {
                DataRow dr = ds.Tables[0].Rows[0];
                result = true;
            }
            catch 
            {
            }
            return result;
        }
    }
}