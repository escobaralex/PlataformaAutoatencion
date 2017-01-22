using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.POCOS
{

    public class Usuario
    {
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public Empresa Empresa { get; set; }
        public Rol Rol { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public bool IsActivo { get; set; }
        public string Celular { get; set; }
        public string Direccion { get; set; }
        public string Contrasena { get; set; }
        public string ConfirmarContrasena { get; set; }
        //public FormaCobro FormaCobro { get; set; }
    }
}