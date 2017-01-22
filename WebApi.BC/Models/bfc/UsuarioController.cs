using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.BC.POCOS;

namespace WebApi.BC.Models.bfc
{
    public class UsuarioController : ApiController
    {
        // GET: api/Usuario
        public Usuario Get()
        {
            Usuario u = new Usuario();
            u.Nombre = "Alex Escobar";
            u.Empresa = "Tadis";
            u.IdEmpresa = "001";
            u.Rol = "Admin";
            return u;
        }

        // GET: api/Usuario/5
        public string Get(int id)
        {
            return "value";
        }

        public IEnumerable<Usuario> ObtenerAll()
        {
            List<Usuario> usuarios = new List<Usuario>();
            Usuario u = new Usuario();
            u.Nombre = "Alex Escobar";
            u.Empresa = "Tadis";
            u.IdEmpresa = "001";
            u.Rol = "Admin";
            usuarios.Add(u);
            usuarios.Add(u);
            usuarios.Add(u);
            usuarios.Add(u);
            return usuarios;
        }

        // POST: api/Usuario
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Usuario/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Usuario/5
        public void Delete(int id)
        {
        }
    }
}
