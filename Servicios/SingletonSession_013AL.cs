using BE_013AL;
using BE_013AL.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_013AL
{
    public class SingletonSession_013AL
    {
        private static SingletonSession_013AL _instance_013AL;
        private static readonly object _lock_013AL = new object();
        private string idiomaActual_013AL;
        public Usuarios_013AL LoggedInUser_013AL { get; private set; }

        public List<Componente_013AL> Permisos_013AL { get; set; }
        private SingletonSession_013AL() { Permisos_013AL = new List<Componente_013AL>(); }

        public void ClearPermisos()
        {
            Permisos_013AL.Clear(); 
        }

        public static SingletonSession_013AL Instance
        {
            get
            {
                lock (_lock_013AL)
                {
                    if (_instance_013AL == null)
                    {
                        _instance_013AL = new SingletonSession_013AL();
                    }
                    return _instance_013AL;
                }
            }
        }

        public bool IsLoggedIn_013AL()
        {
            return LoggedInUser_013AL != null;
        }

        public void Login_013AL(Usuarios_013AL user/*, List<PermisoCompuesto> permisos*/)
        {
            if (!IsLoggedIn_013AL())
            {
                LoggedInUser_013AL = user;
                //Permisos = permisos;
                //Permisos = new UsuarioBLL().ObtenerPermisosPorRol(user.Rol);
            }
            else
            {
                throw new InvalidOperationException("A user is already logged in.");
            }
        }

        public void Logout_013AL()
        {
            LoggedInUser_013AL = null;
            Permisos_013AL = null;
        }

        public Usuarios_013AL GetUsuario_013AL()
        {
            if (IsLoggedIn_013AL())
            {
                return LoggedInUser_013AL;
            }
            else
            {
                throw new InvalidOperationException("No user is currently logged in.");
            }
        }

        /*public bool TienePermiso(string nombrePermiso)
        {
            return Permisos.Any(p => p.Nombre == nombrePermiso);
        }*/

        public string IdiomaActual_013AL
        {
            get { return idiomaActual_013AL; }
            set
            {
                idiomaActual_013AL = value;
                LanguageManager_013AL.ObtenerInstancia_013AL().CargarIdioma_013AL();
                LanguageManager_013AL.ObtenerInstancia_013AL().Notificar_013AL();
            }
        }
    }
}

