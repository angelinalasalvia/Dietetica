using BE_013AL;
using BLL;
using BLL_013AL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Servicios;
using Servicios_013AL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace UI
{
    public partial class GenerarFactura_013AL : Form, IObserver_013AL
    {
        ProductoBLL_013AL prbll = new ProductoBLL_013AL();
        PedidoBLL_013AL pbll = new PedidoBLL_013AL();
        DetalleBLL_013AL dbll = new DetalleBLL_013AL();
        ClienteBLL_013AL cbll = new ClienteBLL_013AL();


        public GenerarFactura_013AL()
        {
            InitializeComponent();            
            btnProductos.Enabled = true; 
            btnBuscarCliente.Enabled = false; 
            button1.Enabled = false;
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }
        
        public void ActualizarIdioma_013AL()
        {
            var lm = LanguageManager_013AL.ObtenerInstancia_013AL();
            lm.CambiarIdiomaControles_013AL(this);

            lm.CambiarIdiomaColumnas_013AL(dataGridView1, this.Name);
            lm.CambiarIdiomaColumnas_013AL(dataGridView2, this.Name);
        }
        private int? compraId;
        private bool facturaCobrada = false;
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            if (!compraId.HasValue)
            {
                compraId = dbll.ObtenerSiguienteIdCompra_013AL();
            }

            SeleccionarProducto_013AL form = new SeleccionarProducto_013AL(compraId.Value, carritoTemporal);
            form.ShowDialog();
            button1.Enabled = true;
            
            btnBuscarCliente.Enabled = true;
            
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            UsuarioBLL_013AL bll = new UsuarioBLL_013AL();

            try
            {
                int cuil = Convert.ToInt32(textBox1.Text);
                var cliente = cbll.BuscarClientePorCUIL_013AL(cuil); // nuevo método

                if (cliente == null)
                {
                    DialogResult result = MessageBox.Show("Cliente no encontrado. ¿Desea registrarlo?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        RegistrarCliente_013AL form = new RegistrarCliente_013AL();
                        form.ShowDialog();
                    }
                }
                else
                {
                    dataGridView2.DataSource = new List<Cliente_013AL> { cliente };
                    MessageBox.Show("¡Cliente encontrado!" + cliente.Nombre_013AL + " " + cliente.Apellido_013AL, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    btnCobrar.Enabled = true; // sólo ahora se habilita el cobro
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message); 
                MessageBox.Show(ex.Message + "\n\n" + ex.InnerException?.Message);
            }



                
                dataGridView1.DataSource = listaFinal;

                LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaColumnas_013AL(dataGridView1, this.Name);

                decimal total = listaFinal.Sum(p => p.Subtotal_013AL);
                txtTotal.Text = total.ToString();
            }
            else
            {
                MessageBox.Show("No hay productos agregados.");
            }











            //???

            /*if (!compraId.HasValue)
            {
                compraId = dbll.ObtenerSiguienteIdCompra_013AL();

                PedidoBLL_013AL pbll = new PedidoBLL_013AL();

                Pedido_013AL pedido = new Pedido_013AL
                {
                    Fecha_013AL = DateTime.Now,
                    Estado_013AL = "Pendiente",
                    Total_013AL = 0
                };

                compraId = pbll.CrearPedido_013AL(pedido);
            }
            SeleccionarProducto_013AL form = new SeleccionarProducto_013AL(compraId.Value);*/
            /*SeleccionarProducto_013AL form = new SeleccionarProducto_013AL(compraId.Value);
            form.ShowDialog();
            button1.Enabled = true;
            
            btnBuscarCliente.Enabled = true;*/

        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            UsuarioBLL_013AL bll = new UsuarioBLL_013AL();

            try
            {
                int cuil = Convert.ToInt32(textBox1.Text);
                var cliente = cbll.BuscarClientePorCUIL_013AL(cuil); 

                if (cliente == null)
                {
                   
                    dataGridView1.DataSource = compraProductos;
                    LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaColumnas_013AL(dataGridView1, this.Name);


                GenerarFacturaPDF_013AL(compraCliente, compraProductos, numeroTarjeta);
                }
                else
                {
                    dataGridView2.DataSource = new List<Cliente_013AL> { cliente };
                    MessageBox.Show("¡Cliente encontrado!" + cliente.Nombre_013AL + " " + cliente.Apellido_013AL, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    pbll.AsignarClientePedido_013AL(compraId.Value, cliente.CUIL_013AL);
                    MessageBox.Show("¡Pedido creado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente.");
            }
        }
        string numeroTarjeta;
        
        private void button1_Click(object sender, EventArgs e)
        {
            try { 
                if (!compraId.HasValue) 
                { MessageBox.Show("No hay pedido seleccionado."); 
                    return; 
                } 
                var lista = dbll.ListarDetalle_013AL().Where(x => x.CodCompra_013AL == compraId.Value).ToList(); 
                if (lista.Count == 0) 
                { 
                    MessageBox.Show("No hay productos en el pedido."); 
                    return; 
                } 
                dataGridView1.DataSource = null; dataGridView1.AutoGenerateColumns = true; 
                dataGridView1.DataSource = lista; 
                decimal total = lista.Sum(x => x.Cantidad_013AL * x.PrecioUnitario_013AL); 
                txtTotal.Text = total.ToString(); 
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try 
            { 
                if (dataGridView1.SelectedRows.Count == 0) 
                { 
                    MessageBox.Show("Seleccione un producto."); 
                    return; 
                } 
                int codCompra = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CodCompra_013AL"].Value); 
                int codProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CodProducto_013AL"].Value);
                int cantidad = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Cantidad_013AL"].Value);

                var confirmResult = MessageBox.Show(
                    "¿Estás seguro de que deseas eliminar este producto?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo
                );

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        // Revertir stock
                        prbll.RevertirStock_013AL(codProducto, cantidad);

                        // Eliminar del carrito temporal
                        carritoTemporal.RemoveAll(p => p.CodProducto_013AL == codProducto && p.CodCompra_013AL == compraid);

                        MessageBox.Show("Producto eliminado con éxito.");

                        // Actualizar DataGridView
                        dataGridView1.DataSource = null;
                        dataGridView1.Columns.Clear();
                        dataGridView1.AutoGenerateColumns = false;

                        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CodCompra", DataPropertyName = "CodCompra_013AL", Name = "CodCompra_013AL" });
                        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CodProducto", DataPropertyName = "CodProducto_013AL", Name = "CodProducto_013AL" });
                        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cantidad", DataPropertyName = "Cantidad_013AL", Name = "Cantidad_013AL" });
                        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Precio Unitario", DataPropertyName = "PrecioUnitario_013AL", Name = "PrecioUnitario_013AL" });
                        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Subtotal", DataPropertyName = "Subtotal_013AL", Name = "Subtotal_013AL" });

                        dataGridView1.DataSource = carritoTemporal;
                        LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaColumnas_013AL(dataGridView1, this.Name);


                        // Actualizar total
                        decimal total = carritoTemporal.Sum(p => p.Subtotal_013AL);
                        txtTotal.Text = total.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar el producto: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para eliminar.");
            }
        }
    }
}
