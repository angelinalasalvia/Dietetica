using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_013AL.Composite
{
    public class Permiso_013AL : Rol_013AL
    {
        
        public override void AgregarHijo_013AL(Rol_013AL comp)
        {
        }

        public override void QuitarHijo_013AL(Rol_013AL comp)
        {
        }

        public override List<Rol_013AL> ObtenerHijos_013AL()
        {
            return new List<Rol_013AL>();
        }

    }

}
