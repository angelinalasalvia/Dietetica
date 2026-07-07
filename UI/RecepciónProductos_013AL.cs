using BE;
using BE_013AL;
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

namespace UI
{
    public partial class RecepciónProductos_013AL : Form, IObserver_013AL
    {
        public RecepciónProductos_013AL()
        {
            InitializeComponent();
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

        OrdenCompraBLL_013AL ocbll = new OrdenCompraBLL_013AL();
        DetalleSolicitudBLL_013AL dbll = new DetalleSolicitudBLL_013AL();
        ProductoBLL_013AL productoBLL = new ProductoBLL_013AL();
        LoteBLL_013AL loteBLL = new LoteBLL_013AL();

        private DataTable dtOrdenesCompra;

        
        private void CargarOrdenesCompra_013AL()
        {
            dtOrdenesCompra = ocbll.ListarOrdenesCompra_013AL();
            comboBoxOrdenesCompra.DataSource = dtOrdenesCompra;
            comboBoxOrdenesCompra.DisplayMember = "CodOrdenCompra-013AL";
            comboBoxOrdenesCompra.ValueMember = "CodOrdenCompra-013AL";
            comboBoxOrdenesCompra.SelectedIndex = -1;
            comboBoxOrdenesCompra.Text = string.Empty;

            dataGridViewProductos.DataSource = null;
        }

        private void ConfigurarDataGridView_013AL()
        {
            foreach (DataGridViewColumn column in dataGridViewProductos.Columns)
                column.ReadOnly = true;

            dataGridViewProductos.Columns["CantidadIngresada"].ReadOnly = false;
            dataGridViewProductos.Columns["FechaVencimiento"].ReadOnly = false;
            dataGridViewProductos.Columns["CodSCotizacion-013AL"].Visible = false;
            dataGridViewProductos.Columns["CodProducto-013AL"].Visible = false;
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

            DataTable dtProductos = ocbll.ListarProductosPorOrden_013AL(codOrdenCompra);

            dataGridViewProductos.DataSource = dtProductos;

            ConfigurarDataGridView_013AL();
        }
        Usuarios_013AL user;
        EventoBLL_013AL bbll = new EventoBLL_013AL();

        
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBoxOrdenesCompra.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una orden de compra.");
                return;
            }

            bool huboRecepcion = false;

            try
            {

                foreach (DataGridViewRow fila in dataGridViewProductos.Rows)
                {
                    if (fila.IsNewRow)
                        continue;

                    int codSolicitud = Convert.ToInt32(fila.Cells["CodSCotizacion-013AL"].Value);
                    int codProducto = Convert.ToInt32(fila.Cells["CodProducto-013AL"].Value);

                    int cantidadPedida = Convert.ToInt32(fila.Cells["CantidadPedida-013AL"].Value);
                    int cantidadRecibida = Convert.ToInt32(fila.Cells["CantidadRecibida"].Value);

                    int cantidadIngresada = 0;

                    if (fila.Cells["CantidadIngresada"].Value != null &&
                    fila.Cells["CantidadIngresada"].Value != DBNull.Value &&
                    fila.Cells["CantidadIngresada"].Value.ToString() != "")
                    {
                        cantidadIngresada = Convert.ToInt32(fila.Cells["CantidadIngresada"].Value);
                    }

                    if (cantidadIngresada < 0)
                    {
                        MessageBox.Show("La cantidad ingresada no puede ser negativa.");
                        return;
                    }

                    if (cantidadIngresada == 0)
                        continue;

                    if (cantidadRecibida + cantidadIngresada > cantidadPedida)
                    {
                        MessageBox.Show("No puede recibir más cantidad de la solicitada.");
                        return;
                    }

                    if (fila.Cells["FechaVencimiento"].Value == DBNull.Value ||
                        fila.Cells["FechaVencimiento"].Value == null)
                    {
                        MessageBox.Show("Debe ingresar la fecha de vencimiento.");
                        return;
                    }

                    DateTime fechaVencimiento =
                        Convert.ToDateTime(fila.Cells["FechaVencimiento"].Value);


                    huboRecepcion = true;

                    dbll.ActualizarCantidadRecibida_013AL(
                        codSolicitud,
                        codProducto,
                        cantidadIngresada);

                    Lote_013AL lote = new Lote_013AL();

                    

                    lote.CodProducto_013AL = codProducto;
                    lote.FechaVencimiento_013AL = fechaVencimiento;
                    lote.CantidadInicial_013AL = cantidadIngresada;
                    lote.CantidadDisponible_013AL = cantidadIngresada;
                    int diasRestantes = (fechaVencimiento.Date - DateTime.Today).Days;
                    lote.Estado_013AL = diasRestantes <= 30 ? "Próximo a vencer" : "Disponible";

                    loteBLL.AgregarLote_013AL(lote);

                    productoBLL.ActualizarStockDesdeLotes_013AL(codProducto);
                }

                ocbll.ActualizarEstadoOrden_013AL(
                    Convert.ToInt32(comboBoxOrdenesCompra.SelectedValue));

                if (!huboRecepcion)
                {
                    MessageBox.Show("No hay cantidades para recibir.");
                    return;
                }

                MessageBox.Show("Recepción registrada correctamente.");

                CargarOrdenesCompra_013AL();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al registrar la recepción.\n\n" + ex.Message);
            }

        }

    }
}
