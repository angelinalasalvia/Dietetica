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
    public partial class RecepciónProductos_013AL : Form, IObserver_013AL
    {
        public RecepciónProductos_013AL()
        {
            InitializeComponent();
            dataGridViewProductos.CellValueChanged += dataGridViewProductos_CellValueChanged;
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
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

        OrdenCompraBLL_013AL bll = new OrdenCompraBLL_013AL();
        private DataTable dtOrdenesCompra;

        private void dataGridViewProductos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewProductos.Columns[e.ColumnIndex].Name == "Completo")
            {
                bool nuevoValorCompleto = Convert.ToBoolean(dataGridViewProductos.Rows[e.RowIndex].Cells["Completo"].Value);
                int codOrdenCompra = Convert.ToInt32(dataGridViewProductos.Rows[e.RowIndex].Cells["CodOrdenCompra"].Value); 

                string resultado = bll.ActualizarEstadoCompleto_013AL(codOrdenCompra, nuevoValorCompleto);
                if (resultado == "OK")
                {
                    MessageBox.Show("Estado de 'Completo' actualizado correctamente.");
                }
                else
                {
                    MessageBox.Show("Error al actualizar el estado de 'Completo'.");
                }
            }
        }


        private void CargarOrdenesCompra_013AL()
        {
            dtOrdenesCompra = bll.ListarOrdenesCompra_013AL();
            comboBoxOrdenesCompra.DataSource = dtOrdenesCompra;
            comboBoxOrdenesCompra.DisplayMember = "CodOrdenCompra-013AL";
            comboBoxOrdenesCompra.ValueMember = "CodOrdenCompra-013AL";
            comboBoxOrdenesCompra.SelectedIndex = -1;
        }

        private void ConfigurarDataGridView_013AL()
        {
            foreach (DataGridViewColumn column in dataGridViewProductos.Columns)
            {
                column.ReadOnly = column.Name != "Completo";
            }
        }

        private void RecepciónProductos_Load(object sender, EventArgs e)
        {
            CargarOrdenesCompra_013AL();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBoxOrdenesCompra.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione una orden de compra.");
                return;
            }

            int codOrdenCompra = Convert.ToInt32(comboBoxOrdenesCompra.SelectedValue);

            DataTable dtProductos = bll.ListarProductosPorOrden_013AL(codOrdenCompra);

            dataGridViewProductos.DataSource = dtProductos;

            ConfigurarDataGridView_013AL();
        }
        Usuarios_013AL user;
        EventoBLL_013AL bbll = new EventoBLL_013AL();
        private void button2_Click(object sender, EventArgs e)
        {
            int codOC = Convert.ToInt32(comboBoxOrdenesCompra.SelectedValue);

            string resultado = bll.ConfirmarRecepcion_013AL(codOC);

            if (resultado == "OK")
            {
                MessageBox.Show("Recepción confirmada correctamente.");
                dataGridViewProductos.DataSource = null;
                CargarOrdenesCompra_013AL();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Recepción Productos", $"Productos de Orden de Compra número {codOC} recibidos.", 3);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }
            else
            {
                MessageBox.Show(resultado);
            }
        }

    }
}
