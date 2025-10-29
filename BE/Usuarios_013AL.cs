using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE_013AL.Composite;

namespace BE_013AL
{
    public class Usuarios_013AL 
    {
        public int Cod_013AL { get; set; }
        public string Email_013AL { get; set; }
        public string Contraseña_013AL { get; set; }
        public string Nombres_013AL { get; set; }
        public string Apellidos_013AL { get; set; }
        public int CodRol_013AL { get; set; }
        public string DNI_013AL { get; set; }
        public string Login_013AL { get; set; }
        public bool Bloqueo_013AL { get; set; }
        public bool Activo_013AL { get; set; }
        public bool Eliminado_013AL { get; set; }


        public List<Rol_013AL> _permisos { get; set; }

        public void AsignarPermisos_013AL(List<Rol_013AL> permisos)
        {
            _permisos = permisos;
        }
            
    }
}
