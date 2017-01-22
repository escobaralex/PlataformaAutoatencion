using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebApi_BFC.DBUtils;
using WebApi_BFC.POCOS;

namespace WebApi_BFC.Models.bfc
{
    public class ArticuloModels
    {
        public List<Articulo> GetStockArticulos(string codigo, string bodega)
        {
            List<Articulo> result = null;
            Articulo articulo = null;
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            
            if (codigo == null || codigo == string.Empty)
            {
                if (bodega == null || bodega == string.Empty)
                {
                    dsArticulos = su.GetXmlQueryToDataSet("GetStockArticulos");
                }
                else
                {
                    dsArticulos = su.GetXmlQueryToDataSet("GetStockArticulosEnBodega",
                        new string[1] { bodega });
                }                
            }
            else
            {
                if (bodega == null || bodega == string.Empty)
                {
                    dsArticulos = su.GetXmlQueryToDataSet("GetStockArticulo", new string[1] { codigo });
                }
                else
                {
                    dsArticulos = su.GetXmlQueryToDataSet("GetStockArticuloEnBodega",
                        new string[2] { bodega,codigo });
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
        public List<Articulo> GetArticulos(string param)
        {
            List<Articulo> result = null;
            Articulo articulo = null;
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            dsArticulos = su.GetXmlQueryToDataSet("SearchArticulos", new string[1] { param });
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
    }
}