using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Models
    {
        public class Primero
        {
            
            public string Tag { get; set; }
        }

        public class Segundo
        {
            public long Id { get; set; }
            public string NumTag { get; set; }
            public double Saldo { get; set; }
            public double saldoViejo { get; set; }
            public double saldoNuevo { get; set; }
        }

    }
}
