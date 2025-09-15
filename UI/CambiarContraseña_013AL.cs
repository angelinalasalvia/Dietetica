using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BE_013AL;
using BLL_013AL;
using Servicios_013AL;

namespace UI
{
    public partial class CambiarContraseña_013AL : Form, IObserver_013AL
    {
        public CambiarContraseña_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            
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

        BLLBitacora_013AL bbll = new BLLBitacora_013AL();
        Usuarios_013AL user;

        public bool SesionCerrada { get; private set; } = false;    

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese contraseña actual", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox1.Focus();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Intento Cambio Contraseña Fallido", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }
            if (textBox2.Text.Trim() == "")
            {
                MessageBox.Show("Ingrese nueva contraseña", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox2.Focus();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Intento Cambio Contraseña Fallido", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }
            if (textBox3.Text.Trim() == "")
            {
                MessageBox.Show("Confirme nueva contraseña", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox3.Focus();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Intento Cambio Contraseña Fallido", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }
            if (textBox2.Text != textBox3.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Focus();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Intento Cambio Contraseña Fallido", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }
            
            string usuario = SingletonSession_013AL.Instance.GetUsuario_013AL().Login_013AL;
            UsuarioBLL_013AL bll = new UsuarioBLL_013AL();

            
            string contraseñaActualEnBD = bll.ObtenerContraseñaActual_013AL(usuario);

            if (contraseñaActualEnBD == null)
            {
                MessageBox.Show("Error al obtener la contraseña actual", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Error al obtener la contraseña actual", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }

            
            string contraseñaActualIngresada = HashHelper_013AL.CalcularSHA256_013AL(textBox1.Text);

            
            if (contraseñaActualIngresada != contraseñaActualEnBD)
            {
                MessageBox.Show("La contraseña actual es incorrecta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Error al verificar la contraseña actual", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }

            
            string nuevaContraseñaEncriptada = HashHelper_013AL.CalcularSHA256_013AL(textBox2.Text);

            
            if (nuevaContraseñaEncriptada == contraseñaActualEnBD)
            {
                MessageBox.Show("La nueva contraseña no puede ser igual a la anterior", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Intento Cambio Contraseña Fallido", 1);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }

            
            string resultado = bll.CambiarContraseña_013AL(usuario, nuevaContraseñaEncriptada);

            if (resultado == "OK")
            {
                MessageBox.Show("Contraseña actualizada con éxito", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();

                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Contraseña actualizada", 2);
                }
                catch (Exception ex) { Console.WriteLine(ex); }

                SingletonSession_013AL.Instance.Logout_013AL();
                SesionCerrada = true;
            }
            else
            {
                MessageBox.Show("Hubo un error al cambiar la contraseña", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bbll.AgregarEvento_013AL(user.Login_013AL, "Cambiar Contraseña", "Intento Cambio Contraseña Fallido", 1);
            }
           
        }

        private void CambiarContraseña_Load(object sender, EventArgs e)
        {
            ActualizarIdioma_013AL();
        }
    }
}
