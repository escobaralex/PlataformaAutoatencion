using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebApi_BFC.DBUtils;
using WebApi_BFC.POCOS;

namespace WebApi_BFC.Models.bfc
{
    public class BodegaModels
    {
        public List<Bodega> GetBodegas()
        {
            List<Bodega> result = null;
            Bodega bodega = null;
            DataSet dsBodega = null;
            SqlUtils su = new SqlUtils();

            dsBodega = su.GetXmlQueryToDataSet("GetBodegas");
            
            if (DataSetUtils.hasRows(dsBodega))
            {
                result = new List<Bodega>();
                foreach (DataRow dr in dsBodega.Tables[0].Rows)
                {
                    bodega = new Bodega();
                    bodega.Descripcion = dr["tb_destab"].ToString();
                    bodega.Codigo = dr["tb_codtab"].ToString();
                                        
                    result.Add(bodega);
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