using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class Paginacao
    {
        public int NumeroPagina { get; set; }
        public int RegistroPorPagina { get; set; }
        public int TotalResgistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}
