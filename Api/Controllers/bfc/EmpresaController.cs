using Api.Infraestructure;
using Api.Models.bfc;
using Api.POCOS;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers.bfc
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/empresa")]
    public class EmpresaController : BaseController
    {
        [Authorize(Roles = "Admin,Operador")]
        [HttpGet]
        [Route("get")]        
        public IEnumerable<Empresa> GetEmpresas()
        {
            EmpresaModels em = new EmpresaModels();
            return em.GetEmpresas().AsEnumerable();
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("change")]
        public IHttpActionResult changes(JObject filter)
        {
            Int32 id = filter["id"].ToObject<Int32>();
            //var user =  User.Identity.GetUserId();
            //user = User.Identity.GetUserName();
            //var Identity = new ClaimsIdentity(User.Identity);
            //var esteusuario = this.User;
            //if (Identity != null)
            //{                
            //    Identity.RemoveClaim(Identity.FindFirst("empresa"));
            //    Identity.AddClaim(new Claim("empresa", id.ToString()));
            //    return Ok();
            //}            
            //else
            //{
            //    return InternalServerError();
            //}
            var UserName =  User.Identity.GetUserName();
            using (var ctx = new ApplicationDbContext())
            {
                try
                {
                    User uUpd = ctx.Users.Where(u => u.UserName == UserName).FirstOrDefault<User>(); ;
                    uUpd.Empresa = id;

                    ctx.Entry(uUpd).State = System.Data.Entity.EntityState.Modified;
                    
                    ctx.SaveChanges();
                    return Ok();
                }
                catch (Exception ex)
                {
                    Log.Error("Error al cambiar usuario de empresa, username: " + UserName,ex);
                    return InternalServerError();
                }                
            }
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpGet]
        [Route("formascobro")]
        public IEnumerable<FormaCobro> GetFormasCobro()
        {
            FormaCobroModels fm = new FormaCobroModels();
            return fm.GetFormaCobro().AsEnumerable();
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpGet]
        [Route("GetDocumentosReferencia")]
        public IHttpActionResult GetDocumentosReferencia()
        {
            List<Api.Models.bfc.DocumentoReferencia> DocumentoReferencias = new List<Api.Models.bfc.DocumentoReferencia>();
            using (var ctx = new WEB_BevfoodCenterEntities())
            {
                try
                {
                    DocumentoReferencias = ctx.DocumentoReferencia.ToList();
                }
                catch (Exception ex)
                {
                    Log.Error("Error al intentar obtener listado de documentos de referencia", ex);
                    InternalServerError(ex);
                }
            }
            return Ok(DocumentoReferencias);
        }
        
    }
}
