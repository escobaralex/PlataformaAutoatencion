using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.POCOS
{
    /// <summary>
    /// Clase para el manejo de los articulos
    /// </summary>
    public class Articulo
    {
        /// <summary>
        /// Código del Artículo
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// del Artículo
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// del Artículo
        /// </summary>
        public string UnidadDeMedida { get; set; }

        /// <summary>
        /// del Artículo
        /// </summary>
        public int UniXcaja { get; set; }

        /// <summary>
        /// del Artículo
        /// </summary>
        public int CXpallet { get; set; }

        /// <summary>
        /// del Artículo
        /// </summary>
        public double Largo { get; set; }

        /// <summary>
        /// del Artículo
        /// </summary>
        public double Alto { get; set; }
        
        /// <summary>
        /// del Artículo
        /// </summary>
        public double Ancho { get; set; }
               

        /// <summary>
        /// del Artículo
        /// </summary>
        public bool IsActivo { get; set; }

        ///// <summary>
        ///// del Artículo
        ///// </summary>
        //public string Observacion { get; set; }

        ///// <summary>
        ///// del Artículo
        ///// </summary>
        //public double Kilos { get; set; }

        /// <summary>
        /// del Artículo
        /// </summary>
        public string Stock { get; set; }
    }
}