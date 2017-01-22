using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_BFC.Models.bfc;
using WebApi_BFC.POCOS;

namespace WebApi_BFC.Controllers.bfc
{
    public class StockController : ApiController
    {
        // GET: api/Stock
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Stock/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Stock
        public IEnumerable<Articulo> Post(JObject filter)
        {            
            Articulo articulo = filter["articulo"].ToObject<Articulo>();
            Bodega bodega = filter["bodega"].ToObject<Bodega>();
            ArticuloModels sm = new ArticuloModels();
            List<Articulo> articulos = sm.GetStockArticulos(articulo.Codigo, bodega.Codigo);
            return articulos;
        }

        // PUT: api/Stock/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Stock/5
        public void Delete(int id)
        {
        }
    }
}
