using Api.Infraestructure;
using Api.Providers;
using log4net;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http;

[assembly: OwinStartup(typeof(Api.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Api
{
    public class Startup
    {
        private static string URL_API = System.Configuration.ConfigurationManager.AppSettings["URL_API"];
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Configuration(IAppBuilder app)
        {
            //Log.Debug("Inicio Configuration Startup");
            HttpConfiguration httpConfig = new HttpConfiguration();            
            ConfigureOAuthTokenGeneration(app);
            ConfigureOAuthTokenConsumption(app);
            ConfigureWebApi(httpConfig);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(httpConfig);
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            //Log.Debug("Inicio ConfigureOAuthTokenGeneration Startup");
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<UserManager>(UserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Plugin the OAuth bearer JSON Web Token tokens generation and Consumption will be here
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions();

            //For Dev enviroment only (on production should be AllowInsecureHttp = false)
            OAuthServerOptions.AllowInsecureHttp = true;
            OAuthServerOptions.TokenEndpointPath = new PathString("/oauth/token");
            OAuthServerOptions.AccessTokenExpireTimeSpan = TimeSpan.FromDays(1);
            OAuthServerOptions.Provider = new CustomOAuthProvider();
            OAuthServerOptions.AccessTokenFormat = new CustomJwtFormat(URL_API);
            

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }
        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            
            var issuer = URL_API;
            //Log.Debug("Inicio ConfigureOAuthTokenConsumption Startup, issuer:" + issuer);
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    },                    
                });
        }
        private void ConfigureWebApi(HttpConfiguration config)
        {
            //Log.Debug("Inicio ConfigureWebApi Startup");
            config.MapHttpAttributeRoutes();
            
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}