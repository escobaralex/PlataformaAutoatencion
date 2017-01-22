using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api
{
    public static class Connection
    {
        static string conn = string.Empty;
        static string conn2 = string.Empty;

        /// <summary>
        /// Obtiene el string de conex. del portal de BevfoodCenter
        /// </summary>
        /// <returns></returns>
        public static string GetConnection()
        {
            conn = System.Configuration.ConfigurationManager.ConnectionStrings["BevfoodCenter"].ConnectionString;
            return conn;
        }
        /// <summary>
        /// Obtiene el string de conex. de la BD nucleo de Sysgestion
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionNucleo()
        {
            conn2 = System.Configuration.ConfigurationManager.ConnectionStrings["NUCLEO"].ConnectionString;
            return conn2;
        }
        /// <summary>
        /// Obtiene la conexión de la empresa segun identificador de empresa
        /// </summary>
        /// <param name="sConn"></param>
        /// <param name="IdEmpresa"></param>
        /// <returns></returns>
        public static string GetConnection(string sConn, int IdEmpresa)
        {
            string r = string.Format(sConn, AgregaCeroIdEmpresa(IdEmpresa));
            return r;
        }
        /// <summary>
        /// Agrega un cero si el codigo de empresa es menor a 10 para apuntar correctamente a la BD de Sysgestión
        /// </summary>
        /// <param name="IdEmpresa"></param>
        /// <returns></returns>
        public static string AgregaCeroIdEmpresa(int IdEmpresa)
        {
            string result = string.Empty;
            if (IdEmpresa < 10)
            {
                result = "0" + IdEmpresa.ToString();
            }
            else
            {
                result = IdEmpresa.ToString();
            }
            return result;
        }
    }
}