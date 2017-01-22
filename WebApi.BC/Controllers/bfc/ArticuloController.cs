using Microsoft.AspNet.Identity;
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
    //[RoutePrefix("api/Articulo")]
    public class ArticuloController : ApiController
    {
        // GET: api/Articulo
        [Authorize(Roles = "Admin")]
        public IEnumerable<Articulo> Get()
        {
            ArticuloModels am = new ArticuloModels();
            var user = System.Web.HttpContext.Current.User.Identity.GetUserId();
            return am.GetArticulos("").AsEnumerable(); ;
        }
        
        public IEnumerable<Articulo> Post(Articulo articulo)
        {
            ArticuloModels am = new ArticuloModels();
            var user = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<Articulo> articulos = am.GetArticulos(articulo.Descripcion);
            return articulos;
        }


        //// GET: api/Articulo/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Articulo
        //public void Post([FromBody]string value)
        //{
        //}

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
