using Api.DBUtils;
using Api.Pocos;
using Api.POCOS;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;

namespace Api.Controllers.bfc
{
    public class StockModels
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int CreateMovimientoExistencia(MovimientoExistencia movimientoExistencia, int idEmpresa)
        {
            int result = 0;
            SqlUtils sm = new SqlUtils();
            result = sm.CreateMovimientoExistencia(movimientoExistencia, idEmpresa);
            return result;
        }

        internal List<MovimientoExistencia> GetMovimientoExistencia(Encabezado encabezado, int idEmpresa)
        {
            List<MovimientoExistencia> result = new List<MovimientoExistencia>();
            SqlUtils sm = new SqlUtils();
            DataSet dsReferencias = new DataSet();
            MovimientoExistencia movExi = null;
            Encabezado enc = null;
            DataSet dsMov = new DataSet();
            if (encabezado.nroReferencia > 0 || encabezado.referencia != null)
            {
                // Consulta los documentos en BD WEB
                dsReferencias = sm.GetReferenciasMovimientosExistencias(encabezado, idEmpresa);
                if (dsReferencias == null)
                {
                    return null;
                }else if (DataSetUtils.IsEmpty(dsReferencias))
                {
                    return result;
                }
            }
            dsMov = sm.GetMovimientoExistencia(encabezado, dsReferencias, idEmpresa);

            if (DataSetUtils.hasRows(dsMov))
            {
                int tipoMov = 0;
                foreach (DataRow dr in dsMov.Tables[0].Rows)
                {
                    movExi = new MovimientoExistencia();
                    enc = new Encabezado();
                    enc.Bodega = new Bodega();
                    enc.Bodega.Codigo = Convert.ToInt32(dr["EE_CODBOD"]);
                    tipoMov = Convert.ToInt32(dr["EE_TIPDOC"]);
                    if (tipoMov == 38)
                    {
                        enc.Movimiento = 1;
                    }
                    else if (tipoMov == 42)
                    {
                        enc.Movimiento = 2;
                    }
                    enc.FechaEmision = Convert.ToDateTime(dr["EE_FECEMI"]);
                    enc.Observacion = dr["EE_OBSERV"].ToString();
                    enc.NroDocumento = Convert.ToInt32(dr["EE_NUMDOC"]);
                    movExi.Encabezado = enc;
                    result.Add(movExi);
                }                
            }
            return result;
        }

        internal MovimientoExistencia GetDetalleMovimientoExistencia(Encabezado encabezado, int idEmpresa)
        {
            MovimientoExistencia result = new MovimientoExistencia();
            SqlUtils sm = new SqlUtils();
            MovimientoExistencia movExi = null;
            Detalle det = null;
            List<Detalle> detalles = new List<Detalle>();
            DataSet dsReferencias = null;
            if (encabezado.referencia == null)
            {
                dsReferencias = sm.GetReferenciasMovimientosExistencias(encabezado.Movimiento,encabezado.NroDocumento, idEmpresa);
                if (DataSetUtils.hasRows(dsReferencias))
                {
                    encabezado.referencia = new Models.bfc.DocumentoReferencia();
                    encabezado.referencia.Id = Convert.ToInt32(dsReferencias.Tables[0].Rows[0]["ID_DOCREF"]);
                    encabezado.referencia.CodigoDocumento = dsReferencias.Tables[0].Rows[0]["CodigoDocumento"].ToString();
                    encabezado.referencia.Descripcion = dsReferencias.Tables[0].Rows[0]["Descripcion"].ToString();
                    encabezado.nroReferencia = Convert.ToInt32(dsReferencias.Tables[0].Rows[0]["NRO_DOCREF"]);
                }
            }


            DataSet dsMov = new DataSet();
            dsMov = sm.GetDetalleMovimientoExistencia(encabezado, idEmpresa);
            int tipoMov = 0;
            foreach (DataRow dr in dsMov.Tables[0].Rows)
            {
                det = new Detalle();
                det.Cantidad = Convert.ToDouble(dr["LE_CANART"]);
                det.Codigo = dr["LE_CODART"].ToString();
                det.ValorUnitario = Convert.ToDouble(dr["LE_VALUNI"]);
                det.Descripcion = dr["AR_DESART"].ToString();
                detalles.Add(det);
            }
            result.Detalles = detalles;

            return result;
        }

        internal bool UpdateMovimientoExistencia(MovimientoExistencia movimientoExistencia, int idEmpresa)
        {
            bool result = false;
            SqlUtils su = new SqlUtils();
            result = su.UpdateMovimientoExistencia(movimientoExistencia, idEmpresa);
            return result;
        }

        internal bool DeleteMovimientoExistencia(MovimientoExistencia movimientoExistencia, int idEmpresa)
        {
            bool result = false;
            SqlUtils su = new SqlUtils();
            result = su.DeleteMovimientoExistencia(movimientoExistencia, idEmpresa);
            return result;
        }
    }
}