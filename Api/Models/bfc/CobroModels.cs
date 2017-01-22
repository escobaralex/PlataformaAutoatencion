using Api.DBUtils;
using Api.POCOS;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;

namespace Api.Controllers.bfc
{
    public class CobroModels
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<Cobro> GetCobrosCliente(string v1, int v2)
        {
            List<Cobro> result = null;
            return result;
        }

        public DataSet GetCobroAlmacenamiento(string mes, string ano, string idEmpresa)
        {
            DataSet result = null;
            string qry = string.Empty;

            // APETURA DE MES
            qry += " SELECT LE_CODART, CANTIDAD, LARGO, ANCHO, ALTO, ";
            qry += " (CANTIDAD * Largo * Ancho * 0.0001) AS M2, ";
            qry += " (CANTIDAD * Largo * Ancho * Alto * 0.000001) AS M3, ";
            qry += " (CANTIDAD / CajasPorPallet) AS POSICION_PALET, ";
            qry += " CEILING((CANTIDAD / CajasPorPallet)) AS POSICION_PALET_REDONDEADO ";
            qry += " FROM( ";
            qry += " SELECT LE_CODART, SUM(CANTIDAD) AS CANTIDAD FROM( ";
            qry += " SELECT LE_CODART, SUM(LE_CANART) CANTIDAD ";
            qry += " FROM a000_sysges" + idEmpresa + ".dbo.LINEXI ";
            qry += " JOIN a000_sysges" + idEmpresa + ".dbo.ENCEXI ON EE_TIPDOC = LE_TIPDOC AND EE_NUMDOC = LE_NUMDOC ";
            qry += " WHERE EE_FECEMI < '" + ano + mes +"01' AND LE_TIPMOV = 'E' ";
            qry += " GROUP BY LE_CODART ";
            qry += " UNION ALL ";
            qry += " SELECT LE_CODART, SUM(LE_CANART) * -1 CANTIDAD ";
            qry += " FROM a000_sysges" + idEmpresa + ".dbo.LINEXI ";
            qry += " JOIN a000_sysges" + idEmpresa + ".dbo.ENCEXI ON EE_TIPDOC = LE_TIPDOC AND EE_NUMDOC = LE_NUMDOC ";
            qry += " WHERE EE_FECEMI < '" + ano + mes + "01' AND LE_TIPMOV = 'S' ";
            qry += " GROUP BY LE_CODART ";
            qry += " ) AS D ";
            qry += " GROUP BY LE_CODART ";
            qry += " ) AS DATOS ";
            qry += " INNER JOIN WEB_BevfoodCenter.dbo.Articulo AS WA ";
            qry += " ON DATOS.LE_CODART = WA.AR_CODART ";
            qry += " WHERE DATOS.CANTIDAD <> 0 AND WA.ID_EMPRESA = " + idEmpresa;
            qry += " ORDER BY LE_CODART ;";

            // Movimientos del Mes
            qry += " SELECT EE_FECEMI,LE_CODART,LE_CANART,LE_TIPMOV,CajasPorPallet,Largo,Ancho,Alto ";
            qry += " , (LE_CANART * Largo * Ancho * 0.0001) AS M2";
            qry += " , (LE_CANART * Largo * Ancho * Alto * 0.000001) AS M3";
            qry += " , (LE_CANART / UnidadesPorCaja / CajasPorPallet) AS PP";
            qry += " , CEILING((LE_CANART / CajasPorPallet)) AS POSICION_PALET_REDONDEADO";
            qry += " FROM a000_sysges" + idEmpresa + ".dbo.LINEXI";
            qry += " JOIN a000_sysges" + idEmpresa + ".dbo.ENCEXI ON EE_TIPDOC = LE_TIPDOC AND EE_NUMDOC = LE_NUMDOC";
            qry += " LEFT JOIN";
            qry += " Web_BevfoodCenter.dbo.Articulo AS WA";
            qry += " ON a000_sysges" + idEmpresa + ".dbo.LINEXI.LE_CODART = WA.AR_CODART";
            qry += " WHERE a000_sysges" + idEmpresa + ".dbo.ENCEXI.EE_FECEMI > EOMONTH('" + ano + mes + "01',-1) AND a000_sysges" + idEmpresa + ".dbo.ENCEXI.EE_FECEMI <= EOMONTH('" + ano + mes + "01') ";
            qry += " and WA.ID_Empresa = " + idEmpresa;
            qry += " ORDER BY a000_sysges" + idEmpresa + ".dbo.ENCEXI.EE_FECEMI";

            SqlUtils u = new SqlUtils();
            result = u.GetDataSqlToDataSet(qry, Connection.GetConnection());
            return result;
            //return GetDetalleCobroAlmacenamiento(mes, ano, idEmpresa, result);
        }
        public DataSet GetDetalleCobroAlmacenamiento(string mes, string ano, string idEmpresa, DataSet dsApertura)
        {
            DataSet result = null;
            string qry = string.Empty;

            
            SqlUtils u = new SqlUtils();
            result = u.GetDataSqlToDataSet(qry, Connection.GetConnection());
            dsApertura.Tables[0].TableName = "DatosApertura";
            dsApertura.Tables.Add(result.Tables[0].Copy());

            return dsApertura;
        }
        private DataSet GetPosicionPalet(DataSet dsCobros)
        {
            DataSet result = new DataSet();

            return result;
        }
    }
}