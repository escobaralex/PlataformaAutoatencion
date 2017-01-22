using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.POCOS
{
    public class EmpresaFormaCobro
    {
        public int IdEmpresa { get; set; }
        public int IdFormaCobro { get; set; }
        public double Valor { get; set; }
    }
}