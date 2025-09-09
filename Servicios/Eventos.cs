using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class Eventos
    {
        public int CodEvento { get; set; }
        public string Login { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Modulo { get; set; }
        public string Evento { get; set; }
        public int Criticidad { get; set; }

    }
}
