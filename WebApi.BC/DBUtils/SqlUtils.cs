using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApi_BFC.DBUtils
{
    public class SqlUtils
    {
        private QueryXML queryXML = QueryXML.Instance;
        
        public DataSet GetDataSqlToDataSet(string queryString, string connectionString)
        {
            DataSet dsResult = new DataSet();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(queryString, connectionString);
                adapter.Fill(dsResult);
            }
            catch (SqlException)
            {

            }
            return dsResult;
        }
        public DataTable GetDataTableFromDataSet(DataSet ds)
        {
            DataTable dtResult = new DataTable();
            dtResult = ds.Tables[0];
            return dtResult;
        }
        public bool BeginTransaction(string query, string connection)
        {
            bool bResult = true;
            //Instaciamos la conexion
            SqlConnection cn = new SqlConnection(connection);
            //Abrimos conexion
            cn.Open();
            //Definimos que es una transaccion
            SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
            //Comando que ejecuta el insert en la tb_pedido
            SqlCommand cmd = new SqlCommand(query, cn, tr);

            try
            {
                //Ejecuto
                cmd.ExecuteNonQuery();
                tr.Commit(); //Actualizar bd
                //msg = "Pedido solicitado con éxito";
            }
            catch (Exception)
            {
                //De haber un error lo capturo
                //msg = ex.Message;
                //Deshacemos la operacion
                tr.Rollback();
                bResult = false;
            }
            finally
            {
                cn.Close(); //Cerramos la conexion
            }
            return bResult;
        }
        public DataSet GetXmlQueryToDataSet(string queryString)
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            try
            {
                sQry = queryXML.Doc.DocumentElement.SelectSingleNode(queryString).InnerText;
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection());
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (SqlException)
            {
                dsResult = null;
            }
            return dsResult;
        }

        public DataSet GetXmlQueryToDataSet(string queryString, string[] param)
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            try
            {
                sQry = string.Format(queryXML.Doc.DocumentElement.SelectSingleNode(queryString).InnerText, param);
                
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection());
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (SqlException)
            {
                dsResult = null;
            }
            return dsResult;
        }
    }
}