using Api.Infraestructure;
using log4net;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Api.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
       {
            //context.OwinContext.Set<string>("as:clientAllowedOrigin", "*");
            //Log.Debug("ValidateClientAuthentication");
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //Log.Debug("GrantResourceOwnerCredentials");
            var allowedOrigin = "*";
            if (!context.OwinContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            }            

            var userManager = context.OwinContext.GetUserManager<UserManager>();

            User user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "El nombre de usuario o contraseña es incorrecto.");
                return;
            }
            
            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "El usuario no ha confirmado su email.");
                return;
            }
            //var manager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            Log.Info("Inicio de Sesión: " + user.UserName);
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
            oAuthIdentity.AddClaim(new Claim("apellidos", user.Apellidos));
            oAuthIdentity.AddClaim(new Claim("empresa", user.Empresa.ToString()));
            oAuthIdentity.AddClaim(new Claim("nombres", user.Nombres));
            oAuthIdentity.AddClaim(new Claim("email", user.Email));
            if (user.Roles != null && user.Roles.Count > 0)
            {
                try
                {
                    string rol = oAuthIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()[0];
                    oAuthIdentity.AddClaim(new Claim("rol", rol));
                }
                catch (Exception ex)
                {
                    Log.Error("Error claims obtener rol",ex);
                }
                
            }
            //var props = new AuthenticationProperties(new Dictionary<string, string>
            //    {
            //        {
            //            "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
            //        },
            //        {
            //            "userName", context.UserName
            //        }
            //    });
            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            context.Validated(ticket);

        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}