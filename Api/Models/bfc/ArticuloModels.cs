using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Api.DBUtils;
using Api.POCOS;
using log4net;
using Newtonsoft.Json;

namespace Api.Models.bfc
{
    public class ArticuloModels
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="bodega"></param>
        /// <param name="idEmpresa"></param>
        /// <returns></returns>
        public List<Articulo> GetStockArticulos(string codigo, string bodega,int idEmpresa)
        {
            List<Articulo> result = null;
            Articulo articulo = null;
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            
            if (codigo == null || codigo == string.Empty)
            {
                if (bodega == null || bodega == string.Empty)
                {
                    dsArticulos = su.GetXmlQueryToDataSet("GetStockArticulos",idEmpresa);
                }
                else
                {
                    dsArticulos = su.GetXmlQueryToDataSet("GetStockArticulosEnBodega",
                        new string[1] { bodega },idEmpresa);
                }                
            }
            else
            {
                if (bodega == null || bodega == string.Empty)
                {
                    dsArticulos = su.GetXmlQueryToDataSet("GetStockArticulo", new string[1] { codigo },idEmpresa);
                }
                else
                {
                    dsArticulos = su.GetXmlQueryToDataSet("GetStockArticuloEnBodega",
                        new string[2] { bodega,codigo },idEmpresa);
                }                
            }

            if (DataSetUtils.hasRows(dsArticulos))
            {
                result = new List<Articulo>();
                foreach (DataRow dr in dsArticulos.Tables[0].Rows)
                {
                    articulo = new Articulo();
                    articulo.Descripcion = dr["AR_DESART"].ToString();
                    articulo.Codigo = dr["AR_CODART"].ToString();
                    try
                    {
                        articulo.UnidadDeMedida = dr["AR_UNIMED"].ToString();
                    }
                    catch (Exception)
                    {                       
                    }
                    try
                    {
                        articulo.Stock = dr["STOCK"].ToString();
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                    result.Add(articulo);
                }
            }
            return result;
        }

        public Informe GetEntradasSalidas(string codigo, string bodega,int anno, int idEmpresa)
        {
            Informe result = new Informe() { };
            DataSet dsEntSal = null;
            DataSet dsStockInicial = null;
            SqlUtils su = new SqlUtils();
            dsStockInicial = su.GetStockInicial(codigo, bodega, anno, idEmpresa);
            if (dsStockInicial == null)
            {
                return null;
            }
            if (DataSetUtils.hasRows(dsStockInicial))
            {
                try
                {
                    result.StockInicial = Convert.ToInt32(dsStockInicial.Tables[0].Rows[0]["ENTRADA"]) - Convert.ToInt32(dsStockInicial.Tables[0].Rows[0]["SALIDA"]);
                }
                catch
                {
                    result.StockInicial = Convert.ToInt32(dsStockInicial.Tables[0].Rows[0]["SALDO"]);
                }
                
            }
            dsEntSal = su.GetEntradasSalidas(codigo, bodega, anno, idEmpresa);
            if (dsEntSal == null)
            {
                return null;
            }
            result.Enero = new int[3];
            result.Febrero = new int[3];
            result.Marzo = new int[3];
            result.Abril = new int[3];
            result.Mayo = new int[3];
            result.Junio = new int[3];
            result.Julio = new int[3];
            result.Agosto = new int[3];
            result.Septiembre = new int[3];
            result.Octubre = new int[3];
            result.Noviembre = new int[3];
            result.Diciembre = new int[3];
            if (DataSetUtils.hasRows(dsEntSal))
            {
                
                foreach (DataRow dr in dsEntSal.Tables[0].Rows)
                {
                    switch (Convert.ToInt32(dr["sd1_mesper"]))
                    {
                        case 1:
                            result.Enero[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Enero[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 2:
                            result.Febrero[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Febrero[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 3:
                            result.Marzo[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Marzo[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 4:
                            result.Abril[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Abril[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 5:
                            result.Mayo[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Mayo[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 6:
                            result.Junio[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Junio[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 7:
                            result.Julio[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Julio[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 8:
                            result.Agosto[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Agosto[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 9:
                            result.Septiembre[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Septiembre[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 10:
                            result.Octubre[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Octubre[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 11:
                            result.Noviembre[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Noviembre[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                        case 12:
                            result.Diciembre[0] = Convert.ToInt32(dr["ENTRADAS"]);
                            result.Diciembre[1] = Convert.ToInt32(dr["SALIDAS"]);
                            break;
                    }
                }
                result.Enero[2] = result.StockInicial + result.Enero[0] - result.Enero[1];
                result.Febrero[2] = result.Enero[2] + result.Febrero[0] - result.Febrero[1];
                result.Marzo[2] = result.Febrero[2] + result.Marzo[0] - result.Marzo[1];
                result.Abril[2] = result.Marzo[2] + result.Abril[0] - result.Abril[1];
                result.Mayo[2] = result.Abril[2] + result.Mayo[0] - result.Mayo[1];
                result.Junio[2] = result.Mayo[2] + result.Junio[0] - result.Junio[1];
                result.Julio[2] = result.Junio[2] + result.Julio[0] - result.Julio[1];
                result.Agosto[2] = result.Julio[2] + result.Agosto[0] - result.Agosto[1];
                result.Septiembre[2] = result.Agosto[2] + result.Septiembre[0] - result.Septiembre[1];
                result.Octubre[2] = result.Septiembre[2] + result.Octubre[0] - result.Octubre[1];
                result.Noviembre[2] = result.Octubre[2] + result.Noviembre[0] - result.Noviembre[1];
                result.Diciembre[2] = result.Noviembre[2] + result.Diciembre[0] - result.Diciembre[1];
                result.Anno = anno;
            }
            return result;
        }

        internal TarjetaExistencia GetDetalleEntradasSalidas(string codigo, string bodega, int anno, int mes,int saldo, int idEmpresa)
        {
            TarjetaExistencia result = new TarjetaExistencia();
            DataSet dsMov = null;
            List<int> ingresos = new List<int>();
            List<int> egresos = new List<int>();
            SqlUtils su = new SqlUtils();
            dsMov = su.GetDetallesEntradasSalidas(codigo, bodega, anno, mes, idEmpresa);
            if (dsMov == null)
            {
                return null;
            }
            if (DataSetUtils.hasRows(dsMov))
            {
                result.encabezado = new EncabezadoTarjetaExistencia();
                result.detalles = new List<DetalleTarjetaExistencia>();
                result.encabezado.Anno = anno;
                result.encabezado.Mes = mes;
                result.encabezado.Articulo = new Articulo();
                result.encabezado.Articulo.Codigo = codigo;
                DetalleTarjetaExistencia dte = null;
                
                foreach (DataRow dr in dsMov.Tables[0].Rows)
                {
                    dte = new DetalleTarjetaExistencia();
                    dte.Bodega = new Bodega()
                    {
                        Codigo = Convert.ToInt32(dr["EE_CODBOD"])
                    };
                    dte.Cantidad = Convert.ToInt32(dr["LE_CANART"]);
                    dte.Dia = Convert.ToDateTime(dr["EE_FECEMI"]).Day;
                    string tipdoc = dr["EE_TIPDOC"].ToString();
                    dte.Movimiento = tipdoc == "38" ? 1 : 2;
                    dte.NroDocumento = Convert.ToInt32(dr["EE_NUMDOC"]);
                    if (dte.Movimiento == 1)
                    {
                        saldo += dte.Cantidad;
                        ingresos.Add(dte.NroDocumento);
                    }
                    else
                    {
                        saldo = saldo - dte.Cantidad;
                        egresos.Add(dte.NroDocumento);
                    }
                    dte.Saldo = saldo;

                    result.detalles.Add(dte);
                }
            }
            // Obtener las referencias 
            DataSet dsReferencias = su.GetReferenciasMovimientosExistencias(ingresos, egresos, idEmpresa);
            if (dsReferencias == null)
            {
                return null;
            }
            if (DataSetUtils.hasRows(dsReferencias))
            {
                foreach (DataRow dr in dsReferencias.Tables[0].Rows) 
                {
                    int tpoDoc = Convert.ToInt32(dr["EF_TIPDOC"]) == 38 ? 1 : Convert.ToInt32(dr["EF_TIPDOC"]) == 42 ? 2 : 0;
                    int numDoc = Convert.ToInt32(dr["EE_NUMDOC"]);
                    foreach (var item in result.detalles)
                    {
                        if (item.NroDocumento == numDoc && item.Movimiento == tpoDoc)
                        {
                            item.NroReferencia = Convert.ToInt32(dr["NRO_DOCREF"]);
                            item.Referencia = dr["Descripcion"].ToString();
                        }                        
                    }
                }
            }
            return result;
        }

        public string GetListadoEntradasSalidas(string codigo, DateTime desde, DateTime hasta, int idEmpresa)
        {
            string result = string.Empty;
            DataSet dsEntSal = null;
            
            SqlUtils su = new SqlUtils();
            dsEntSal = su.GetListadoEntradasSalidas(codigo, desde, hasta, idEmpresa);

            if (DataSetUtils.hasRows(dsEntSal))
            {
                result = JsonConvert.SerializeObject(dsEntSal.Tables[0]);
            }
            return result;
        }

        public List<Articulo> GetAllArticulos(string param, int idEmpresa)
        {
            List<Articulo> result = null;
            Articulo articulo = null;
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            dsArticulos = su.GetXmlQueryToDataSet("GetAllArticulos", new string[1] { param }, idEmpresa);
            if (DataSetUtils.hasRows(dsArticulos))
            {
                result = new List<Articulo>();
                foreach (DataRow dr in dsArticulos.Tables[0].Rows)
                {
                    articulo = new Articulo();
                    articulo.Descripcion = dr["AR_DESART"].ToString();
                    articulo.Codigo = dr["AR_CODART"].ToString();
                    articulo.UnidadDeMedida = dr["AR_UNIMED"].ToString();
                    articulo.IsActivo = Convert.ToBoolean(dr["AR_ACTIVO"]);
                    result.Add(articulo);
                }
            }
            return result;
        }

        public Articulo GetDetalleArticulo(Articulo articulo, int idEmpresa)        {
            
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            dsArticulos = su.GetInfoArticulo("GetInfoArticulo", new string[2] { idEmpresa.ToString(), articulo.Codigo });
            if (DataSetUtils.hasRows(dsArticulos))
            {                
                foreach (DataRow dr in dsArticulos.Tables[0].Rows)
                {                   
                    articulo.UniXcaja = Convert.ToInt32(dr["UnidadesPorCaja"]);
                    articulo.CXpallet = Convert.ToInt32(dr["CajasPorPallet"]);
                    articulo.Largo = Convert.ToInt32(dr["Largo"]);
                    articulo.Ancho = Convert.ToInt32(dr["Ancho"]);
                    articulo.Alto = Convert.ToInt32(dr["Alto"]);
                }
            }
            return articulo;
        }

        public bool Remover(string codigo, int idEmpresa)
        {
            bool result = false;
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            result = su.DeleteXmlQuery("DeleteArticulo", new string[1] { codigo }, idEmpresa);
            
            return result;
        }
        
        public List<Articulo> GetArticulos(string param, int idEmpresa)
        {
            List<Articulo> result = null;
            Articulo articulo = null;
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            dsArticulos = su.GetXmlQueryToDataSet("SearchArticulos", new string[1] { param }, idEmpresa);
            if (DataSetUtils.hasRows(dsArticulos))
            {
                result = new List<Articulo>();
                foreach (DataRow dr in dsArticulos.Tables[0].Rows)
                {
                    articulo = new Articulo();
                    articulo.Descripcion = dr["AR_DESART"].ToString();
                    articulo.Codigo = dr["AR_CODART"].ToString();
                    result.Add(articulo);
                }
            }
            return result;
        }

        public bool Create(Articulo articulo, int idEmpresa)
        {
            bool result = false;

            SqlUtils su = new SqlUtils();
            result = su.CreateArticulo(articulo, idEmpresa);
            
            return result;
        }

        public bool ExisteArticulo(string codigo, int idEmpresa)
        {
            bool result = false;
            
            SqlUtils su = new SqlUtils();
            result = su.ExisteArticulo("ExisteArticulo", new string[1] { codigo.Trim() }, idEmpresa);

            return result;
        }

        public bool Update(Articulo articulo, int idEmpresa)
        {
            bool result = false;
            
            SqlUtils su = new SqlUtils();
            result = su.UpdateArticulo(articulo, idEmpresa);
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idEmpresa"></param>
        /// <returns></returns>
        public List<UnidadDeMedida> GetUnidadDeMedida(int idEmpresa)
        {
            List<UnidadDeMedida> result = null;
            UnidadDeMedida unidadDeMedida = null;
            DataSet dsUnidadDeMedida = null;
            SqlUtils su = new SqlUtils();
            dsUnidadDeMedida = su.GetXmlQueryToDataSet("GetUnidadesDeMedida", idEmpresa);
            if (DataSetUtils.hasRows(dsUnidadDeMedida))
            {
                result = new List<UnidadDeMedida>();
                foreach (DataRow dr in dsUnidadDeMedida.Tables[0].Rows)
                {
                    unidadDeMedida = new UnidadDeMedida();
                    unidadDeMedida.Id = Convert.ToInt32(dr["tb_codtab"]);
                    unidadDeMedida.Descripcion = dr["tb_destab"].ToString();
                    unidadDeMedida.Codigo = dr["tb_codstr"].ToString();
                    result.Add(unidadDeMedida);
                }
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="idEmpresa"></param>
        /// <returns></returns>
        public bool ExisteMovExistencia(string codigo, int idEmpresa)
        {
            bool result = false;
            
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            dsArticulos = su.GetXmlQueryToDataSet("ExisteMovExistencia", new string[1] { codigo }, idEmpresa);
            if (DataSetUtils.hasRows(dsArticulos))
            {
                result = true;
            }
            return result;
        }
    }
}