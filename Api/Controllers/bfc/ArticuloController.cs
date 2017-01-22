using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Api.Models.bfc;
using Api.POCOS;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers.bfc
{
    //[RoutePrefix("api/Articulo")]
    [RoutePrefix("api/Articulo")]
    public class ArticuloController : BaseController
    {
        // GET: api/Articulo

        [Authorize]
        [HttpGet]
        [Route("get")]
        public IEnumerable<Articulo> Get()
        {
            ArticuloModels am = new ArticuloModels();
            // user = System.Web.HttpContext.Current.User.Identity.GetUserId();
           
            return am.GetArticulos("",this.GetIdEmpresa(new ClaimsIdentity(User.Identity))).AsEnumerable();
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("GetDetalleArticulo")]
        public IHttpActionResult getDetalleArticulo(Articulo articulo)
        {
            ArticuloModels am = new ArticuloModels();
            // user = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (am.GetDetalleArticulo(articulo, this.GetIdEmpresa(new ClaimsIdentity(User.Identity))) == null)
            {
                return InternalServerError();
            }
            return Ok(am.GetDetalleArticulo(articulo, this.GetIdEmpresa(new ClaimsIdentity(User.Identity))));
        }

        [Authorize]
        [HttpGet]
        [Route("getAll")]
        public IEnumerable<Articulo> GetAll()
        {
            ArticuloModels am = new ArticuloModels();
            // user = System.Web.HttpContext.Current.User.Identity.GetUserId();

            return am.GetAllArticulos("", this.GetIdEmpresa(new ClaimsIdentity(User.Identity))).AsEnumerable();
        }

        [Authorize]
        [HttpPost]
        [Route("getByCodigoDescripcion")]
        public IEnumerable<Articulo> getByCodigoDescripcion(Articulo articulo)
        {
            ArticuloModels am = new ArticuloModels();
            //var user = System.Web.HttpContext.Current.User.Identity.GetUserId();
            List<Articulo> articulos = am.GetArticulos(articulo.Descripcion, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
            return articulos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("GetUnidadesDeMedida")]        
        public IEnumerable<UnidadDeMedida> getUnidadesDeMedida()
        {
            ArticuloModels am = new ArticuloModels();
            return am.GetUnidadDeMedida(this.GetIdEmpresa(new ClaimsIdentity(User.Identity))).AsEnumerable();
        }
        /// <summary>
        /// Elimina un articulo de la Base de datos
        /// </summary>
        /// <param name="articulo"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("Remover")]
        public async Task<IHttpActionResult> remover(Articulo articulo)
        {
            if (articulo == null || articulo.Codigo == null)
            {
                return BadRequest("El Articulo o codigo es nulo");
            }
            ArticuloModels am = new ArticuloModels();
            if (am.Remover(articulo.Codigo,this.GetIdEmpresa(new ClaimsIdentity(User.Identity))))
            {                
                return Ok();
            }
            return InternalServerError();                        
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="articulo"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("VerificarPoderEliminar")]
        public async Task<IHttpActionResult> verificarPoderEliminar(Articulo articulo)
        {
            if (articulo == null || articulo.Codigo == null)
            {
                return BadRequest("El Articulo o codigo es nulo");
            }
            ArticuloModels am = new ArticuloModels();
            if (!am.ExisteMovExistencia(articulo.Codigo, this.GetIdEmpresa(new ClaimsIdentity(User.Identity))))
            {
                return Ok(true);
            }
            return Ok(false);
        }
        
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("update")]
        public async Task<IHttpActionResult> Put(Articulo articulo)
        {
            if (articulo == null || articulo.Codigo == null)
            {
                return BadRequest("El Articulo o codigo es nulo");
            }
            ArticuloModels am = new ArticuloModels();
            if (am.Update(articulo, this.GetIdEmpresa(new ClaimsIdentity(User.Identity))))
            {
                return Ok(true);
            }
            return Ok(false);
        }
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create(Articulo articulo)
        {            
            try
            {
                if (articulo == null || articulo.Codigo == null)
                {
                    return BadRequest("El Artículo o código es nulo");
                }
                ArticuloModels am = new ArticuloModels();
                // Si no existe lo crea
                if (!am.ExisteArticulo(articulo.Codigo, this.GetIdEmpresa(new ClaimsIdentity(User.Identity))))
                {
                    // Crea el Articulo
                    if (am.Create(articulo, this.GetIdEmpresa(new ClaimsIdentity(User.Identity))))
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return InternalServerError(new Exception("El artículo no pudo ser creado, consulte el registro de errores"));
                    }
                }
                else
                {
                    return BadRequest("El código de artículo ya existe");
                }                
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear articulo", ex);
                return InternalServerError(ex);
            }
        }
    }
}
