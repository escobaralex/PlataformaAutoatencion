using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi_BFC.Models.bfc;
using WebApi_BFC.POCOS;

namespace WebApi.BC.Controllers.bfc
{
    public class BodegaController : ApiController
    {
        // GET: api/Bodega
        public IEnumerable<Bodega> Get()
        {
            BodegaModels bm = new BodegaModels();
            List<Bodega> bodegas = bm.GetBodegas();
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
