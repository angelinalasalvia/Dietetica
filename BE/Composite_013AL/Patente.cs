using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Composite
{
    public class Patente : Permiso
    {
        public override bool PuedeAcceder(string NombrePermiso)
        {
            return NombrePermiso == Nombre;
        }
    }
}
