namespace Api.Migrations
{
    using Infraestructure;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;
    internal sealed class Configuration : DbMigrationsConfiguration<Api.Infraestructure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Api.Infraestructure.ApplicationDbContext context)
        {
            var manager = new UserManager<User>(new UserStore<User>(new ApplicationDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            if (roleManager.Roles.Count() == 0)
            {                
                roleManager.Create(new IdentityRole { Name = "Admin" });
                roleManager.Create(new IdentityRole { Name = "Operador" });
                roleManager.Create(new IdentityRole { Name = "Cliente" });
            }
            try
            {
                var user = new User()
                {
                    UserName = "aescobar@tadis.cl",
                    Email = "aescobar@tadis.cl",
                    EmailConfirmed = true,
                    Nombres = "Alex",
                    Apellidos = "Escobar",
                    IsActivo = true,
                    Empresa = 1
                };
                manager.Create(user, "AlexEscobar");
                var adminUser = manager.FindByName(user.UserName);
                if (adminUser != null)
                {
                    manager.AddToRoles(adminUser.Id, new string[] { "Admin" });
                }
                
                var user2 = new User()
                {
                    UserName = "cmunro@bevfoodcenter.cl",
                    Email = "cmunro@bevfoodcenter.cl",
                    EmailConfirmed = true,
                    Nombres = "Christopher",
                    Apellidos = "Munro",
                    IsActivo = true,
                    Empresa = 1
                };
                manager.Create(user2, "bfc_2016");
                var adminUser2 = manager.FindByName(user2.UserName);
                manager.AddToRoles(adminUser2.Id, new string[] { "Admin" });
                
                var operador = new User()
                {
                    UserName = "operador@bevfoodcenter.cl",
                    Email = "operador@bevfoodcenter.cl",
                    EmailConfirmed = true,
                    Nombres = "Operador",
                    Apellidos = " BF",
                    IsActivo = true,
                    Empresa = 1
                };
                manager.Create(operador, "operador_2016");
                var operadorU = manager.FindByName(operador.UserName);
                manager.AddToRoles(operadorU.Id, new string[] { "Operador" });
                
                var cliente = new User()
                {
                    UserName = "cliente@silfa.cl",
                    Email = "cliente@silfa.cl",
                    EmailConfirmed = true,
                    Nombres = "Cliente",
                    Apellidos = "Silfa",
                    IsActivo = true,
                    Empresa = 1
                };
                manager.Create(cliente, "cliente_2016");
                var clienteU = manager.FindByName(cliente.UserName);
                manager.AddToRoles(clienteU.Id, new string[] { "Cliente" });
                /**/
                
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException dbValEx)
                {
                    var outputLines = new StringBuilder();
                    foreach (var eve in dbValEx.EntityValidationErrors)
                    {
                        outputLines.AppendFormat("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:"
                          , DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State);

                        foreach (var ve in eve.ValidationErrors)
                        {
                            outputLines.AppendFormat("- Property: \"{0}\", Error: \"{1}\""
                             , ve.PropertyName, ve.ErrorMessage);
                        }
                    }

                    throw new DbEntityValidationException(string.Format("Validation errors\r\n{0}"
                     , outputLines.ToString()), dbValEx);
                }
            }
            catch (Exception)
            {

                
            }
            
        }
    }
}
