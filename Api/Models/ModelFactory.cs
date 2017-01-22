using Api.Infraestructure;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace Api.Models
{
    public class ModelFactory
    {
        private UrlHelper _UrlHelper;
        private UserManager _UserManager;

        public ModelFactory(HttpRequestMessage request, UserManager userManager)
        {
            _UrlHelper = new UrlHelper(request);
            _UserManager = userManager;
        }

        public UserReturnModel Create(User user)
        {
            return new UserReturnModel
            {
                Url = _UrlHelper.Link("GetUserById", new { id = user.Id }),
                Id = user.Id,
                UserName = user.UserName,
                FullName = string.Format("{0} {1}", user.Nombres, user.Apellidos),
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Empresa = user.Empresa,
                IsActivo = user.IsActivo,
                Roles = _UserManager.GetRolesAsync(user.Id).Result,
                Claims = _UserManager.GetClaimsAsync(user.Id).Result
            };
        }
        public RoleReturnModel Create(IdentityRole appRole)
        {

            return new RoleReturnModel
            {
                Url = _UrlHelper.Link("GetRoleById", new { id = appRole.Id }),
                Id = appRole.Id,
                Name = appRole.Name
            };
        }
    }
    public class UserReturnModel
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public int Empresa { get; set; }
        public bool IsActivo { get; set; }
        public IList<string> Roles { get; set; }
        public IList<System.Security.Claims.Claim> Claims { get; set; }
    }
    public class RoleReturnModel
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}