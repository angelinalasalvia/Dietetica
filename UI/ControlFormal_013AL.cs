using BE_013AL;
using BLL;
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
    public partial class ControlFormal_013AL : Form
    {
        public ControlFormal_013AL()
        {
            InitializeComponent();
        }
        PedidoBLL_013AL pBLL = new PedidoBLL_013AL();
        DetalleBLL_013AL dBLL = new DetalleBLL_013AL();
        ClienteBLL_013AL cBLL = new ClienteBLL_013AL();
        ProductoBLL_013AL prBLL = new ProductoBLL_013AL();
        private void ControlFormal_013AL_Load(object sender, EventArgs e)
        {
            CargarPedidosPendientes();
        }
        private void CargarPedidosPendientes() 
        {
            DataGridViewPedidos.AutoGenerateColumns = true; 
            DataGridViewPedidos.DataSource = null; 
            DataGridViewPedidos.DataSource = pBLL.ListarPedidosPendientes_013AL();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (DataGridViewPedidos.SelectedRows.Count == 0) 
                { 
                    MessageBox.Show("Seleccione un pedido."); 
                    return; 
                } 
                int codCompra = Convert.ToInt32(DataGridViewPedidos.SelectedRows[0].Cells[0].Value); 
                DialogResult r = MessageBox.Show("¿Aprobar pedido?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question); 
                if (r == DialogResult.Yes) 
                { 
                    pBLL.ActualizarEstadoPedido_013AL(codCompra, "Aprobado"); 
                    MessageBox.Show("Pedido aprobado."); 
                    CargarPedidosPendientes(); 
                    DataGridViewDetalle.DataSource = null; 
                    txtCUIL.Clear(); 
                    txtNombre.Clear(); 
                    txtApellido.Clear(); 
                } 
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (DataGridViewPedidos.SelectedRows.Count == 0) 
                { 
                    MessageBox.Show("Seleccione un pedido."); 
                    return; 
                } 
                int codCompra = Convert.ToInt32(DataGridViewPedidos.SelectedRows[0].Cells[0].Value); 
                var detalle = dBLL.ListarDetallePorCompra_013AL(codCompra); 
                DataGridViewDetalle.DataSource = null; 
                DataGridViewDetalle.DataSource = detalle; 
                Pedido_013AL pedido = pBLL.BuscarPedidoPorId_013AL(codCompra); 
                Cliente_013AL cliente = cBLL.BuscarClientePorCUIL_013AL(pedido.CUIL_013AL); 
                txtCUIL.Text = cliente.CUIL_013AL.ToString(); 
                txtNombre.Text = cliente.Nombre_013AL; 
                txtApellido.Text = cliente.Apellido_013AL; 
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (DataGridViewPedidos.SelectedRows.Count == 0) 
                { 
                    MessageBox.Show("Seleccione un pedido."); 
                    return; 
                } 
                int codCompra = Convert.ToInt32(DataGridViewPedidos.SelectedRows[0].Cells[0].Value); 
                DialogResult r = MessageBox.Show("¿Rechazar pedido?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question); 
                if (r == DialogResult.Yes) 
                { 
                    List<Detalle_013AL> detalles = dBLL.ListarDetallePorCompra_013AL(codCompra); 
                    foreach (var item in detalles) 
                    { 
                        prBLL.RevertirStock_013AL(item.CodProducto_013AL, item.Cantidad_013AL); 
                    } 
                    pBLL.ActualizarEstadoPedido_013AL(codCompra, "Rechazado"); 
                    MessageBox.Show("Pedido rechazado."); 
                    CargarPedidosPendientes(); 
                    DataGridViewDetalle.DataSource = null; 
                    txtCUIL.Clear(); 
                    txtNombre.Clear(); 
                    txtApellido.Clear(); 
                } 
            } catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }
        }
    }
}
