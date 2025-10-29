using BLL_013AL;
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
            // Verifica si la columna modificada es "Completo"
            if (dataGridViewProductos.Columns[e.ColumnIndex].Name == "Completo")
            {
                // Obtener el valor actualizado de la celda
                bool nuevoValorCompleto = Convert.ToBoolean(dataGridViewProductos.Rows[e.RowIndex].Cells["Completo"].Value);
                int codOrdenCompra = Convert.ToInt32(dataGridViewProductos.Rows[e.RowIndex].Cells["CodOrdenCompra"].Value); // Asegúrate de tener "CodOrdenCompra" en los datos

                // Actualizar en la base de datos
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
            comboBoxOrdenesCompra.DisplayMember = "CodOrdenCompra";
            comboBoxOrdenesCompra.ValueMember = "CodOrdenCompra";
        }

        /*private void comboBoxOrdenesCompra_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxOrdenesCompra.SelectedValue != null)
            {
                int codSolicitud = Convert.ToInt32(comboBoxOrdenesCompra.SelectedValue);

                // Llama al método en BLL para obtener los productos asociados a la orden de compra
                DataTable dtProductos = bll.ListarProductosPorSolicitud(codSolicitud);

                // Carga los datos en el DataGridView
                dataGridViewProductos.DataSource = dtProductos;

                // Configura la columna "Completo" para que sea la única editable
                ConfigurarDataGridView();
            }
        }*/
        private void ConfigurarDataGridView_013AL()
        {
            // Configurar todas las columnas como de solo lectura excepto "Completo"
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
            int codOrdenCompra = Convert.ToInt32(comboBoxOrdenesCompra.SelectedValue);

            // Llama al método BLL para obtener los productos de la orden
            DataTable dtProductos = bll.ListarProductosPorOrden_013AL(codOrdenCompra);

            // Carga los productos en el DataGridView
            dataGridViewProductos.DataSource = dtProductos;

            // Configura el DataGridView para que solo la columna "Completo" sea editable
            ConfigurarDataGridView_013AL();
        }
    }
}
