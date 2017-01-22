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
    [RoutePrefix("api/Articulo")]
    public class ArticuloController : ApiController
    {
        // GET: api/Articulo        
        public IEnumerable<Articulo> Get()
        {
            ArticuloModels am = new ArticuloModels();
            List<Articulo> articulos = am.GetArticulos();
            return articulos;
        }

        // POST: api/Articulo/Search
        [Route("Search")]
        public IEnumerable<Articulo> Search(string param)
        {
            ArticuloModels am = new ArticuloModels();
            List<Articulo> articulos = am.GetArticulos(param);
            return articulos;
        }
        public IEnumerable<Articulo> Get(string param)
        {
            ArticuloModels am = new ArticuloModels();
            List<Articulo> articulos = am.GetArticulos(param);
            return articulos;
        }
        // GET: api/Articulo/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Articulo
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Articulo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Articulo/5
        public void Delete(int id)
        {
        }
    }
}
