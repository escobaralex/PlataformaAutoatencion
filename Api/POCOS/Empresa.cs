using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.POCOS
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int AnoAct { get; set; }
        public int MesAct { get; set; }
        public int IdFormaCobro { get; set; }
        public string NombreFormaCobro { get; set; }
        public double Valor { get; set; }
    }
}