using Newtonsoft.Json.Linq;
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
using Api.Pocos;
using System.Data;
using Api.DBUtils;

namespace Api.Controllers.bfc
{
    [RoutePrefix("api/Stock")]
    public class StockController : BaseController
    {
        // POST: api/Stock
        /// <summary>
        /// Obtiene Stock de Articulos
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Post(JObject filter)
        {
            Articulo articulo = null;
            Bodega bodega = null;
            List<Articulo> articulos = null;
            try
            {
                articulo = filter["articulo"].ToObject<Articulo>();
                bodega = filter["bodega"].ToObject<Bodega>();
            }
            catch (Exception ex)
            {
                string sLog = "Error al intentar obtener stock articulos.  Filtro: " + filter.ToString()
                    + ", Usuario: " + this.User.Identity.Name;
                Log.Error(sLog, ex);
                return BadRequest(sLog);
            }
            
            ArticuloModels sm = new ArticuloModels();
            articulos = sm.GetStockArticulos(articulo.Codigo, bodega.Codigo > 0 ? bodega.Codigo.ToString() : "", this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
            if (articulo == null || articulos.Count == 0)
            {
                return Ok();
            }
            else
            {
                return Ok(articulos);
            }       
        }

        [Authorize]
        [HttpPost]
        [Route("GetEntradasSalidas")]
        public IHttpActionResult GetEntradasSalidas(JObject filter)
        {
            Articulo articulo = null;
            Bodega bodega = null;
            Informe result = null;
            int anno = 0;
            try
            {
                articulo = filter["articulo"].ToObject<Articulo>();
                bodega = filter["bodega"].ToObject<Bodega>();
                anno = filter["anno"].ToObject<Int32>();

                ArticuloModels sm = new ArticuloModels();
                result = sm.GetEntradasSalidas(articulo.Codigo, bodega.Codigo.ToString(), anno, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
                if (result == null)
                {
                    return InternalServerError(new Exception("Ha ocurrido un error al intentar obtener el stock de entradas y salidas"));
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                string sLog = "Error al intentar obtener stock articulos.  Filtro: " + filter.ToString()
                    + ", Usuario: " + this.User.Identity.Name;
                Log.Error(sLog, ex);
                return InternalServerError(ex);
            }            
        }

        [Authorize]
        [HttpPost]
        [Route("GetDetallesEntradasSalidas")]
        public IHttpActionResult GetDetallesEntradasSalidas(JObject filter)
        {
            Articulo articulo = null;
            Bodega bodega = null;
            TarjetaExistencia result = null;
            int anno = 0;
            int mes = 0;
            int saldo = 0;
            try
            {
                articulo = filter["articulo"].ToObject<Articulo>();
                bodega = filter["bodega"].ToObject<Bodega>();
                anno = filter["anno"].ToObject<Int32>();
                mes = filter["mes"].ToObject<Int32>();
                saldo = filter["saldo"].ToObject<Int32>();

                ArticuloModels sm = new ArticuloModels();
                result = sm.GetDetalleEntradasSalidas(articulo.Codigo, bodega.Codigo.ToString(), anno,mes, saldo,this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
                if (result == null)
                {
                    return InternalServerError(new Exception("Ha ocurrido un error al intentar obtener el stock de entradas y salidas"));
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                string sLog = "Error al intentar obtener stock articulos.  Filtro: " + filter.ToString()
                    + ", Usuario: " + this.User.Identity.Name;
                Log.Error(sLog, ex);
                return InternalServerError(ex);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("GetListadoEntradasSalidas")]
        public IHttpActionResult GetListadoEntradasSalidas(JObject filter)
        {
            Articulo articulo = null;
            string result = string.Empty;
            JToken desde; JToken hasta;
            try
            {
                articulo = filter["articulo"].ToObject<Articulo>();

                filter.TryGetValue("desde", out desde);
                filter.TryGetValue("hasta", out hasta);
                if (desde == null || hasta == null)
                {
                    return BadRequest("parametros desde-hasta son obligatorios");
                }

                ArticuloModels sm = new ArticuloModels();

                DateTime d = desde.ToObject<DateTime>();
                DateTime h = hasta.ToObject<DateTime>();

                result = sm.GetListadoEntradasSalidas(articulo.Codigo, d, h, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
                if (result == null)
                {
                    return InternalServerError(new Exception("Ha ocurrido un error al intentar obtener el stock de entradas y salidas"));
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                string sLog = "Error al intentar obtener stock articulos.  Filtro: " + filter.ToString()
                    + ", Usuario: " + this.User.Identity.Name;
                Log.Error(sLog, ex);
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("CreateMovimientoExistencia")]
        public async Task<IHttpActionResult> CreateMovimientoExistencia(MovimientoExistencia movimientoExistencia)
        {
            if (movimientoExistencia == null)
            {
                return BadRequest("El Movimiento de existencia es nulo");
            }
            int result = 0;
            StockModels sm = new StockModels();
            try
            {
                result = sm.CreateMovimientoExistencia(movimientoExistencia, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
                if (result > 0)
                {
                    return Ok(result);
                }
                return InternalServerError(new Exception("Error inesperado al intentar crear movimiento de existencia"));
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear movimiento de existencia", ex);
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("UpdateMovimientoExistencia")]
        public async Task<IHttpActionResult> UpdateMovimientoExistencia(MovimientoExistencia movimientoExistencia)
        {
            if (movimientoExistencia == null)
            {
                return BadRequest("El Movimiento de existencia es nulo");
            }
            bool result = false;
            StockModels sm = new StockModels();
            try
            {
                result = sm.UpdateMovimientoExistencia(movimientoExistencia, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
                if (result)
                {
                    return Ok(result);
                }
                return InternalServerError(new Exception("Error inesperado al intentar actualizar movimiento de existencia"));
            }
            catch (Exception ex)
            {
                Log.Error("Error al actualizar movimiento de existencia", ex);
                return InternalServerError(ex);
            }
        }


        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("GetMovimientoExistencia")]
        public async Task<IHttpActionResult> GetMovimientoExistencia(Encabezado encabezado)
        {
            if (encabezado == null)
            {
                return BadRequest("El Movimiento de existencia es nulo");
            }
            List<MovimientoExistencia> movimientos = new List<Pocos.MovimientoExistencia>();
            StockModels sm = new StockModels();
            try
            {
                movimientos = sm.GetMovimientoExistencia(encabezado, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return Ok(movimientos);
        }


        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("GetDetalleMovimientoExistencia")]
        public async Task<IHttpActionResult> GetDetalleMovimientoExistencia(Encabezado encabezado)
        {
            if (encabezado == null)
            {
                return BadRequest("El Movimiento de existencia es nulo");
            }
            MovimientoExistencia movimiento = new MovimientoExistencia();
            StockModels sm = new StockModels();
            try
            {
                movimiento = sm.GetDetalleMovimientoExistencia(encabezado, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
                movimiento.Encabezado = encabezado;
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return Ok(movimiento);
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpGet]
        [Route("GetAllMovimientoExistencia")]
        public async Task<IHttpActionResult> GetAllMovimientoExistencia()
        {
            StockModels sm = new StockModels();
            //List<Articulo> articulos = sm.GetStockArticulos(movimientoExistencia, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
            return InternalServerError();
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("DeleteMovimientoExistencia")]
        public async Task<IHttpActionResult> DeleteMovimientoExistencia(MovimientoExistencia movimientoExistencia)
        {
            if (movimientoExistencia == null)
            {
                return BadRequest("El Movimiento de existencia es nulo");
            }
            bool result = false;
            StockModels sm = new StockModels();
            try
            {
                result = sm.DeleteMovimientoExistencia(movimientoExistencia, this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
                if (result)
                {
                    return Ok(result);
                }
                return InternalServerError(new Exception("Error inesperado al intentar eliminar movimiento de existencia"));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }            
        }
    }
}

