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
        OrdenCompraBLL_013AL bll = new OrdenCompraBLL_013AL();

        public void ActualizarIdioma_013AL()
        {
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
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
            /*if (comboBox1.SelectedItem is ComboBoxItem selectedItem)
            {
                var solicitudSeleccionada = (OrdenCompra_013AL)selectedItem.Value;
                int codsc = solicitudSeleccionada.Total_013AL;
                label3.Text = codsc.ToString();
            }*/

        }
        Usuarios_013AL user;
        EventoBLL_013AL bbll = new EventoBLL_013AL();
        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una orden.");
                return;
            }

            if (comboBox2.Text == "")
            {
                MessageBox.Show("Seleccione método de pago.");
                return;
            }

            if (comboBox2.Text == "Tarjeta Débito")
            {
                if (txtTarjeta.Text.Length != 16 || !txtTarjeta.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Tarjeta inválida.");
                    return;
                }
            }

            int codOrden = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CodOrdenCompra-013AL"].Value);

            string resultado = bll.ActualizarEstadoCobrado_013AL(codOrden);

            if (resultado == "OK")
            {
                MessageBox.Show("Pago realizado con éxito");

                CargarOrdenes_013AL();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Pagar Producto", $"Compra número {codOrden} Pagada", 2);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }
            else
            {
                MessageBox.Show(resultado);
            }
        }
        private void CargarOrdenes_013AL()
        {
            dataGridView1.DataSource =
                bll.ListarOrdenesPendientesPago_013AL();
        }
        private void PagarProducto_Load(object sender, EventArgs e)
        {
            CargarOrdenes_013AL();
            comboBox2.Items.Add("Efectivo");
            comboBox2.Items.Add("Tarjeta Débito");
            txtTarjeta.Enabled = false;
        }
        
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTarjeta.Enabled = comboBox2.Text == "Tarjeta Débito";
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int total = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Total-013AL"].Value);

                textBox1.Text = total.ToString();
            }
        }
    }
}
