using Api.Models.bfc;
using Api.POCOS;
using System;
using System.Collections.Generic;

namespace Api.Pocos
{
    public class MovimientoExistencia
    {
        public Encabezado Encabezado {get;set ;}
        public List<Detalle> Detalles { get; set; }
    }
    public class Encabezado
    {
        public int Movimiento { get; set; }
        public int NroDocumento { get; set; }
        public System.DateTime FechaEmision { get; set; }
        public string Observacion { get; set; }
        public Bodega Bodega { get; set; }
        public DocumentoReferencia referencia { get; set; }
        public int nroReferencia { get; set; }
    }
    public class Detalle
    {
        public string Codigo { get; set; }
        public Bodega Bodega { get; set; }
        public double Cantidad { get; set; }
        public double ValorUnitario { get; set; }
        public string Descripcion { get; set; }
    }
}
