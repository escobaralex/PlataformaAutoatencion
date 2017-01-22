using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Api.Models.bfc;
using Api.POCOS;
using System.Security.Claims;

namespace Api.Controllers.bfc
{
    [RoutePrefix("api/Bodega")]
    public class BodegaController : BaseController
    {

        [Authorize]
        [HttpGet]
        [Route("get")]
        public IEnumerable<Bodega> Get()
        {
            BodegaModels bm = new BodegaModels();
            List<Bodega> bodegas = bm.GetBodegas(this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
            return bodegas;
        }

        // GET: api/Bodega/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Bodega
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Bodega/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Bodega/5
        public void Delete(int id)
        {
        }
    }
}
