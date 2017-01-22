using Api.Infraestructure;
using Api.Models;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Api.Controllers
{
    public class BaseController : ApiController
    {
        public BaseController()
        {
        }

        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ModelFactory _modelFactory;
        private UserManager _UserManager = null;
        private ApplicationRoleManager _AppRoleManager = null;

        protected ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request, this.UserManager);
                }
                return _modelFactory;
            }
        }

        protected UserManager UserManager
        {
            get
            {
                return _UserManager ?? Request.GetOwinContext().GetUserManager<UserManager>();
            }
        }
        protected int GetIdEmpresa(ClaimsIdentity Identity)
        {
            int empresa = 0;
            var UserName = Identity.GetUserName();
            using (var ctx = new ApplicationDbContext())
            {
                try
                {
                    User uUpd = ctx.Users.Where(u => u.UserName == UserName).FirstOrDefault<User>(); ;
                    empresa = uUpd.Empresa;
                }
                catch (Exception ex)
                {
                    Log.Error("Error al obtener id de la empresa",ex);
                }
            }
            return empresa;
        }
        protected ApplicationRoleManager AppRoleManager
        {
            get
            {
                return _AppRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }
        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {   
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
