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
        public List<Articulo> GetArticulos()
        {
            List<Articulo> result = null;
            Articulo articulo = null;
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            dsArticulos = su.GetXmlQueryToDataSet("GetArticulos");
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
        public List<Articulo> GetArticulos(string param)
        {
            List<Articulo> result = null;
            Articulo articulo = null;
            DataSet dsArticulos = null;
            SqlUtils su = new SqlUtils();
            dsArticulos = su.GetXmlQueryToDataSet("SearchArticulos", param );
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