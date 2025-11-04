using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Traduccion_013AL
    {
        public int IdTraduccion_013AL { get; set; }
        public int IdIdioma_013AL { get; set; }
        public int IdEtiqueta_013AL { get; set; }
        public string Texto_013AL { get; set; }

        public Etiqueta_013AL Etiqueta_013AL { get; set; }
    }
}
