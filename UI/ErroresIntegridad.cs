using BE;
using BE_013AL;
using BLL;
using BLL_013AL;
using Servicios;
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
    public partial class ErroresIntegridad : Form
    {
        List<ErrorIntegridad_013AL> ListaErrores;
        FacturaBLL_013AL fbll = new FacturaBLL_013AL();
        Usuarios_013AL user;
        EventoBLL_013AL bll = new EventoBLL_013AL();
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
            var tablas = new List<string> { "Factura-013AL" };
            foreach (string tabla in tablas)
            {
                fbll.ActualizarDVH(tabla);
                fbll.ActualizarDVV(tabla);
            }

            List<ErrorIntegridad_013AL> nuevosErrores = fbll.VerificarIntegridadCompleta(tablas);

            if (nuevosErrores.Count == 0)
            {
                MessageBox.Show("Integridad restaurada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL("Administrador", "Errores Integridad", "Se actualizaron los digitos verificadores de la tabla Factura-013AL", 4);
                this.Close();
            }
            else
            {
                MessageBox.Show("Persisten errores de integridad. Revise los datos manualmente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dataGridView1.DataSource = nuevosErrores;
                //user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL("Administrador", "Errores Integridad", "Persisten errores de integridad de la tabla Factura-013AL", 5);
            }
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
