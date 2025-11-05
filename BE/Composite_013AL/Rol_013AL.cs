using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_013AL.Composite
{
    public abstract class Rol_013AL
    {
        public int Cod_013AL { get; set; }
        public string Nombre_013AL { get; set; }
        public string Tipo_013AL { get; set; }

        public abstract void AgregarHijo_013AL(Rol_013AL comp);
        public abstract void QuitarHijo_013AL(Rol_013AL comp);

        public abstract List<Rol_013AL> ObtenerHijos_013AL();

        public override string ToString()
        {
            return $"{Cod_013AL} - {Nombre_013AL} - {Tipo_013AL}";
        }

    }
}
   