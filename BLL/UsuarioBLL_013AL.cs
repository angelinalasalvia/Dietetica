using BE_013AL;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE_013AL.Composite;
using Servicios;
using System.Globalization;
using System.Data;
using DAL_013AL;

namespace BLL_013AL
{
    public class UsuarioBLL_013AL 
    {
        private DALUsuario_013AL dal = new DALUsuario_013AL();

        public List<Usuarios_013AL> Listar_013AL()
        {
            return dal.Listar_013AL();
        }

        public string CambiarContraseña_013AL(string login, string contraseña)
        {
            Usuarios_013AL usuario = new Usuarios_013AL();
            usuario.Login_013AL = login;
            usuario.Contraseña_013AL = contraseña;
            return dal.CambiarContraseña_013AL(usuario);
        }

        public string DesbloquearUsuario_013AL(string nomUsuario)
        {
            Usuarios_013AL usuario = new Usuarios_013AL();
            usuario.Login_013AL = nomUsuario;
            return dal.DesbloquearUsuario_013AL(usuario);
        }

        public string BloquearUsuario_013AL(string nomUsuario)
        {
            Usuarios_013AL usuario = new Usuarios_013AL();
            usuario.Login_013AL = nomUsuario;
            return dal.BloquearUsuario_013AL(usuario);
        }

        public string AgregarUsuario_013AL(string email, string contraseña, string nombres, string apellidos, int rol, string dni, string nomUsuario, bool bloqueo, bool activo)
        {
            Usuarios_013AL usuario = new Usuarios_013AL();
            usuario.Email_013AL = email;
            usuario.Contraseña_013AL = contraseña;
            usuario.Nombres_013AL = nombres;
            usuario.Apellidos_013AL = apellidos;
            usuario.CodRol_013AL = rol;
            usuario.DNI_013AL = dni;
            usuario.Login_013AL = nomUsuario;
            usuario.Bloqueo_013AL = bloqueo;
            usuario.Activo_013AL = activo;
            return dal.AgregarUsuario_013AL(usuario);
        }

        public string ObtenerContraseñaActual_013AL(string email)
        {
            return dal.ObtenerContraseñaActual_013AL(email);
        }

        public DataTable ListarUsuarios_013AL()
        {
            return dal.ListarUsuarios_013AL();
        }

        public DataSet ListarUsuariosActivos_013AL()
        {
            return dal.ListarUsuariosActivos_013AL();
        }

        public DataSet ObtenerUsuarios_013AL()
        {
            return dal.ObtenerUsuarios_013AL();
        }

        public void GuardarUsuarios_013AL(string NombreTabla, DataSet Dset)
        {
            dal.GuardarUsuarios_013AL(NombreTabla, Dset);
            
        }
        
        public DataTable ObtenerNombreApellidoPorLogin_013AL(string login)
        {
            return dal.ObtenerNombreApellidoPorLogin_013AL(login);
        }
    }
}