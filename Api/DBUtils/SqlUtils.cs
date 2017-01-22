using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Api.Controllers.bfc;
using Api.POCOS;
using Api.Pocos;
using log4net;

namespace Api.DBUtils
{
    public class SqlUtils
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private QueryXML queryXML = QueryXML.Instance;
        
        public DataSet GetDataSqlToDataSet(string queryString, string connectionString)
        {
            DataSet dsResult = new DataSet();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(queryString, connectionString);
                adapter.Fill(dsResult);
            }
            catch (SqlException ex)
            {
                Log.Error("Error GetDataSqlToDataSet, query: " + queryString, ex);
            }
            return dsResult;
        }

        internal DataSet GetMovimientoExistencia(Encabezado encabezado, DataSet dsReferencias, int idEmpresa)
        {
            DataSet result = null;
            string sQry = " SELECT TOP 20 EE_TIPDOC,EE_NUMDOC,EE_FECEMI,EE_CODBOD,EE_OBSERV FROM ENCEXI WHERE 1=1 ";
            
            if (DataSetUtils.hasRows(dsReferencias))
            {
                sQry += " AND EE_TIPDOC = " + dsReferencias.Tables[0].Rows[0]["EF_TIPDOC"].ToString();
                sQry += " AND EE_NUMDOC = " + dsReferencias.Tables[0].Rows[0]["EE_NUMDOC"].ToString();
            }
            else
            {               
                if (encabezado.NroDocumento > 0)
                {
                    sQry += " AND EE_NUMDOC = " + encabezado.NroDocumento;
                }
                if (encabezado.Movimiento != 0)
                {
                    if (encabezado.Movimiento == 1)
                    {
                        sQry += " AND EE_TIPDOC = " + 38;
                    }
                    else if (encabezado.Movimiento == 2)
                    {
                        sQry += " AND EE_TIPDOC = " + 42;
                    }
                }
                if (encabezado.FechaEmision != null && encabezado.Observacion != string.Empty)
                {
                    sQry += " AND EE_FECEMI >= '" + String.Format("{0:yyyyMMdd}", encabezado.FechaEmision)
                        + "' AND EE_FECEMI <= '" + encabezado.Observacion.Substring(0, 10).Replace("-","") + " 23:59:59'";
                }
                if (encabezado.Bodega != null && encabezado.Bodega.Codigo > 0)
                {
                    sQry += " AND EE_CODBOD = " + encabezado.Bodega.Codigo;
                }
            }
            
            SqlUtils SqlUtils = new SqlUtils();
            result = GetDataSqlToDataSet(sQry, Connection.GetConnection(SqlUtils.queryXML.Doc.DocumentElement.SelectSingleNode("stringConnection").InnerText, idEmpresa));

            return result;
        }

        internal DataSet GetDetalleMovimientoExistencia(Encabezado encabezado, int idEmpresa)
        {
            DataSet result = null;
            string sQry = "SELECT LE_TIPDOC,LE_NUMDOC,LE_CRRLIN,LE_TIPMOV,LE_CODART,LE_CODBOD,LE_CANART,LE_VALUNI,AR_DESART FROM LINEXI,MAEART ";
            sQry += " WHERE LE_CODART = AR_CODART "; 
            sQry += " AND LE_NUMDOC = " + encabezado.NroDocumento;
            
            if (encabezado.Movimiento == 1)
            {
                sQry += " AND LE_TIPDOC = " + 38;
            }
            else if (encabezado.Movimiento == 2)
            {
                sQry += " AND LE_TIPDOC = " + 42;
            }
            sQry += " ORDER BY LE_CRRLIN";
            result = GetDataSqlToDataSet(sQry, Connection.GetConnection(queryXML.Doc.DocumentElement.SelectSingleNode("stringConnection").InnerText, idEmpresa));

            return result;
        }

        internal DataSet GetStockInicial(string codigo, string bodega, int anno, int idEmpresa)
        {
            DataSet result = null;
            string squery = string.Empty;
            
            if (bodega != null && bodega != string.Empty && bodega != "0")
            {
                squery += "SELECT sd1_codart, sd1_entrad, sd1_salida, sd1_codbod, SALDO = (sd1_entrad - sd1_salida) from( ";
                squery += "SELECT MAEART.AR_CODART AS sd1_codart, MAETAB.tb_codtab AS sd1_codbod, ";

                squery += "ISNULL((SELECT SUM(LINEXI.LE_CANART) AS Expr1 ";
                squery += "FROM ENCEXI INNER JOIN LINEXI ON ENCEXI.EE_TIPDOC = LINEXI.LE_TIPDOC AND ENCEXI.EE_NUMDOC = LINEXI.LE_NUMDOC ";
                squery += " WHERE(LINEXI.LE_TIPMOV = 'E') AND(LINEXI.LE_CODART = MAEART.AR_CODART) AND(LINEXI.LE_CODBOD = {0}) AND(LINEXI.LE_CODBOD = MAETAB.tb_codtab) AND(ENCEXI.EE_FECEMI < '{1}0101')), 0) AS sd1_entrad, ";

                squery += "ISNULL ((SELECT SUM(LINEXI.LE_CANART) AS Expr1 ";
                squery += "FROM ENCEXI INNER JOIN LINEXI ON ENCEXI.EE_TIPDOC = LINEXI.LE_TIPDOC AND ENCEXI.EE_NUMDOC = LINEXI.LE_NUMDOC ";
                squery += "WHERE(LINEXI.LE_TIPMOV = 'S') AND(LINEXI.LE_CODART = MAEART.AR_CODART) AND(LINEXI.LE_CODBOD = {0}) AND(LINEXI.LE_CODBOD = MAETAB.tb_codtab) AND(ENCEXI.EE_FECEMI < '{1}0101')), 0) AS sd1_salida ";

                squery += "FROM LINEXI INNER JOIN MAEART ON LINEXI.LE_CODART = MAEART.AR_CODART INNER JOIN  MAETAB ON LINEXI.LE_CODBOD = MAETAB.tb_codtab ";
                squery += "WHERE(MAETAB.tb_tiptab = 18) ";
                squery += "AND MAEART.AR_CODART = '{2}' ";
                squery += "AND MAETAB.tb_codtab = {0} ";
                squery += "GROUP BY MAEART.AR_CODART, MAETAB.tb_codtab) AS SALDOS ";
                squery = string.Format(squery, bodega, anno,codigo);
            }
            else
            {
                squery += "SELECT sd1_codart,sum(sd1_entrad) ENTRADA,suM(sd1_salida) SALIDA from ( ";
                squery += "SELECT MAEART.AR_CODART AS sd1_codart, MAETAB.tb_codtab AS sd1_codbod, ";
                squery += "ISNULL((SELECT SUM(LINEXI.LE_CANART) AS Expr1  ";
                squery += "FROM ENCEXI INNER JOIN LINEXI ON ENCEXI.EE_TIPDOC = LINEXI.LE_TIPDOC AND ENCEXI.EE_NUMDOC = LINEXI.LE_NUMDOC ";
                squery += " WHERE(LINEXI.LE_TIPMOV = 'E') AND(LINEXI.LE_CODART = MAEART.AR_CODART) AND(LINEXI.LE_CODBOD = MAETAB.tb_codtab) AND(ENCEXI.EE_FECEMI < '{1}0101')), 0) AS sd1_entrad, ";

                squery += "ISNULL ((SELECT SUM(LINEXI.LE_CANART) AS Expr1 ";

                squery += "FROM ENCEXI INNER JOIN LINEXI ON ENCEXI.EE_TIPDOC = LINEXI.LE_TIPDOC AND ENCEXI.EE_NUMDOC = LINEXI.LE_NUMDOC ";

                squery += "WHERE(LINEXI.LE_TIPMOV = 'S') AND(LINEXI.LE_CODART = MAEART.AR_CODART) AND(LINEXI.LE_CODBOD = MAETAB.tb_codtab) AND(ENCEXI.EE_FECEMI < '{1}0101')), 0) AS sd1_salida ";

                squery += "FROM LINEXI INNER JOIN MAEART ON LINEXI.LE_CODART = MAEART.AR_CODART INNER JOIN  MAETAB ON LINEXI.LE_CODBOD = MAETAB.tb_codtab ";
                squery += "WHERE(MAETAB.tb_tiptab = 18) ";
                squery += "AND MAEART.AR_CODART = '{0}' ";
                squery += "GROUP BY MAEART.AR_CODART, MAETAB.tb_codtab) AS SALDOS ";
                squery += "GROUP BY sd1_codart ";
                squery = string.Format(squery, codigo, anno);
            }
            result = this.GetDataSqlToDataSet(squery, Connection.GetConnection(queryXML.Doc.DocumentElement.SelectSingleNode("stringConnection").InnerText, idEmpresa));

            return result;
        }

        internal DataSet GetListadoEntradasSalidas(string codigo, DateTime desde, DateTime hasta, int idEmpresa)
        {
            DataSet result = null;
            string qry = string.Empty;

            // Primero Obtiene los saldos
            qry += " SELECT  EE_TIPDOC, EE_NUMDOC,EE_FECEMI, EE_CODBOD, LE_CODART, AR_DESART, LE_CODBOD,LE_CANART , SALDO = ENTRADA-SALIDA, DESCRIPCION = CONCAT(LE_CODART, ' - ' , AR_DESART) FROM ( ";
            qry += " SELECT sd1_codart AS CODIGO, sum(sd1_entrad)ENTRADA, suM(sd1_salida) SALIDA ";
            qry += " from(SELECT MAEART.AR_CODART AS sd1_codart, MAETAB.tb_codtab AS sd1_codbod, ";
            qry += " ISNULL((SELECT SUM(LINEXI.LE_CANART) AS Expr1 ";
            qry += " FROM ENCEXI INNER JOIN LINEXI ";
            qry += " ON ENCEXI.EE_TIPDOC = LINEXI.LE_TIPDOC AND ENCEXI.EE_NUMDOC = LINEXI.LE_NUMDOC ";
            qry += " WHERE(LINEXI.LE_TIPMOV = 'E') AND(LINEXI.LE_CODART = MAEART.AR_CODART) ";
            qry += " AND(LINEXI.LE_CODBOD = MAETAB.tb_codtab) AND(ENCEXI.EE_FECEMI < '{0}')), 0) ";
            qry += " AS sd1_entrad, ISNULL((SELECT SUM(LINEXI.LE_CANART) AS Expr1 ";
            qry += " FROM ENCEXI INNER JOIN LINEXI ON ENCEXI.EE_TIPDOC = LINEXI.LE_TIPDOC ";
            qry += " AND ENCEXI.EE_NUMDOC = LINEXI.LE_NUMDOC WHERE(LINEXI.LE_TIPMOV = 'S') ";
            qry += " AND(LINEXI.LE_CODART = MAEART.AR_CODART) AND(LINEXI.LE_CODBOD = MAETAB.tb_codtab) ";
            qry += " AND(ENCEXI.EE_FECEMI < '{0}')), 0) AS sd1_salida ";
            qry += " FROM LINEXI ";
            qry += " INNER JOIN MAEART ON LINEXI.LE_CODART = MAEART.AR_CODART ";
            qry += " INNER JOIN  MAETAB ON LINEXI.LE_CODBOD = MAETAB.tb_codtab ";
            qry += " WHERE(MAETAB.tb_tiptab = 18) ";
            qry += " GROUP BY MAEART.AR_CODART, MAETAB.tb_codtab) AS SALDOS GROUP BY sd1_codart ) STOCK_INICIAL JOIN";
            qry = string.Format(qry, desde.AddDays(-1).ToString("yyyyMMdd"), codigo);

            qry += " (SELECT EE_TIPDOC, EE_NUMDOC,EE_FECEMI, EE_CODBOD, LE_CODART, AR_DESART, LE_CODBOD,LE_CANART ";
            qry += "  FROM ENCEXI  JOIN LINEXI ON EE_TIPDOC = LE_TIPDOC AND EE_NUMDOC = LE_NUMDOC  JOIN MAEART ON LE_CODART = AR_CODART ";
            qry += " WHERE EE_FECEMI between '{0}' AND '{1} 23:59:59' ";
            qry += " ) AS DETALLES on STOCK_INICIAL.CODIGO = DETALLES.LE_CODART ";

            qry = string.Format(qry, desde.ToString("yyyyMMdd"), hasta.ToString("yyyyMMdd"));
            if (!string.IsNullOrEmpty(codigo))
            {
                qry = string.Format(qry +" WHERE DETALLES.LE_CODART = '{0}'",codigo);
            }
            qry += "  ORDER BY LE_CODART,EE_FECEMI ,EE_TIPDOC, EE_NUMDOC";

            result = this.GetDataSqlToDataSet(qry, Connection.GetConnection(queryXML.Doc.DocumentElement.SelectSingleNode("stringConnection").InnerText, idEmpresa));

            return result;
        }

        internal DataSet GetEntradasSalidas(string codigo, string bodega, int anno, int idEmpresa)
        {
            DataSet result = null;
            string squery = string.Empty;
            squery = "SELECT sd1_codart,";            
            if (bodega != null && bodega != string.Empty && bodega != "0")
            {
                squery += "sd1_codbod,";
            }
            squery += "sd1_anoper,sd1_mesper,sum(sd1_entrad) ENTRADAS,sum(sd1_salida) SALIDAS ";
            squery += "FROM VW_1040_STPR ";
            squery += "WHERE sd1_codart = '{0}' ";
            squery += "AND sd1_anoper = {1} ";
            if (bodega != null && bodega != string.Empty && bodega != "0")
            {
                squery += "AND sd1_codbod ={2} ";
            }
            squery += "GROUP BY sd1_codart,sd1_mesper,sd1_anoper ";
            if (bodega != null && bodega != string.Empty && bodega != "0")
            {
                squery += ",sd1_codbod ";
            }
            squery += "ORDER BY sd1_mesper";
            if (bodega != null && bodega != string.Empty && bodega != "0")
            {
                squery = string.Format(squery, codigo, anno, bodega);
            }
            else
            {
                squery = string.Format(squery, codigo, anno);
            }
            result = this.GetDataSqlToDataSet(squery, Connection.GetConnection(queryXML.Doc.DocumentElement.SelectSingleNode("stringConnection").InnerText, idEmpresa));

            return result;
        }

        internal DataSet GetDetallesEntradasSalidas(string codigo, string bodega, int anno, int mes, int idEmpresa)
        {
            DataSet result = new DataSet();
            string sQry = string.Empty;
            int daysInMonth = System.DateTime.DaysInMonth(anno, mes);
            sQry += " select EE_TIPDOC, EE_NUMDOC,EE_FECEMI, EE_CODBOD, LE_CODART, ";
            sQry += " LE_CODBOD,LE_CANART from ENCEXI, LINEXI ";
            sQry += " where EE_FECEMI between '{0}' AND '{1}' ";
            sQry += " AND EE_TIPDOC = LE_TIPDOC AND EE_NUMDOC = LE_NUMDOC ";
            sQry += " AND EE_TIPDOC IN (38, 42) ";// PARA CONSIDERAR SOLO LOS INGRESOS Y EGRESOS DESDE EL PORTAL
            sQry += " AND LE_CODART = '{2}' ";
            if (bodega != "0")
            {
                sQry += " AND LE_CODBOD = {3} ";
            }
            sQry += " ORDER BY EE_FECEMI ";
            if (bodega == "0")
            {
                sQry = string.Format(sQry, anno + ""+ (mes < 10 ? "0" + mes.ToString() : mes.ToString()) + "01", anno + "" + (mes < 10 ? "0" + mes.ToString() : mes.ToString()) + "" + daysInMonth + " 23:59:59", codigo);
            }
            else
            {
                sQry = string.Format(sQry, anno + "" + (mes < 10 ? "0" + mes.ToString() : mes.ToString()) + "01", anno + "" + (mes < 10 ? "0" + mes.ToString() : mes.ToString()) + "" + daysInMonth + " 23:59:59", codigo, bodega);
            }       
            

            result = GetDataSqlToDataSet(sQry, Connection.GetConnection(queryXML.Doc.SelectSingleNode("Query/stringConnection").InnerText, idEmpresa));

            return result;
        }

        internal DataSet GetReferenciasMovimientosExistencias(int movimiento, int nroDocumento, int idEmpresa)
        {
            DataSet result = new DataSet();
            string sQry = string.Empty;
            sQry += " select * from ReferenciaDocExistencia, documentoreferencia ";
            sQry += " WHERE ID_EMPRESA = " + idEmpresa + " AND ReferenciaDocExistencia.ID_DOCREF = DocumentoReferencia.Id ";
            if (movimiento == 1)
            {
                sQry += " AND EF_TIPDOC = 38 ";
            }
            else
            {
                sQry += " AND EF_TIPDOC = 42 ";
            }
            
            sQry += " AND EE_NUMDOC = " + nroDocumento;
            
            result = GetDataSqlToDataSet(sQry, Connection.GetConnection());

            return result;
        }

        internal DataSet GetReferenciasMovimientosExistencias(Encabezado encabezado, int idEmpresa)
        {
            DataSet result = new DataSet();
            string sQry = string.Empty;
            sQry += " SELECT EF_TIPDOC, EE_NUMDOC FROM ReferenciaDocExistencia ";
            sQry += " WHERE ID_EMPRESA = " + idEmpresa;
            if (encabezado.referencia != null && encabezado.referencia.Id > 0)
            {
                sQry += " AND ID_DOCREF = " + encabezado.referencia.Id;
            }
            if (encabezado.referencia != null && encabezado.nroReferencia > 0)
            {
                sQry += " AND NRO_DOCREF = " + encabezado.nroReferencia;
            }
            result = GetDataSqlToDataSet(sQry, Connection.GetConnection());
            
            return result;
        }

        internal bool DeleteMovimientoExistencia(MovimientoExistencia me, int idEmpresa)
        {
            bool result = false;
            int tipoDoc = me.Encabezado.Movimiento == 1 ? 38 : 42;
            string sQryRef = string.Empty;
            string sQryEnc = string.Empty;
            string sQryDet = string.Empty;
            // BORRAR REFERENCIAS
            sQryRef += " DELETE FROM ReferenciaDocExistencia WHERE 1 = 1 ";
            sQryRef += " AND EF_TIPDOC = {0} AND EE_NUMDOC = {1} AND ID_EMPRESA = {2}";
            sQryRef = string.Format(sQryRef, tipoDoc, me.Encabezado.NroDocumento, idEmpresa);
            // BORRAR ENCABEZADO
            sQryEnc = " DELETE FROM ENCEXI WHERE EE_TIPDOC = " + tipoDoc;
            sQryEnc += " AND EE_NUMDOC = " + me.Encabezado.NroDocumento;
            // BORRAR DETALLES
            sQryDet = " DELETE FROM LINEXI WHERE LE_TIPDOC = " + tipoDoc;
            sQryDet += " AND LE_NUMDOC = " + me.Encabezado.NroDocumento;
            // OBTENGO STRING DE CONEXION DE LA EMPRESA
            string sCon = Connection.GetConnection(queryXML.Doc.SelectSingleNode("Query/stringConnection").InnerText, idEmpresa);

            SqlConnection cnSG = new SqlConnection(sCon);
            SqlConnection cnWeb = new SqlConnection(Connection.GetConnection());

            cnWeb.Open();            
            SqlTransaction trWeb = cnWeb.BeginTransaction(IsolationLevel.Serializable);
            SqlCommand cmdWeb = new SqlCommand(sQryRef, cnWeb, trWeb);
            bool resultWeb = false;
            try
            {
                cmdWeb.ExecuteNonQuery();
                resultWeb = true;
            }
            catch (Exception ex)
            {
                trWeb.Rollback();
                cnWeb.Close();
                Log.Error("Error al eliminar movimiento de existencia en BD Web", ex);

            }
            if (resultWeb)
            {
                cnSG.Open();
                SqlTransaction trSG = cnSG.BeginTransaction(IsolationLevel.Serializable);
                SqlCommand cmdSGEnc = new SqlCommand(sQryEnc, cnSG, trSG);
                SqlCommand cmdSGDet = new SqlCommand(sQryDet, cnSG, trSG);
                try
                {
                    cmdSGEnc.ExecuteNonQuery();
                    cmdSGDet.ExecuteNonQuery();
                    result = true;
                    trSG.Commit();
                    trWeb.Commit();
                }
                catch (Exception ex)
                {
                    trWeb.Rollback();
                    trSG.Rollback();
                    cnWeb.Close();
                    cnSG.Close();
                    Log.Error("Error al eliminar movimiento de existencia BD SG " + sCon, ex);
                }
            }
            return result;
        }

        internal DataSet GetReferenciasMovimientosExistencias(List<int> ingresos, List<int> egresos, int idEmpresa)
        {
            DataSet result = null;
            string query = string.Empty;
            query += " SELECT * FROM ReferenciaDocExistencia,DocumentoReferencia ";
            query += " WHERE ID_EMPRESA = " + idEmpresa;
            query += " AND (EF_TIPDOC = 38 AND EE_NUMDOC IN ({0})  AND ReferenciaDocExistencia.ID_DOCREF = DocumentoReferencia.Id) ";
            query += " OR (EF_TIPDOC = 42 AND EE_NUMDOC IN ({1})  AND ReferenciaDocExistencia.ID_DOCREF = DocumentoReferencia.Id)  and ID_EMPRESA = " + idEmpresa;
            string sIngresos = string.Empty;
            for (int i = 0; i < ingresos.Count; i++)
            {
                sIngresos += ingresos[i];
                if (i+1 < ingresos.Count)
                {
                    sIngresos += ",";
                }
            }
            string sEgresos = string.Empty;
            for (int i = 0; i < egresos.Count; i++)
            {
                sEgresos += egresos[i];
                if (i + 1 < egresos.Count)
                {
                    sEgresos += ",";
                }
            }
            query = string.Format(query, (string.IsNullOrWhiteSpace(sIngresos) ? "0" : sIngresos) , (string.IsNullOrWhiteSpace(sEgresos) ? "0" : sEgresos));
            result = GetDataSqlToDataSet(query, Connection.GetConnection());

            return result;
        }

        public int CreateMovimientoExistencia(MovimientoExistencia movimientoExistencia, int idEmpresa)
        {
            int result = 0;
            string sQrySGEnc = string.Empty;
            string sQrySGDet = string.Empty;
            string sQryGetNroDoc = string.Empty;
            string sQryWebDocRef = string.Empty;
            string tipoDoc = string.Empty;
            #region Querys Sysgestion
            if (movimientoExistencia.Encabezado.Movimiento == 1)
            {
                tipoDoc += " 38"; // INGRESO
            }
            else
            {
                tipoDoc += " 42 "; // SALIDA
            }
            sQryGetNroDoc = " SELECT ISNULL(MAX(EE_NUMDOC),0) FROM ENCEXI WHERE EE_TIPDOC = " + tipoDoc;

            sQrySGEnc += " INSERT INTO ENCEXI ";
            sQrySGEnc += " (EE_TIPDOC ";
            sQrySGEnc += " ,EE_NUMDOC ";
            sQrySGEnc += " ,EE_FECEMI ";
            sQrySGEnc += " ,EE_OBSERV ";
            sQrySGEnc += " ,EE_FECDES ";
            sQrySGEnc += " ,EE_CODUSR ";
            sQrySGEnc += " ,EE_CENCOS ";
            sQrySGEnc += " ,EE_NUMFUN ";
            sQrySGEnc += " ,EE_CODBOD ";
            sQrySGEnc += " ,EE_NUMOPR ";
            sQrySGEnc += " ,EE_UNEGOC ";
            sQrySGEnc += " ,EE_CODLOC ";
            //--desde aca puros CEROS
            sQrySGEnc += " ,EE_CODSUP ";
            sQrySGEnc += " ,EE_CODMAQ ";
            sQrySGEnc += " ,EE_ETAPRD ";
            sQrySGEnc += " ,EE_PROYEC ";
            sQrySGEnc += " ,EE_NUMOTR ";
            sQrySGEnc += " ,EE_RUTCLI ";
            sQrySGEnc += " ,EE_FLAG01 ";
            sQrySGEnc += " ,EE_FLAG02 ";
            sQrySGEnc += " ,EE_FLAG03 ";
            sQrySGEnc += " ,EE_REAL01 ";
            sQrySGEnc += " ,EE_REAL02 ";
            sQrySGEnc += " ,EE_REAL03 ";
            //-- HASTA ACA PUROS CEROS
            //Desde aca puros vacios
            sQrySGEnc += " , EE_TEXT01 ";
            sQrySGEnc += " , EE_TEXT02 ";
            sQrySGEnc += " , EE_TEXT03) VALUES (";

            sQrySGEnc += tipoDoc + ",";//(EE_TIPDOC
            sQrySGEnc += "{0}, '";//,EE_NUMDOC  movimientoExistencia.Encabezado.NroDocumento +
            //sQrySGEnc += movimientoExistencia.Encabezado.FechaEmision + "', '";//,EE_FECEMI  2000-01-01 00:00:00
            sQrySGEnc += String.Format("{0:yyyyMMdd}", movimientoExistencia.Encabezado.FechaEmision) + "', '";//,EE_FECEMI  2000-01-01 00:00:00

            sQrySGEnc += movimientoExistencia.Encabezado.Observacion + "', ";//,EE_OBSERV
            sQrySGEnc += "'20000101', ";//,EE_FECDES
            sQrySGEnc += "'ADM', ";//,EE_CODUSR
            sQrySGEnc += "0, ";//,EE_CENCOS
            sQrySGEnc += "0, ";//,EE_NUMFUN
            sQrySGEnc += movimientoExistencia.Encabezado.Bodega.Codigo + ", ";//,EE_CODBOD
            sQrySGEnc += "0, ";//,EE_NUMOPR
            sQrySGEnc += "0, ";//,EE_UNEGOC
            sQrySGEnc += "1, ";//,EE_CODLOC
            //--desde aca puros CEROS
            sQrySGEnc += "0, ";//,EE_CODSUP
            sQrySGEnc += "0, ";//,EE_CODMAQ
            sQrySGEnc += "0, ";//,EE_ETAPRD
            sQrySGEnc += "0, ";//,EE_PROYEC
            sQrySGEnc += "0, ";//,EE_NUMOTR
            sQrySGEnc += "0, ";//,EE_RUTCLI
            sQrySGEnc += "0, ";//,EE_FLAG01
            sQrySGEnc += "0, ";//,EE_FLAG02
            sQrySGEnc += "0, ";//,EE_FLAG03
            sQrySGEnc += "0, ";//,EE_REAL01
            sQrySGEnc += "0, ";//,EE_REAL02
            sQrySGEnc += "0, ";//,EE_REAL03 
            //-- HASTA ACA PUROS CEROS
            //Desde aca puros vacios
            sQrySGEnc += " '',";//, EE_TEXT01
            sQrySGEnc += " '',";//, EE_TEXT02
            sQrySGEnc += " '')";//, EE_TEXT03)

            string tipoMov = string.Empty;
            if (movimientoExistencia.Encabezado.Movimiento == 1)
            {
                tipoMov = " 'E'";
            }
            else
            {
                tipoMov = " 'S'";
            }
                        
            sQrySGDet = GetQueryInsertLINFAC(movimientoExistencia);
            
            #endregion
            #region Query Web
            if (movimientoExistencia.Encabezado.referencia != null)
            {
                sQryWebDocRef += " INSERT INTO ReferenciaDocExistencia ";
                sQryWebDocRef += " (EF_TIPDOC ";
                sQryWebDocRef += " , EE_NUMDOC ";
                sQryWebDocRef += " , ID_DOCREF ";
                sQryWebDocRef += " , NRO_DOCREF ";
                sQryWebDocRef += " , ID_EMPRESA) ";
                sQryWebDocRef += " VALUES ( ";
                sQryWebDocRef += tipoDoc + " , ";
                sQryWebDocRef += " {0}, ";
                sQryWebDocRef += movimientoExistencia.Encabezado.referencia.Id + " , ";
                sQryWebDocRef += movimientoExistencia.Encabezado.nroReferencia + " , ";
                sQryWebDocRef += idEmpresa + " ) ";
            }
            #endregion
            sQrySGDet = sQrySGDet.Substring(0, sQrySGDet.Length - 1);
            string sCon = Connection.GetConnection(queryXML.Doc.SelectSingleNode("Query/stringConnection").InnerText, idEmpresa);

            SqlConnection cnSG = new SqlConnection(sCon);
            SqlConnection cnWeb = new SqlConnection(Connection.GetConnection());
                        
            cnSG.Open();

            int Folio = 0;
            try
            {
                DataSet dsFolio = GetDataSqlToDataSet(sQryGetNroDoc, sCon);
                if (DataSetUtils.hasRows(dsFolio))
                {
                    Folio = Convert.ToInt32(dsFolio.Tables[0].Rows[0][0]) + 1;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar obtener el folio del Documento a registrar", ex);
                cnSG.Close();
                return -1;
            }
            sQrySGDet = string.Format(sQrySGDet, Folio);
            sQrySGEnc = string.Format(sQrySGEnc, Folio);
            bool resultWeb = false;
            SqlTransaction trWeb = null;
            if (movimientoExistencia.Encabezado.referencia != null)
            {
                sQryWebDocRef = string.Format(sQryWebDocRef, Folio);
                cnWeb.Open();
                trWeb = cnWeb.BeginTransaction(IsolationLevel.Serializable);
                SqlCommand cmdWeb = new SqlCommand(sQryWebDocRef, cnWeb, trWeb);
                try
                {
                    cmdWeb.ExecuteNonQuery();
                    resultWeb = true;
                }
                catch (Exception ex)
                {
                    trWeb.Rollback();
                    cnWeb.Close();
                    Log.Error("Error al crear movimiento de existencia al insertar referencia del documento en BD Web", ex);
                }
            }
            else
            {
                resultWeb = true;
            }
            if (resultWeb)
            {
                SqlTransaction trSG = cnSG.BeginTransaction(IsolationLevel.Serializable);
                SqlCommand cmdSGDet = new SqlCommand(sQrySGDet, cnSG, trSG);
                SqlCommand cmdSGEnc = new SqlCommand(sQrySGEnc, cnSG, trSG);

                bool resultadoSG = false;
                try
                {
                    cmdSGEnc.ExecuteNonQuery();
                    cmdSGDet.ExecuteNonQuery();
                    trSG.Commit();
                    if (movimientoExistencia.Encabezado.referencia != null)
                    {
                        trWeb.Commit();
                    }
                    resultadoSG = true;
                    result = Folio;
                }
                catch (Exception ex)
                {
                    trSG.Rollback();
                    cnSG.Close();
                    cnWeb.Close();
                    Log.Error("Error al crear movimiento de existencia", ex);
                }
            }
            return result;
        }
        public string GetQueryInsertLINFAC(MovimientoExistencia movimientoExistencia)
        {
            string sQrySGDet = string.Empty;
            string tipoDoc = string.Empty;
            string tipoMov = string.Empty;
            if (movimientoExistencia.Encabezado.Movimiento == 1)
            {
                tipoDoc += " 38"; // INGRESO
                tipoMov = " 'E'";
            }
            else
            {
                tipoDoc += " 42 "; // SALIDA
                tipoMov = " 'S'";
            }
            int correlativo = 1;
            foreach (Detalle detalle in movimientoExistencia.Detalles)
            {
                sQrySGDet += " INSERT INTO LINEXI ";
                sQrySGDet += " (LE_TIPDOC ";
                sQrySGDet += " ,LE_NUMDOC ";
                sQrySGDet += " ,LE_CRRLIN ";
                sQrySGDet += " ,LE_TIPMOV ";
                sQrySGDet += " ,LE_CODART ";
                sQrySGDet += " ,LE_CODBOD ";
                sQrySGDet += " ,LE_CANART ";
                sQrySGDet += " ,LE_VALUNI ";
                sQrySGDet += " ,LE_COSPMP ";
                sQrySGDet += " ,LE_CENCOS ";
                sQrySGDet += " ,LE_TIPMON ";
                sQrySGDet += " ,LE_TIPCAM ";
                sQrySGDet += " ,LE_COSCOM ";
                sQrySGDet += " ,LE_COSREF ";
                sQrySGDet += " ,LE_ETAPRD ";
                sQrySGDet += " ,LE_ITMPRO ";
                sQrySGDet += " ,LE_DESTIN ";
                sQrySGDet += " ,LE_CANPED ";
                sQrySGDet += " ,LE_CODLOT ";
                sQrySGDet += " ,LE_DETLOT ";
                //sQrySGDet += " ,LE_FLAG01 ";
                //sQrySGDet += " ,LE_FLAG02 ";
                //sQrySGDet += " ,LE_FLAG03 ";
                //sQrySGDet += " ,LE_REAL01 ";
                //sQrySGDet += " ,LE_REAL02 ";
                //sQrySGDet += " ,LE_REAL03 ";
                //sQrySGDet += " ,LE_TEXT01 ";
                sQrySGDet += " ) VALUES (";
                sQrySGDet += tipoDoc + " ,"; //LE_TIPDOC ";
                sQrySGDet += " {0},"; //LE_NUMDOC ";
                sQrySGDet += correlativo + " ,"; //LE_CRRLIN ";

                sQrySGDet += tipoMov + " ,'"; //LE_TIPMOV ";
                sQrySGDet += detalle.Codigo + "',"; //LE_CODART ";
                sQrySGDet += movimientoExistencia.Encabezado.Bodega.Codigo + " ,"; //LE_CODBOD ";
                sQrySGDet += detalle.Cantidad + " ,"; //LE_CANART ";
                sQrySGDet += detalle.ValorUnitario + " ,"; //LE_VALUNI ";
                sQrySGDet += " 0,"; //LE_COSPMP ";
                sQrySGDet += " 0,"; //LE_CENCOS ";
                sQrySGDet += " 1,"; //LE_TIPMON ";
                sQrySGDet += " 1,"; //LE_TIPCAM ";
                sQrySGDet += " 0,"; //LE_COSCOM ";
                sQrySGDet += " 1,"; //LE_COSREF ";
                sQrySGDet += " 0,"; //LE_ETAPRD ";
                sQrySGDet += " 0,"; //LE_ITMPRO ";
                sQrySGDet += " 0,"; //LE_DESTIN ";
                sQrySGDet += " 0,"; //LE_CANPED ";
                sQrySGDet += " 0,"; //LE_CODLOT ";
                sQrySGDet += " ''"; //LE_DETLOT ";
                //sQrySGDet += " 0,"; //LE_FLAG01 ";
                //sQrySGDet += " 0,"; //LE_FLAG02 ";
                //sQrySGDet += " 0,"; //LE_FLAG03 ";
                //sQrySGDet += " 0,"; //LE_REAL01 ";
                //sQrySGDet += " 0,"; //LE_REAL02 ";
                //sQrySGDet += " 0,"; //LE_REAL03 ";
                //sQrySGDet += " ''); "; //LE_TEXT01) ;
                sQrySGDet += ");";
                correlativo++;
            }
            return sQrySGDet;
        }
        public string GetQueryInsertENCFAC(MovimientoExistencia movimientoExistencia)
        {
            string sQrySGDet = string.Empty;
            string tipoDoc = string.Empty;
            string tipoMov = string.Empty;
            if (movimientoExistencia.Encabezado.Movimiento == 1)
            {
                tipoDoc += " 38"; // INGRESO
                tipoMov = " 'E'";
            }
            else
            {
                tipoDoc += " 42 "; // SALIDA
                tipoMov = " 'S'";
            }
            int correlativo = 1;
            foreach (Detalle detalle in movimientoExistencia.Detalles)
            {
                sQrySGDet += " INSERT INTO LINEXI ";
                sQrySGDet += " (LE_TIPDOC ";
                sQrySGDet += " ,LE_NUMDOC ";
                sQrySGDet += " ,LE_CRRLIN ";
                sQrySGDet += " ,LE_TIPMOV ";
                sQrySGDet += " ,LE_CODART ";
                sQrySGDet += " ,LE_CODBOD ";
                sQrySGDet += " ,LE_CANART ";
                sQrySGDet += " ,LE_VALUNI ";
                sQrySGDet += " ,LE_COSPMP ";
                sQrySGDet += " ,LE_CENCOS ";
                sQrySGDet += " ,LE_TIPMON ";
                sQrySGDet += " ,LE_TIPCAM ";
                sQrySGDet += " ,LE_COSCOM ";
                sQrySGDet += " ,LE_COSREF ";
                sQrySGDet += " ,LE_ETAPRD ";
                sQrySGDet += " ,LE_ITMPRO ";
                sQrySGDet += " ,LE_DESTIN ";
                sQrySGDet += " ,LE_CANPED ";
                sQrySGDet += " ,LE_CODLOT ";
                sQrySGDet += " ,LE_DETLOT ";
                //sQrySGDet += " ,LE_FLAG01 ";
                //sQrySGDet += " ,LE_FLAG02 ";
                //sQrySGDet += " ,LE_FLAG03 ";
                //sQrySGDet += " ,LE_REAL01 ";
                //sQrySGDet += " ,LE_REAL02 ";
                //sQrySGDet += " ,LE_REAL03 ";
                //sQrySGDet += " ,LE_TEXT01 ";
                sQrySGDet += " ) VALUES (";
                sQrySGDet += tipoDoc + " ,"; //LE_TIPDOC ";
                sQrySGDet += " {0},"; //LE_NUMDOC ";
                sQrySGDet += correlativo + " ,"; //LE_CRRLIN ";

                sQrySGDet += tipoMov + " ,'"; //LE_TIPMOV ";
                sQrySGDet += detalle.Codigo + "',"; //LE_CODART ";
                sQrySGDet += movimientoExistencia.Encabezado.Bodega.Codigo + " ,"; //LE_CODBOD ";
                sQrySGDet += detalle.Cantidad + " ,"; //LE_CANART ";
                sQrySGDet += detalle.ValorUnitario + " ,"; //LE_VALUNI ";
                sQrySGDet += " 0,"; //LE_COSPMP ";
                sQrySGDet += " 0,"; //LE_CENCOS ";
                sQrySGDet += " 1,"; //LE_TIPMON ";
                sQrySGDet += " 1,"; //LE_TIPCAM ";
                sQrySGDet += " 0,"; //LE_COSCOM ";
                sQrySGDet += " 1,"; //LE_COSREF ";
                sQrySGDet += " 0,"; //LE_ETAPRD ";
                sQrySGDet += " 0,"; //LE_ITMPRO ";
                sQrySGDet += " 0,"; //LE_DESTIN ";
                sQrySGDet += " 0,"; //LE_CANPED ";
                sQrySGDet += " 0,"; //LE_CODLOT ";
                sQrySGDet += " '',"; //LE_DETLOT ";
                //sQrySGDet += " 0,"; //LE_FLAG01 ";
                //sQrySGDet += " 0,"; //LE_FLAG02 ";
                //sQrySGDet += " 0,"; //LE_FLAG03 ";
                //sQrySGDet += " 0,"; //LE_REAL01 ";
                //sQrySGDet += " 0,"; //LE_REAL02 ";
                //sQrySGDet += " 0,"; //LE_REAL03 ";
                //sQrySGDet += " ''); "; //LE_TEXT01) ;
                sQrySGDet += ");";
                correlativo++;
            }
            return sQrySGDet;
        }
        public bool UpdateMovimientoExistencia(MovimientoExistencia movimientoExistencia, int idEmpresa)
        {
            bool result = false;
            string sQrySGEnc = string.Empty;
            string sQryRefBorrar = string.Empty;
            string sQrySGDet = string.Empty;
            string sQrySGDetBorrar = string.Empty;
            string sQryGetNroDoc = string.Empty;
            string sQryWebDocRef = string.Empty;
            string tipoDoc = string.Empty;
            string tipoMov = string.Empty;
            #region Querys Sysgestion
            if (movimientoExistencia.Encabezado.Movimiento == 1)
            {
                tipoDoc += " 38"; // INGRESO
                tipoMov = " 'E'";
            }
            else
            {
                tipoDoc += " 42 "; // SALIDA
                tipoMov = " 'S'";
            }
           
            sQrySGEnc += " UPDATE ENCEXI SET ";
            sQrySGEnc += " EE_FECEMI = '" + String.Format("{0:yyyyMMdd}", movimientoExistencia.Encabezado.FechaEmision) + "',"; ;
            sQrySGEnc += " EE_OBSERV = '" + movimientoExistencia.Encabezado.Observacion + "',"; ;
            sQrySGEnc += " EE_CODBOD = " + movimientoExistencia.Encabezado.Bodega.Codigo + " ";
            sQrySGEnc += " WHERE EE_TIPDOC = " + tipoDoc ;
            sQrySGEnc += " AND EE_NUMDOC = " + movimientoExistencia.Encabezado.NroDocumento;

            /*  Los detalles se borran y luego se insertan  */
            // BORRAR REFERENCIAS
            sQryRefBorrar += " DELETE FROM ReferenciaDocExistencia WHERE 1 = 1 ";
            sQryRefBorrar += " AND EF_TIPDOC = {0} AND EE_NUMDOC = {1} AND ID_EMPRESA = {2}";
            sQryRefBorrar = string.Format(sQryRefBorrar, tipoDoc, movimientoExistencia.Encabezado.NroDocumento, idEmpresa);
            // BORRAR DETALLES
            sQrySGDetBorrar = " DELETE FROM LINEXI WHERE LE_TIPDOC = " + tipoDoc;
            sQrySGDetBorrar += " AND LE_NUMDOC = " + movimientoExistencia.Encabezado.NroDocumento;

            sQrySGDet = GetQueryInsertLINFAC(movimientoExistencia);

            #endregion
            #region Query Web
            if (movimientoExistencia.Encabezado.referencia != null)
            {
                sQryWebDocRef += " INSERT INTO ReferenciaDocExistencia ";
                sQryWebDocRef += " (EF_TIPDOC ";
                sQryWebDocRef += " , EE_NUMDOC ";
                sQryWebDocRef += " , ID_DOCREF ";
                sQryWebDocRef += " , NRO_DOCREF ";
                sQryWebDocRef += " , ID_EMPRESA) ";
                sQryWebDocRef += " VALUES ( ";
                sQryWebDocRef += tipoDoc + " , ";
                sQryWebDocRef += " {0}, ";
                sQryWebDocRef += movimientoExistencia.Encabezado.referencia.Id + " , ";
                sQryWebDocRef += movimientoExistencia.Encabezado.nroReferencia + " , ";
                sQryWebDocRef += idEmpresa + " ) ";
            }
            #endregion
            sQrySGDet = sQrySGDet.Substring(0, sQrySGDet.Length - 1);
           
            SqlConnection cnWeb = new SqlConnection(Connection.GetConnection());
            
            sQrySGDet = string.Format(sQrySGDet, movimientoExistencia.Encabezado.NroDocumento);
            sQrySGEnc = string.Format(sQrySGEnc, movimientoExistencia.Encabezado.NroDocumento);
           
            bool resultWeb = false;
            SqlTransaction trWeb = null;
            if (movimientoExistencia.Encabezado.referencia != null)
            {
                sQryWebDocRef = string.Format(sQryWebDocRef, movimientoExistencia.Encabezado.NroDocumento);
                cnWeb.Open();
                trWeb = cnWeb.BeginTransaction(IsolationLevel.Serializable);
                SqlCommand cmdWebDelete = new SqlCommand(sQryRefBorrar, cnWeb, trWeb);
                SqlCommand cmdWebInsert = new SqlCommand(sQryWebDocRef, cnWeb, trWeb);
                
                try
                {
                    cmdWebDelete.ExecuteNonQuery();
                    cmdWebInsert.ExecuteNonQuery();
                    resultWeb = true;
                }
                catch (Exception ex)
                {
                    trWeb.Rollback();
                    cnWeb.Close();
                    Log.Error("Error al actualizar movimiento de existencia, actualizacion referencia del documento en BD Web", ex);
                }
            }
            else
            {
                cnWeb.Open();
                trWeb = cnWeb.BeginTransaction(IsolationLevel.Serializable);
                SqlCommand cmdWebDelete = new SqlCommand(sQryRefBorrar, cnWeb, trWeb);
                try
                {
                    cmdWebDelete.ExecuteNonQuery();
                    resultWeb = true;
                }
                catch (Exception ex)
                {
                    trWeb.Rollback();
                    cnWeb.Close();
                    Log.Error("Error al actualizar movimiento de existencia, actualizacion referencia del documento en BD Web", ex);
                }
            }
            if (resultWeb)
            {
                string sCon = Connection.GetConnection(queryXML.Doc.SelectSingleNode("Query/stringConnection").InnerText, idEmpresa);

                SqlConnection cnSG = new SqlConnection(sCon);
                cnSG.Open();
                SqlTransaction trSG = cnSG.BeginTransaction(IsolationLevel.Serializable);

                SqlCommand cmdSGEnc = new SqlCommand(sQrySGEnc, cnSG, trSG);
                SqlCommand cmdSGDetBorrar = new SqlCommand(sQrySGDetBorrar, cnSG, trSG);
                SqlCommand cmdSGDet = new SqlCommand(sQrySGDet, cnSG, trSG);


                try
                {
                    cmdSGEnc.ExecuteNonQuery();
                    cmdSGDetBorrar.ExecuteNonQuery();                    
                    cmdSGDet.ExecuteNonQuery();
                    trSG.Commit();
                   
                    trWeb.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    trSG.Rollback();
                    if (movimientoExistencia.Encabezado.referencia != null)
                    {
                        trWeb.Rollback();
                    }
                    cnSG.Close();
                    cnWeb.Close();
                    Log.Error("Error al crear movimiento de existencia", ex);
                }
            }
            return result;
        }

        public DataSet GetRoles()
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            string sConnection = string.Empty;
            try
            {
                sQry = queryXML.Doc.DocumentElement.SelectSingleNode("GetRoles").InnerText;
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection());
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (SqlException ex)
            {
                Log.Error("Error al intentar obtener Roles", ex);
                dsResult = null;
            }
            return dsResult;
        }

        public DataSet GetUsuarioRol()
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            string sConnection = string.Empty;
            try
            {
                sQry = queryXML.Doc.DocumentElement.SelectSingleNode("GetUsuarioRol").InnerText;
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection());
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (SqlException ex)
            {
                Log.Error("Error al intentar obtener Rol de Usuario, qry: " + sQry, ex);
                dsResult = null;
            }
            return dsResult;
        }

        public DataSet GetFormasCobroXml()
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            string sConnection = string.Empty;
            try
            {
                sQry = queryXML.Doc.DocumentElement.SelectSingleNode("GetFormasCobro").InnerText;
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection());
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar obtener formas de cobro, qry: " + sQry, ex);
                dsResult = null;
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
            catch (Exception ex)
            {
                Log.Error("Error al ejecutar transacción, qry: " + query, ex);
                tr.Rollback();
                bResult = false;
            }
            finally
            {
                cn.Close(); //Cerramos la conexion
            }
            return bResult;
        }
        public DataSet GetXmlQueryToDataSet(string queryString, int idEmpresa)
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            try
            {
                sQry = queryXML.Doc.DocumentElement.SelectSingleNode(queryString).InnerText.Replace("\r\n", "");
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection(queryXML.Doc.DocumentElement.SelectSingleNode("stringConnection").InnerText,idEmpresa));
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (Exception ex)
            {
                Log.Error("Error GetXmlQueryToDataSet, qry: " + sQry, ex);
                dsResult = null;
            }
            return dsResult;
        }
        public DataSet GetXmlQueryToDataSet(string queryString, string[] param, int idEmpresa)
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            try
            {
                sQry = string.Format(queryXML.Doc.DocumentElement.SelectSingleNode(queryString).InnerText, param);
                
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection(queryXML.Doc.DocumentElement.SelectSingleNode("stringConnection").InnerText, idEmpresa));
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (Exception ex)
            {
                Log.Error("Error GetXmlQueryToDataSet, qry: " + sQry, ex);
                dsResult = null;
            }
            return dsResult;
        }

        public DataSet GetInfoArticulo(string queryString, string[] param)
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
            catch (Exception ex)
            {
                Log.Error("Error al obtener información de articulo, qry: " + sQry, ex);
                dsResult = null;
            }
            return dsResult;
        }

        internal bool UpdateArticulo(Articulo articulo, int idEmpresa)
        {

            bool result = false;
            string sQryVerif = string.Empty;
            string sQryUpdateSG = string.Empty;
            string sQryUpdatePortal = string.Empty;
            string sQryCreate = string.Empty;
            DataSet dsVerif = null;
            try
            {
                // Verifico que el articulo existe en la BD del portal BVC
                dsVerif = GetInfoArticulo("GetInfoArticulo", new string[2] { idEmpresa.ToString(), articulo.Codigo });

                if (DataSetUtils.hasRows(dsVerif))// Si existe realizo UPDATE c/transaccion
                {
                    sQryUpdatePortal = "UPDATE Articulo";
                    sQryUpdatePortal += " SET UnidadesPorCaja = " + articulo.UniXcaja;
                    sQryUpdatePortal += " ,CajasPorPallet = " + articulo.CXpallet;
                    sQryUpdatePortal += " ,Largo = " + articulo.Largo;
                    sQryUpdatePortal += " ,Ancho = " + articulo.Ancho;
                    sQryUpdatePortal += " ,Alto = " + articulo.Alto;
                    sQryUpdatePortal += " WHERE ID_Empresa = " + idEmpresa;
                    sQryUpdatePortal += " AND AR_CODART =  '" + articulo.Codigo + "'";

                }
                else // Si no existe, realizo el INSERT  c/transaccion
                {
                    sQryCreate = "INSERT INTO Articulo " +
                                        "(ID_Empresa " +
                                        ", AR_CODART " +
                                        ", UnidadesPorCaja " +
                                        ", CajasPorPallet " +
                                        ", Largo " +
                                        ", Ancho " +
                                        ", Alto) " +
                                        "VALUES (" +
                                        idEmpresa + "," +
                                        articulo.Codigo + "," +
                                        articulo.UniXcaja + "," +
                                        articulo.CXpallet + "," +
                                        articulo.Largo + "," +
                                        articulo.Ancho + "," +
                                        articulo.Alto + ")";
                }
                sQryUpdateSG = "UPDATE MAEART "+
                                "SET AR_DESART = '" + articulo.Descripcion + "'" +
                                ", AR_UNIMED = '" + articulo.UnidadDeMedida + "'" +
                                ", AR_ACTIVO = " + Convert.ToInt16(articulo.IsActivo) +
                                " WHERE AR_CODART = '" + articulo.Codigo +"'";
                string sCon = Connection.GetConnection(queryXML.Doc.SelectSingleNode("Query/stringConnection").InnerText, idEmpresa);

                SqlConnection cnSG = new SqlConnection(sCon);
                SqlConnection cnWeb = new SqlConnection(Connection.GetConnection());              

                //Abrimos conexion Del portal
                cnWeb.Open();
                cnSG.Open();
                //Definimos que es una transaccion
                SqlTransaction trWeb = cnWeb.BeginTransaction(IsolationLevel.Serializable);
                SqlTransaction trSG = cnSG.BeginTransaction(IsolationLevel.Serializable);
                //Comando que ejecuta el insert en la tb_pedido
                SqlCommand cmdWeb = null;
                if (sQryCreate != string.Empty)
                {
                    cmdWeb = new SqlCommand(sQryCreate, cnWeb, trWeb);
                }
                else
                {
                    cmdWeb = new SqlCommand(sQryUpdatePortal, cnWeb, trWeb);
                }
                SqlCommand cmdSG = new SqlCommand(sQryUpdateSG, cnSG, trSG);
                bool resultadoWEB = false;
                try
                {
                    //Ejecuto
                    cmdWeb.ExecuteNonQuery();
                    
                    resultadoWEB = true;
                }
                catch (SqlException  ex)
                {
                    Log.Error("Error ExecuteNonQuery, qry: " + sQryUpdatePortal, ex);
                    trWeb.Rollback();
                    cnWeb.Close(); //Cerramos la conexion                    
                    result = false;
                }
                
                if (resultadoWEB)
                {
                    try
                    {
                        cmdSG.ExecuteNonQuery();
                        trSG.Commit();
                        trWeb.Commit();
                        result = true;
                    }
                    catch (SqlException ex)
                    {
                        Log.Error("Error ExecuteNonQuery, qry: " + sQryUpdateSG, ex);
                        trSG.Rollback();
                        result = false;
                    }
                    finally
                    {
                        cnSG.Close(); //Cerramos la conexion
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al actualizar articulo", ex);
            }
            return result;
        }

        internal bool CreateArticulo(Articulo articulo, int idEmpresa)
        {

            bool result = false;
            string sQrySG = string.Empty;
            string sQryWeb = string.Empty;
            try
            {


                sQryWeb += " INSERT INTO Articulo ";
                sQryWeb += "(ID_Empresa  ";
                sQryWeb += " , AR_CODART ";
                sQryWeb += " , UnidadesPorCaja ";
                sQryWeb += " , CajasPorPallet ";
                sQryWeb += " , Largo ";
                sQryWeb += " , Ancho ";
                sQryWeb += " , Alto) VALUES (";
                sQryWeb += idEmpresa + ",'";
                sQryWeb += articulo.Codigo + "',";
                sQryWeb += articulo.UniXcaja + ",";
                sQryWeb += articulo.CXpallet + ",";
                sQryWeb += articulo.Largo + ",";
                sQryWeb += articulo.Ancho + ",";
                sQryWeb += articulo.Alto + ")";

                sQrySG += " INSERT INTO MAEART ";
                sQrySG += " (AR_CODART ";
                sQrySG += " ,AR_DESART ";
                sQrySG += " ,AR_CODBAR ";
                sQrySG += " ,AR_CODEXT ";
                sQrySG += " ,AR_FECCRE ";
                sQrySG += " ,AR_UNIMED ";
                sQrySG += " ,AR_CODFAM ";
                sQrySG += " ,AR_ACTIVO ";
                sQrySG += " ,AR_CTRINV ";
                sQrySG += " ,AR_FORMUL ";
                sQrySG += " ,AR_TIPMON ";
                sQrySG += " ,AR_TIPMOC ";
                sQrySG += " ,AR_PROCED ";
                sQrySG += " ,AR_SUBFAM ";
                sQrySG += " ,AR_CTACVT ";
                sQrySG += " ,AR_LARART ";
                sQrySG += " ,AR_ANCART ";
                sQrySG += " ,AR_ALTART ";
                sQrySG += " ,AR_TIPCON ";
                sQrySG += " ,AR_CAJMAS ";
                sQrySG += " ,AR_VALCIF ";
                sQrySG += " ,AR_VALFOB ";
                sQrySG += " ,AR_CTAMER ";
                sQrySG += " ,AR_CCTMER ";
                sQrySG += " ,AR_USASER ";
                sQrySG += " ,AR_USALOT ";
                sQrySG += " ,AR_FLAG01 ";
                sQrySG += " ,AR_REAL01 ";
                sQrySG += " ) VALUES ('";
                sQrySG += articulo.Codigo + "','";
                sQrySG += articulo.Descripcion + "','";
                sQrySG += articulo.Codigo + "','";// CODBAR
                sQrySG += articulo.Codigo + "','";// CODEXT
                sQrySG += String.Format("{0:yyyyMMdd}", DateTime.Now) + "','";//AR_FECCRE
                sQrySG += articulo.UnidadDeMedida + "',";//AR_UNIMED
                sQrySG += 99 + ",";//AR_CODFAM
                sQrySG += Convert.ToInt32(articulo.IsActivo) + ",";//AR_ACTIVO
                sQrySG += 1 + ",";//AR_CTRINV
                sQrySG += 0 + ",";//AR_FORMUL
                sQrySG += 1 + ",";//AR_TIPMON
                sQrySG += 1 + ",";//AR_TIPMOC
                sQrySG += 99 + ",";//AR_PROCED
                sQrySG += 99 + ", ";//AR_SUBFAM
                sQrySG += "'4010001', ";//AR_CTACVT
                sQrySG += 0 + ",";//AR_LARART
                sQrySG += 0 + ",";//AR_ANCART
                sQrySG += 0 + ",";//AR_ALTART
                sQrySG += 0 + ",";//AR_TIPCON
                sQrySG += 0 + ",";//AR_CAJMAS
                sQrySG += 0 + ",";//AR_VALCIF
                sQrySG += 0 + ",";//AR_VALFOB
                sQrySG += " '1020101',";//AR_CTAMER
                sQrySG += 0 + ",";//AR_CCTMER
                sQrySG += 0 + ",";//AR_USASER
                sQrySG += 0 + ",";//AR_USALOT
                sQrySG += 0 + ",";//AR_FLAG01
                sQrySG += 0 + "";//AR_REAL01
                sQrySG +=  ")";


                string sCon = Connection.GetConnection(queryXML.Doc.SelectSingleNode("Query/stringConnection").InnerText, idEmpresa);

                SqlConnection cnSG = new SqlConnection(sCon);
                SqlConnection cnWeb = new SqlConnection(Connection.GetConnection());

                //Abrimos conexion Del portal
                cnWeb.Open();

                //Definimos que es una transaccion
                SqlTransaction trWeb = cnWeb.BeginTransaction(IsolationLevel.Serializable);

                //Comando que ejecuta el insert en la tb_pedido
                SqlCommand cmdWeb = new SqlCommand(sQryWeb, cnWeb, trWeb);

                bool resultadoWEB = false;
                try
                {
                    cmdWeb.ExecuteNonQuery();
                    resultadoWEB = true;
                }
                catch (SqlException ex)
                {
                    Log.Error("Error al crear articulo, ExecuteNonQuery query: " + sQryWeb, ex);
                    trWeb.Rollback();
                    cnWeb.Close(); //Cerramos la conexion                    
                    result = false;
                }

                if (resultadoWEB)
                {
                    cnSG.Open();
                    SqlTransaction trSG = cnSG.BeginTransaction(IsolationLevel.Serializable);
                    SqlCommand cmdSG = new SqlCommand(sQrySG, cnSG, trSG);
                    try
                    {
                        cmdSG.ExecuteNonQuery();
                        trSG.Commit();
                        trWeb.Commit();
                        result = true;
                    }
                    catch (SqlException ex)
                    {
                        Log.Error("Error al crear articulo, ExecuteNonQuery query: "+ sQrySG, ex);
                        trSG.Rollback();
                        trWeb.Rollback();
                        result = false;
                    }
                    finally
                    {
                        cnSG.Close(); //Cerramos la conexion                    
                        cnWeb.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear articulo", ex);
            }
            return result;
        }

        public bool DeleteXmlQuery(string queryString, string[] param, int idEmpresa)
        {
            bool result = false;
            string sQry = string.Empty;
            try
            {
                sQry = string.Format(queryXML.Doc.DocumentElement.SelectSingleNode(queryString).InnerText, param);

                result = BeginTransaction(sQry, Connection.GetConnection(queryXML.Doc.DocumentElement.SelectSingleNode("stringConnection").InnerText, idEmpresa));
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear articulo ExecuteNonQuery, query: " + sQry, ex);
            }
            return result;
        }
        
        public bool ExisteArticulo(string queryString, string[] param, int idEmpresa)
        {
            bool result = false;
            
            DataSet dsExist = null;
            
            dsExist = GetXmlQueryToDataSet(queryString,param, idEmpresa);
            if (DataSetUtils.hasRows(dsExist))
            {
                result = true;
            }            
            return result;
        }
        public DataSet GetEmpresas()
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            string sConnection = string.Empty;
            try
            {
                sQry = queryXML.Doc.DocumentElement.SelectSingleNode("GetEmpresas").InnerText;
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnectionNucleo());
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear Obtener empresas, query: " + sQry, ex);
                dsResult = null;
            }
            return dsResult;
        }
        public DataSet GetEmpresasFormasCobros()
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            string sConnection = string.Empty;
            try
            {
                sQry = queryXML.Doc.DocumentElement.SelectSingleNode("GetEmpresasFormasCobros").InnerText;
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection());
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (Exception ex)
            {
                Log.Error("Error al obtener formas de cobro, query: " + sQry, ex);
                dsResult = null;
            }
            return dsResult;
        }
        public DataSet GetAllUsuarios()
        {
            DataSet dsResult = null;
            string sQry = string.Empty;
            string sConnection = string.Empty;
            try
            {
                sQry = queryXML.Doc.DocumentElement.SelectSingleNode("GetAllUsers").InnerText;
                SqlDataAdapter adapter = new SqlDataAdapter(sQry, Connection.GetConnection());
                dsResult = new DataSet();
                adapter.Fill(dsResult);
            }
            catch (Exception ex)
            {
                Log.Error("Error al obtener usuarios, query: " + sQry, ex);
                dsResult = null;
            }
            return dsResult;
        }
    }
}