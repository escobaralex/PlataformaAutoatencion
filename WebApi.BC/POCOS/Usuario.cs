using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.BC.POCOS
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string Empresa { get; set; }
        public string IdEmpresa { get; set; }
        public string Rol { get; set; }
    }
}