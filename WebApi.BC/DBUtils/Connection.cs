using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_BFC
{
    public static class Connection
    {
        static string conn = string.Empty;
        public static string GetConnection()
        {
            conn = System.Configuration.ConfigurationManager.ConnectionStrings["SQL"].ConnectionString;
            return conn;
        }
    }
}