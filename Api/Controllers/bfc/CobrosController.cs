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
using Newtonsoft.Json.Linq;
using System.Data;
using Newtonsoft.Json;
using Api.DBUtils;

namespace Api.Controllers.bfc
{
    [RoutePrefix("api/Cobros")]
    public class CobrosController : BaseController
    {
        /// <summary>
        /// Obtiene un listado de los cobros de la empresa asociada al Usuario
        /// </summary>
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        [Route("GetCobrosCliente")]
        public async Task<IHttpActionResult> GetCobrosCliente()
        {

            CobroModels cm = new CobroModels();
            try
            {
                List<Cobro> cobros = null;
                cobros = cm.GetCobrosCliente("GetCobrosCliente", this.GetIdEmpresa(new ClaimsIdentity(User.Identity)));
                if (null == cobros)
                {
                    Log.Debug("Cobros no encontrados para cliente: " + User.Identity.Name);
                    return NotFound();
                }
                return Ok(cobros);
            }
            catch (Exception ex)
            {
                Log.Error("Error al intentar obtener cobros del cliente", ex);
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("GetDetalle")]
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

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("GetInformePreliminar")]
        public async Task<IHttpActionResult> GetInformePreliminar(Articulo articulo)
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
                Log.Error("Error al obtener informe preliminar", ex);
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("GenerarProcesoCobro")]
        public async Task<IHttpActionResult> GenerarProcesoCobro(Articulo articulo)
        {
            Log.Debug("Primera Prueba Log");
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
                Log.Error("Error al Generar Proceso de Cobro", ex);
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("GetValorUFMes")]
        public IHttpActionResult GetValorUFMes(ValorUF valor)
        {
            List<Api.Models.bfc.ValorUF> valorUF = new List<ValorUF>();
            using (var ctx = new ValorUFModel())
            {
                try
                {
                    valorUF = ctx.ValorUF.Where(v => v.Ano == valor.Ano && v.Mes == valor.Mes).ToList();
                }
                catch (Exception ex)
                {
                    Log.Error("Error al intentar obtener listado de documentos de referencia", ex);
                    InternalServerError(ex);
                }
            }
            return Ok(valorUF);
            /*{
            string mes = string.Empty;
            string ano = string.Empty;
            try
            {
                mes = filter["mes"].ToObject<string>();
                ano = filter["ano"].ToObject<string>();
            }
            catch (Exception ex)
            {
                string sLog = "Error al intentar obtener valor UF mes. filtro: " + filter.ToString()
                    + ", Usuario: " + this.User.Identity.Name;
                Log.Error(sLog, ex);
                return BadRequest(sLog);
            }
            CobroModels cm = new CobroModels();
            // user = System.Web.HttpContext.Current.User.Identity.GetUserId();
            //if (cm.GetDetalleArticulo(articulo, this.GetIdEmpresa(new ClaimsIdentity(User.Identity))) == null)
            //{
            //    return InternalServerError();
            //}
            //return Ok(cm.GetDetalleArticulo(articulo, this.GetIdEmpresa(new ClaimsIdentity(User.Identity))));
            return Ok();*/
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("SaveUF")]
        public IHttpActionResult SaveUF(ValorUF valor)
        {
            try
            {
                using (var ctx = new ValorUFModel())
                {
                    ctx.ValorUF.Add(valor);
                    ctx.SaveChanges();
                    return Ok();
                }                
            }
            catch (Exception ex)
            {
                Log.Error("Error al guardar los valor UF", ex);
                return InternalServerError(ex);
            }
        }
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("UpdValorUFMes")]
        public IHttpActionResult UpdValorUFMes(ValorUF valor)
        {
            
            using (var ctx = new ValorUFModel())
            {
                try
                {
                    var valorActualUF = ctx.ValorUF.Where(v => v.Ano == valor.Ano && v.Mes == valor.Mes).FirstOrDefault();
                    valorActualUF.Valor = valor.Valor;
                    ctx.Entry(valorActualUF).CurrentValues.SetValues(valor);
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    Log.Error("Error al intentar actualizar valor UF", ex);
                    InternalServerError(ex);
                }
            }
            return Ok();
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpGet]
        [Route("GetValoresCobrosEmpresa")]
        public async Task<IHttpActionResult> GetValoresCobrosEmpresa()
        {
            WEB_BevfoodCenterEntities db = new WEB_BevfoodCenterEntities();
            int idEmpresa = this.GetIdEmpresa(new ClaimsIdentity(User.Identity));
            var result = db.ValorCobroEmpresa.Where(v=>v.IdEmpresa == idEmpresa).FirstOrDefault();
            return Ok(result);           
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("SaveValoresCobrosEmpresa")]
        public async Task<IHttpActionResult> SaveValoresCobrosEmpresa(ValorCobroEmpresa valorCobroEmpresa)
        {
            int idEmpresa = this.GetIdEmpresa(new ClaimsIdentity(User.Identity));
            try
            {
                WEB_BevfoodCenterEntities db = new WEB_BevfoodCenterEntities();                
                valorCobroEmpresa.IdEmpresa = idEmpresa;
                db.ValorCobroEmpresa.Add(valorCobroEmpresa);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {                
                Log.Error("Error al guardar los valores de cobro de la empresa, ID: " + idEmpresa, ex);
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("UpdateValoresCobrosEmpresa")]
        public async Task<IHttpActionResult> UpdateValoresCobrosEmpresa(ValorCobroEmpresa valorCobroEmpresa)
        {

            WEB_BevfoodCenterEntities db = new WEB_BevfoodCenterEntities();
            int idEmpresa = this.GetIdEmpresa(new ClaimsIdentity(User.Identity));
            ValorCobroEmpresa aActualizar = db.ValorCobroEmpresa.Where(v => v.IdEmpresa == idEmpresa).First();

            if (aActualizar == null)
            {
                return InternalServerError(new Exception("Error al intentar "));
            }
            aActualizar.MetroCuadrado = valorCobroEmpresa.MetroCuadrado;
            aActualizar.MetroCubico = valorCobroEmpresa.MetroCubico;
            aActualizar.PosicionPalet = valorCobroEmpresa.PosicionPalet;
            aActualizar.ValorDefault = valorCobroEmpresa.ValorDefault;
            using (var dbCtx = new WEB_BevfoodCenterEntities())
            {
                //3. Mark entity as modified
                dbCtx.Entry(aActualizar).State = System.Data.Entity.EntityState.Modified;

                //4. call SaveChanges
                dbCtx.SaveChanges();
            }

            return Ok();
        }

        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [Route("GetCobroAlmacenamiento")]
        public IHttpActionResult GetCobroAlmacenamiento(JObject filter)
        {
            string mes = string.Empty;
            string ano = string.Empty;
            try
            {
                mes = filter["mes"].ToObject<string>();
                ano = filter["ano"].ToObject<string>();
            }
            catch (Exception ex)
            {
                string sLog = "Error al intentar obtener cobros de almacenaje.  Filtro: " + filter.ToString()
                    + ", Usuario: " + this.User.Identity.Name;
                Log.Error(sLog, ex);
                return BadRequest(sLog);
            }
            try
            {
                if (string.IsNullOrEmpty(mes) || string.IsNullOrEmpty(ano))
                {
                    var msgMes = "El mes y/o año a procesar cobros de almacenamiento es nulo o vacio.";
                    Log.Error(msgMes);
                    return BadRequest(msgMes);
                }
                CobroModels cm = new CobroModels();

                // Obtiene DataSet con el stock inicial
                DataSet dsDatosDeArrastre = cm.GetCobroAlmacenamiento(mes,ano,Connection.AgregaCeroIdEmpresa(this.GetIdEmpresa(new ClaimsIdentity(User.Identity))));
                if (dsDatosDeArrastre == null)
                {
                    return InternalServerError(new Exception("Ocurrio un error al obtener los datos de apertura de mes"));
                }
                if (dsDatosDeArrastre.Tables.Count == 0)
                    return InternalServerError(new Exception("Ocurrio un error al obtener información del mes."));
                if (DataSetUtils.hasRows(dsDatosDeArrastre))
                {
                    dsDatosDeArrastre.Tables[0].TableName = "DatosApertura";
                    dsDatosDeArrastre.Tables[1].TableName = "DatosDelMes";
                }
                decimal totalM2 = 0;
                decimal totalM3 = 0;
                decimal totalPP = 0;
                Dictionary<string, decimal> arrastre = new Dictionary<string, decimal>();

                // Recorrer el ds de apertura para obtener totales y articulos existentes

                foreach (DataRow dr in dsDatosDeArrastre.Tables[0].Rows)
                {
                    arrastre.Add(dr["LE_CODART"].ToString(),Convert.ToDecimal(dr["CANTIDAD"]));
                    totalM2 += Convert.ToDecimal(dr["M2"]);
                    totalM3 += Convert.ToDecimal(dr["M3"]);
                    totalPP += Convert.ToDecimal(dr["POSICION_PALET_REDONDEADO"]);
                }

                // AGREGAR COLUMNA DE CANTIDAD DE PALET POR DETALLE
                dsDatosDeArrastre.Tables[1].Columns.Add("PP_CALCULADO", typeof(Decimal));
                decimal finalTotalM2 = 0;
                decimal finalTotalM3 = 0;
                decimal finalTotalPP = 0;

                foreach (DataRow dr in dsDatosDeArrastre.Tables[1].Rows)
                {

                }

                string sTableArrastre = JsonConvert.SerializeObject(dsDatosDeArrastre);
                return Ok(sTableArrastre);
            }
            catch (Exception ex)
            {
                Log.Error("Error al obtener informe preliminar", ex);
                return InternalServerError(ex);
            }
        }
    }
}

