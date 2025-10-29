using BE_013AL;
using BLL;
using BLL_013AL;
using Servicios_013AL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class Productos_C_013AL : Form, IObserver_013AL
    {
        public Productos_C_013AL()
        {
            InitializeComponent();
            dateTimePicker1.ValueChanged += new EventHandler(dateTimePicker1_ValueChanged);
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }
        ProductoCBLL_013AL bll = new ProductoCBLL_013AL();
        public void ActualizarIdioma_013AL()
        {
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }
        private void ListarProductosC_013AL()
        {
            dataGridView1.DataSource = bll.ListarProductosC_013AL();
        }

        private void Productos_C_Load(object sender, EventArgs e)
        {
            ListarProductosC_013AL();
            
            dateTimePicker1.Value = DateTime.Now; 
            dateTimePicker2.Value = dateTimePicker1.Value.AddDays(30);
        }
        
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
            dateTimePicker2.Value = dateTimePicker1.Value.AddDays(30);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = bll.ConsultaProductosC_013AL(
        string.IsNullOrEmpty(textBox1.Text) ? (int?)null : Convert.ToInt32(textBox1.Text), // idp
        textBox2.Text, // nombre
        string.IsNullOrEmpty(dateTimePicker1.Text) ? (DateTime?)null : Convert.ToDateTime(dateTimePicker1.Text).Date, // fechaInicio
        string.IsNullOrEmpty(dateTimePicker2.Text) ? (DateTime?)null : Convert.ToDateTime(dateTimePicker2.Text).Date // fechaFin
        );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ListarProductosC_013AL();
            textBox1.Text = "";
            textBox2.Text = "";
            dateTimePicker1.Text = "";
            dateTimePicker2.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int codProductoC = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CodProductoC-013AL"].Value);

                try
                {
                    bll.RestaurarVersionProducto_013AL(codProductoC);

                    MessageBox.Show("Versión restaurada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ListarProductosC_013AL();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al restaurar la versión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccioná una versión para restaurar.");
            }
        }
    }
}
