using BE;
using BE_013AL;
using BE_013AL.Composite;
using BLL;
using BLL_013AL;
using Servicios_013AL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UI
{
    public partial class Login_013AL : Form, IObserver_013AL
    {
        private UsuarioBLL_013AL bllUsuarios = new UsuarioBLL_013AL();
        private RolBLL_013AL rbll = new RolBLL_013AL();
        private PermisoBLL_013AL pbll = new PermisoBLL_013AL();
        private FacturaBLL_013AL fbll = new FacturaBLL_013AL();
        Usuarios_013AL usuario;
        private Dictionary<string, int> intentosFallidosPorUsuario = new Dictionary<string, int>();
        private const int maxFailedAttempts = 3;
        public Login_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }

        EventoBLL_013AL bbll = new EventoBLL_013AL();
        Usuarios_013AL user;

        private void btnIngresar_Click_1(object sender, EventArgs e)
        {
            
            string tabla = "Usuarios_013AL"; 

            string passwordHash = HashHelper_013AL.CalcularSHA256_013AL(txtPassword.Text);

            if (SingletonSession_013AL.Instance.IsLoggedIn_013AL())
            {
                MessageBox.Show("Ya hay un usuario logueado. Cierre la sesión actual antes de iniciar una nueva.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Login", "Ya hay una sesión iniciada", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                return;
            }

            var usuarios = bllUsuarios.Listar_013AL();
            usuario = usuarios.FirstOrDefault(u => u.Login_013AL == txtUsuario.Text);

            if (usuario == null)
            {
                MessageBox.Show("Usuario no encontrado.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Login", "Usuario ingresado no existe", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }

            if (usuario.Bloqueo_013AL)
            {
                MessageBox.Show("El usuario está bloqueado. No puede iniciar sesión.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Login", "Usuario ingresado bloqueado", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                return;
            }

            if (usuario.Activo_013AL == false)
            {
                MessageBox.Show("El usuario está Desactivado. No puede iniciar sesión.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Login", "Usuario ingresado desactivado", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                return;
            }
            if (usuario.Eliminado_013AL)
            {
                MessageBox.Show("El usuario ha sido eliminado. No puede iniciar sesión.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Login", "Usuario ingresado eliminado", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                return;
            }

            if (usuario.Contraseña_013AL == passwordHash)
            {               

                if (intentosFallidosPorUsuario.ContainsKey(usuario.Login_013AL))
                    intentosFallidosPorUsuario.Remove(usuario.Login_013AL);

                List<string> tablas = new List<string> { "Factura-013AL" };
                List<ErrorIntegridad_013AL> errores = fbll.VerificarIntegridadCompleta(tablas);

                if (errores.Count > 0)
                {
                    // si hay errores, mostrar
                    if (usuario.CodRol_013AL == 1)
                    {
                        ErroresIntegridad frmerrores = new ErroresIntegridad(errores);
                        frmerrores.Show();
                    }
                    else
                    {
                        MessageBox.Show("No es posible ingresar al sistema. Contacte con un administrador.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return;
                }

                // si no hay errores, inicializar (recalcular) los DVH/DVV
                fbll.InicializarDVH_DVV("Factura-013AL");


                SingletonSession_013AL.Instance.Login_013AL(usuario);
                MessageBox.Show("Login exitoso", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Login", "Inicio de Sesión Exitoso", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            else
            {
                if (!intentosFallidosPorUsuario.ContainsKey(usuario.Login_013AL))
                    intentosFallidosPorUsuario[usuario.Login_013AL] = 0;

                intentosFallidosPorUsuario[usuario.Login_013AL]++;

                int intentos = intentosFallidosPorUsuario[usuario.Login_013AL];

                if (intentos >= maxFailedAttempts)
                {
                    bllUsuarios.BloquearUsuario_013AL(usuario.Login_013AL);
                    MessageBox.Show("Usuario bloqueado después de 3 intentos fallidos.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        
                    intentosFallidosPorUsuario.Remove(usuario.Login_013AL);
                    try
                    {
                        user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bbll.AgregarEvento_013AL(user.Login_013AL, "Login", "Usuario Bloqueado", 3);
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }
                else
                {
                    MessageBox.Show($"Usuario y/o Contraseña incorrectos. Intento {intentos} de {maxFailedAttempts}", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    try
                    {
                        user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bbll.AgregarEvento_013AL(usuario.Login_013AL, "Login", $"Intento {intentos} de Login Fallido ", 2);
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }
            }


            if (SingletonSession_013AL.Instance.IsLoggedIn_013AL())
            {
                
                string dni = SingletonSession_013AL.Instance.GetUsuario_013AL().DNI_013AL;

                
                List <Rol_013AL> permisos = rbll.ListarPermisos_013AL(dni);

                
                foreach (var permiso in permisos)
                {
                    string nombre = pbll.ObtenerNombrePermiso_013AL(permiso.Cod_013AL);
                    permiso.Nombre_013AL = nombre;
                }

                SingletonSession_013AL.Instance.Permisos_013AL = permisos;

                
                usuario.AsignarPermisos_013AL(permisos);

            }   

            
        }
        
    

        private void btnsalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void ActualizarIdioma_013AL()
        {
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }

       
    }

}

