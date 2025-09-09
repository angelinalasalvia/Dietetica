using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_013AL.Composite
{
    public class Permiso_013AL : Componente_013AL
    {
        /*public override int Id { get; set; }
        public override string Nombre { get; set; }
        public override string Tipo { get; set; }*/

        public override void AgregarHijo_013AL(Componente_013AL comp)
        {
        }

        public override void QuitarHijo_013AL(Componente_013AL comp)
        {
        }

        public override List<Componente_013AL> ObtenerHijos_013AL()
        {
            return new List<Componente_013AL>();
        }

    }

}
