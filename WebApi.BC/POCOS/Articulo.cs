using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_BFC.POCOS
{
    public class Articulo
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string UnidadDeMedida { get; set; }
        public string Stock { get; set; }
    }
}