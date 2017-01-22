using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Api.Infraestructure
{
    public class UserManager : UserManager<User>
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public UserManager(IUserStore<User> store)
            : base(store)
        {
        }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            //Log.Debug("UserManager Create");
            //context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                //Log.Debug("UserManager Create en el if(...");
                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            }
            var appDbContext = context.Get<ApplicationDbContext>();
            var appUserManager = new UserManager(new UserStore<User>(appDbContext));

            // Configure validation logic for usernames
            appUserManager.UserValidator = new UserValidator<User>(appUserManager)
            {
                //AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                //RequireNonLetterOrDigit = true,
                //RequireDigit = false,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };
            //Log.Debug("UserManager Create FIN");
            return appUserManager;
        }
    }
}