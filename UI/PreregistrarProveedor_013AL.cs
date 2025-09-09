using BE_013AL;
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

namespace UI
{
    public partial class PreregistrarProveedor_013AL : Form, IObserver_013AL
    {
        public PreregistrarProveedor_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }
        NegocioBLL_013AL bll = new NegocioBLL_013AL();
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return;
            }

            int cuit;
            if (!int.TryParse(textBox2.Text, out cuit))
            {
                MessageBox.Show("El CUIT debe ser un número válido.");
                return;
            }

            if (bll.ExisteCuit_013AL(cuit))
            {
                MessageBox.Show("El CUIT ingresado ya está registrado.");
                return;
            }

            string respuesta = bll.PreregistrarProveedor_013AL(textBox1.Text, cuit, textBox3.Text);
            MessageBox.Show(respuesta);

            if (respuesta == "Proveedor preregistrado correctamente.")
            {
                BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bbll.AgregarEvento_013AL(user.Login_013AL, "Preregistrar Proveedores", "Preregistrar Proveedor", 2);
            }
            /*string respuesta = "";
            respuesta = bll.PreregistrarProveedor(textBox1.Text, Convert.ToInt32(textBox2.Text), textBox3.Text);


            BLLBitacora bbll = new BLLBitacora();
            Usuarios user = SingletonSesion.Instance.GetUsuario();
            bbll.AgregarEvento(user.NombreUsuario, "Preregistrar Proveedores", "Preregistrar Proveedor", 2);*/
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
