using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.POCOS
{
    public class TarjetaExistencia
    {
        public EncabezadoTarjetaExistencia encabezado { get; set; }
        public List<DetalleTarjetaExistencia> detalles { get; set; }
    }
    public class EncabezadoTarjetaExistencia
    {
        public Articulo Articulo { get; set; }
        public int Anno { get; set; }
        public int Mes { get; set; }
    }
    public class DetalleTarjetaExistencia
    {
        public int Dia { get; set; }
        public Bodega Bodega { get; set; }
        public int NroDocumento { get; set; }
        public int Cantidad { get; set; }
        public int Movimiento { get; set; }// 1 = Ingreso, 2 = egreso
        public int Saldo { get; set; }
        public string Referencia { get; internal set; }
        public int NroReferencia { get; internal set; }
    }
}