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
    public class UsuarioBLL_013AL //: AbstractBLL<Usuarios>
    {
        private DALConexiones_013AL dal = new DALConexiones_013AL();

        public List<Usuarios_013AL> Listar_013AL()
        {
            return dal.Listar_013AL();
        }

        public string CambiarContraseña_013AL(string login, string contraseña)
        {
            DALConexiones_013AL dal = new DALConexiones_013AL();
            Usuarios_013AL usuario = new Usuarios_013AL();
            usuario.Login_013AL = login;
            usuario.Contraseña_013AL = contraseña;
            return dal.CambiarContraseña_013AL(usuario);
        }

        /*public string BloquearUsuario(string nomUsuario, bool bloqueo)
        {
            DALConexiones dal = new DALConexiones();
            Usuarios usuario = new Usuarios();
            usuario.NombreUsuario = nomUsuario;
            usuario.Bloqueo = bloqueo;
            return dal.BloquearUsuario(usuario);
        }*/

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

        public string AgregarUsuario_013AL(string email, string contraseña, string nombres, string apellidos, Rol_013AL rol, string dni, string nomUsuario, bool bloqueo, bool activo)
        {
            DALConexiones_013AL dal = new DALConexiones_013AL();
            Usuarios_013AL usuario = new Usuarios_013AL();
            usuario.Email_013AL = email;
            usuario.Contraseña_013AL = contraseña;
            usuario.Nombres_013AL = nombres;
            usuario.Apellidos_013AL = apellidos;
            usuario.Rol_013AL = rol;
            usuario.DNI_013AL = dni;
            usuario.Login_013AL = nomUsuario;
            usuario.Bloqueo_013AL = bloqueo;
            usuario.Activo_013AL = activo;
            return dal.AgregarUsuario_013AL(usuario);
        }

        public string ObtenerContraseñaActual_013AL(string email)
        {
            DALConexiones_013AL dal = new DALConexiones_013AL();
            return dal.ObtenerContraseñaActual_013AL(email);
        }

        public DataTable ListarUsuarios_013AL()
        {
            DALConexiones_013AL dal = new DALConexiones_013AL();
            return dal.ListarUsuarios_013AL();
        }

        public DataSet ListarUsuariosActivos_013AL()
        {
            DALConexiones_013AL dal = new DALConexiones_013AL();
            return dal.ListarUsuariosActivos_013AL();
        }



        /*public DataSet Listar2()
        {
            DALConexiones dal = new DALConexiones();
            dynamic query = "SELECT Id, Mail, Contraseña, Nombres, Apellidos, Rol, DNI, NombreUsuario, Bloqueo, Activo";
            return dal.Leer(query);
        }*/


        //private UsuarioDAL usuarioDAL = new UsuarioDAL();

        public DataSet ObtenerUsuarios_013AL()
        {
            return dal.ObtenerUsuarios_013AL();
        }

        public DataSet ObtenerClientes_013AL()
        {
            return dal.ObtenerClientes_013AL();
        }

        public void GuardarUsuarios_013AL(string NombreTabla, DataSet Dset)
        {
            dal.GuardarUsuarios_013AL(NombreTabla, Dset);
            
        }
        public void GuardarClientes_013AL(string NombreTabla, DataSet Dset)
        {
            dal.GuardarClientes_013AL(NombreTabla, Dset);

        }

        //NEGOCIO
        public string AgregarCliente_013AL(string nombre, string apellido, int cuil, string domicilio, string mail, int tel)
        {
            Cliente_013AL cliente = new Cliente_013AL();
            cliente.Nombre_013AL = nombre;
            cliente.Apellido_013AL = apellido;
            cliente.CUIL_013AL = cuil;
            cliente.Domicilio_013AL = domicilio;
            cliente.Mail_013AL = mail;
            cliente.Telefono_013AL = tel;
            return dal.AgregarCliente_013AL(cliente);
        }

        public List<Cliente_013AL> BuscarCliente_013AL()
        {
            return dal.BuscarCliente_013AL();

        }
        public Cliente_013AL BuscarClientePorCUIL_013AL(int cuil)
        {
            return dal.BuscarClientePorCUIL_013AL(cuil);
        }


    }

}

    /*private DALConexiones dal = new DALConexiones();

    public List<Usuarios> Listar()
    {
        return dal.Listar();
    }

    public LoginResult Login(string email, string password)
    {
        if (SingletonSesion.Instancia.IsLogged())
            throw new Exception("Ya hay una sesión iniciada");

        var user = dal.Listar().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (user == null) throw new LoginException(LoginResult.InvalidUsername);

        if (!Encriptador.Hash(password).Equals(user.Contraseña))
            throw new LoginException(LoginResult.InvalidPassword);

        SingletonSesion.Instancia.Login(user);
        return LoginResult.ValidUser;
    }

    public void Logout()
    {
        if (!SingletonSesion.Instancia.IsLogged())
            throw new Exception("No hay sesión iniciada");

        SingletonSesion.Instancia.Logout();
    }*/

    /*private DALConexiones dal = new DALConexiones();

    public LoginResult Login(string email, string password)
    {
        if (SingletonSesion.Instancia.IsLogged())
            throw new Exception("Ya hay una sesión iniciada");

        // Ahora llamamos a la versión del método Listar que acepta parámetros
        var user = dal.Listar(email, password).FirstOrDefault();
        if (user == null)
            throw new LoginException(LoginResult.InvalidUsername);

        if (!Encriptador.Hash(password).Equals(user.Contraseña))
            throw new LoginException(LoginResult.InvalidPassword);

        SingletonSesion.Instancia.Login(user);
        return LoginResult.ValidUser;
    }

    public void Logout()
    {
        if (!SingletonSesion.Instancia.IsLogged())
            throw new Exception("No hay sesión iniciada");

        SingletonSesion.Instancia.Logout();
    }
}*/















    //FamiliaBLL _bllFamilias = new FamiliaBLL();

    /*
     ChatGPT
     public bool Login(string email, string contrasena)
    {
        Usuarios usuario = DALConexiones.Instance.ObtenerUsuarioPorNombre(email);

        if (usuario != null && usuario.Contraseña == contrasena)
        {
            return true;
        }

        return false;
    }*/


    /* public bool Login(string mail, string contraseña)
    {
        if (SessionManager.IsLogged()) ;
    }*/

    /*    public List<Usuarios> Listar()
        {
            return dal.Listar();
        }*/
    /*public UsuarioBLL()
    {
        _crud = new UsuarioDAL();
        SimularDatos();
    }


    private void SimularDatos()
    {


        _bllFamilias.SimularDatos();

        //u1 puede gestionar usuarios
        var u = new Usuarios();
        u.Email = "u1@mail.com";
        u.Contraseña = Encriptador.Hash("123");
        var f = _bllFamilias.GetAll().Where(ff => ff.Nombre.Contains("Gestores de usuarios")).FirstOrDefault();
        if (f != null) u.Permisos.Add(f);

        _crud.Save(u);


        //u2 puede gestionar permisos
        u = new Usuarios();
        u.Email = "u2@mail.com";
        u.Contraseña = Encriptador.Hash("123");
        f = _bllFamilias.GetAll().Where(ff => ff.Nombre.Contains("Gestores de permisos")).FirstOrDefault();
        if (f != null) u.Permisos.Add(f);
        _crud.Save(u);


        //admin tiene todo
        u = new Usuarios();
        u.Email = "admin@mail.com";
        u.Contraseña = Encriptador.Hash("123");
        f = _bllFamilias.GetAll().Where(ff => ff.Nombre.Contains("Administradores")).FirstOrDefault();
        if (f != null) u.Permisos.Add(f);

        _crud.Save(u);




    }*/
    /*private DALConexiones dal = new DALConexiones();
 public List<Usuarios> Listar()
    {
        return dal.Listar();
    }

    public LoginResult Login(string email, string password)
     {

         if (SingletonSesion.Instancia.IsLogged())
             throw new Exception("Ya hay una sesión iniciada"); //doble validación, anulo en boton en formulario y valido en la bll


         var user = _crud.GetAll().Where(u => u.Email.Equals(email)).FirstOrDefault();
         if (user == null) throw new LoginException(LoginResult.InvalidUsername);

         if (!Encriptador.Hash(password).Equals(user.Contraseña))
             throw new LoginException(LoginResult.InvalidPassword);
         else
         {
             SingletonSesion.Instancia.Login(user);
             return LoginResult.ValidUser;
         }


    }

     public void Logout()
     {
         if (!SingletonSesion.Instancia.IsLogged())
             throw new Exception("No hay sesión iniciada"); //doble validación, anulo en boton en formulario y valido en la bll


         SingletonSesion.Instancia.Logout();
     }*/


