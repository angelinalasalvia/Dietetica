using BE;
using BLL;
using Servicios;
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
    public partial class ErroresIntegridad : Form
    {
        List<ErrorIntegridad_013AL> ListaErrores;
        FacturaBLL_013AL fbll = new FacturaBLL_013AL();
        public ErroresIntegridad(List<ErrorIntegridad_013AL> errores)
        {
            InitializeComponent();
            ListaErrores = errores;
        }
        void CargarGrillaErrores()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = ListaErrores;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tablas = new List<string> { "Usuario_597DG" };
            foreach (string tabla in tablas)
            {
                fbll.ActualizarDVH(tabla);
                fbll.ActualizarDVV(tabla);
            }

            MessageBox.Show("Digitos Verificadores Actualizados.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        private void ErroresIntegridad_Load(object sender, EventArgs e)
        {
            CargarGrillaErrores();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Respaldos_013AL form = new Respaldos_013AL();
            form.ShowDialog();
        }
    }
}
