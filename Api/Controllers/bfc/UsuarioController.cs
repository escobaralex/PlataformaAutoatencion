using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Api.POCOS;
using Api.Infraestructure;
using System.Collections;
using Api.Models.bfc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Api.Models;

namespace Api.Controllers.bfc
{
    [RoutePrefix("api/Usuario")]
    public class UsuarioController : BaseController
    {
        // GET: api/Usuario
        [Authorize]
        [HttpGet]
        [Route("get")]
        public Usuario Get()
        {
            Usuario u = new Usuario();
            //u.Nombres = "Alex Escobar";
            //u.Empresa = "Tadis";
            //u.IdEmpresa = "001";
            //u.Rol = "Admin";
            return u;
        }

        // GET: api/Usuario/5
        public string Get(int id)
        {
            return "value";
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]        
        [Route("ObtenerAll")]
        public IEnumerable<Usuario> ObtenerAll()
        {
            UsuarioModels um = new UsuarioModels();
            return um.getAll(this.GetIdEmpresa(new ClaimsIdentity(User.Identity))).AsEnumerable();
            //using (var ctx = new ApplicationDbContext())
            //{
            //    try
            //    {
            //        //return ctx.Users.ToList();

            //        return ctx.Usuarios.ToList();
            //    }
            //    catch (Exception)
            //    {
            //        //return InternalServerError();
            //    }
            //}

            //List<Usuario> usuarios = new List<Usuario>();
            //Usuario u = new Usuario();
            //u.Nombre = "Alex Escobar";
            //u.Empresa = "Tadis";
            //u.IdEmpresa = "001";
            //u.Rol = "Admin";
            //usuarios.Add(u);
            //usuarios.Add(u);
            //usuarios.Add(u);
            //usuarios.Add(u);
            //return usuarios;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Post(Usuario usuario)
        {
            var user = new User();
            try
            {
                user.Nombres = usuario.Nombres;
                user.Apellidos = usuario.Apellidos;
                user.UserName = usuario.Email;
                user.Email = usuario.Email;
                user.Empresa = usuario.Empresa.Id;
                user.IsActivo = usuario.IsActivo;
                user.PhoneNumber = usuario.Telefono;
                user.Celular = usuario.Celular;
                user.Direccion = usuario.Direccion;
                user.EmailConfirmed = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al crear usuario, error de parseo", ex);
                return InternalServerError(ex);
            }
            
            IdentityResult addUserResult = await this.UserManager.CreateAsync(user, usuario.Contrasena);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }
            this.UserManager.AddToRoles(user.Id, new string[] { usuario.Rol.Name });
            //Uri locationHeader = new Uri(Url.Link("Create", new { id = user.Id }));

            return Created("Create", TheModelFactory.Create(user));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Update")]
        public async Task<IHttpActionResult> Put(Usuario usuario)
        {
            var user = this.UserManager.FindById(usuario.Id);
            try
            {
                user.Nombres = usuario.Nombres;
                user.Apellidos = usuario.Apellidos;
                user.UserName = usuario.Email;
                user.Email = usuario.Email;
                user.Empresa = usuario.Empresa.Id;
                user.IsActivo = usuario.IsActivo;
                user.PhoneNumber = usuario.Telefono;
                user.Celular = usuario.Celular;
                user.Direccion = usuario.Direccion;
                user.EmailConfirmed = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error al actualizar usuario", ex);
                return InternalServerError(ex);
            }

            try
            {

                IdentityResult removeResult = await this.UserManager.RemoveFromRolesAsync(user.Id, new string[3] { "Admin", "Operador", "Cliente" });
                if (!removeResult.Succeeded)
                {
                    if (removeResult.Errors.ToArray()[0] != "User is not in role.")
                    {
                        return GetErrorResult(removeResult);
                    }
                }
                IdentityResult reasigResult = this.UserManager.AddToRoles(user.Id, new string[] { usuario.Rol.Name });
                if (!reasigResult.Succeeded)
                {
                    if (reasigResult.Errors.ToArray()[0] != "User already in role.")
                    {
                        return GetErrorResult(reasigResult);
                    }
                }
                if (usuario.Contrasena != null && usuario.Contrasena.Trim() != string.Empty &&
                    usuario.Contrasena == usuario.ConfirmarContrasena)
                {
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(usuario.Contrasena);
                    var result = await UserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }

                    //IdentityResult result = await this.UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);

                    //if (!result.Succeeded)
                    //{
                    //    return GetErrorResult(result);
                    //}
                }
                else
                {
                    var result = await UserManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }
                }
                //ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
                //ctx.SaveChanges();
                return Ok(TheModelFactory.Create(user));
            }
            catch (Exception ex)
            {
                Log.Error("Error al actualizar usuario", ex);
                return InternalServerError(ex);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("remover")]
        public async Task<IHttpActionResult> Delete(Usuario usuario)
        {
            var user = await this.UserManager.FindByIdAsync(usuario.Id);

            if (user != null)
            {
                var r = await this.UserManager.DeleteAsync(user);
                if (r.Succeeded)
                {
                    UsuarioModels um = new UsuarioModels();
                    return Ok(um.getAll(this.GetIdEmpresa(new ClaimsIdentity(User.Identity))).AsEnumerable());
                }
                else
                {                                        
                    return InternalServerError();
                }                
                
            }
            return NotFound();            
        }
    }
}
