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
using System.Windows.Controls;
using System.Windows.Forms;

namespace UI
{
    public partial class PagarProducto_013AL : Form, IObserver_013AL
    {
        public PagarProducto_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }
        NegocioBLL_013AL bll = new NegocioBLL_013AL();

        public void ActualizarIdioma_013AL()
        {
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }

        public void CargarOrdenCompra_013AL()
        {
            var listacod = bll.ListarOrdenCompra_013AL();
            comboBox1.Items.Clear();

            foreach (var cod in listacod)
            {
                string detalle = $"{cod.CodOrdenCompra_013AL}";

                comboBox1.Items.Add(new ComboBoxItem
                {
                    Text = detalle,
                    Value = cod
                });
            }
        }
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is ComboBoxItem selectedItem)
            {
                var solicitudSeleccionada = (OrdenCompra_013AL)selectedItem.Value;
                int codsc = solicitudSeleccionada.Total_013AL;
                label3.Text = codsc.ToString();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Pago realizado con éxito");

            BLLBitacora_013AL bbll = new BLLBitacora_013AL();
            Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
            bbll.AgregarEvento_013AL(user.Login_013AL, "PagarProducto", "Compra Pagada", 2);
        }

        private void PagarProducto_Load(object sender, EventArgs e)
        {
            CargarOrdenCompra_013AL();
        }
    }
}
