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
    public partial class RegistrarPedido_013AL : Form, IObserver_013AL
    {
        ProductoBLL_013AL prbll = new ProductoBLL_013AL();
        PedidoBLL_013AL pbll = new PedidoBLL_013AL();
        DetalleBLL_013AL dbll = new DetalleBLL_013AL();
        ClienteBLL_013AL cbll = new ClienteBLL_013AL();


        private int? compraId;

        public RegistrarPedido_013AL()
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
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (!compraId.HasValue) 
                { 
                    compraId = dbll.ObtenerSiguienteIdCompra_013AL(); 
                    Pedido_013AL pedido = new Pedido_013AL() 
                    { 
                        CodCompra_013AL = compraId.Value, 
                        Fecha_013AL = DateTime.Now, 
                        Estado_013AL = "Pendiente"
                    }; 
                    pbll.CrearPedido_013AL(pedido); 
                } 
                SeleccionarProducto_013AL form = new SeleccionarProducto_013AL(compraId.Value); 
                form.ShowDialog(); 
                button1.Enabled = true; 
                btnBuscarCliente.Enabled = true; 
            } catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }

        }
        Usuarios_013AL user;
        EventoBLL_013AL bbll = new EventoBLL_013AL();
        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            UsuarioBLL_013AL bll = new UsuarioBLL_013AL();

            try
            {
                int cuil = Convert.ToInt32(textBox1.Text);
                var cliente = cbll.BuscarClientePorCUIL_013AL(cuil); 

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
                    pbll.AsignarClientePedido_013AL(compraId.Value, cliente.CUIL_013AL);
                    MessageBox.Show("¡Cliente encontrado!" + cliente.Nombre_013AL + " " + cliente.Apellido_013AL, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    MessageBox.Show("Pedido registrado con éxito", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    try
                    {
                        user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Pedido", $"Pedido número {compraId.Value} creado.", 3);
                    }
                    catch (Exception ex) { Console.WriteLine(ex); }
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
            try 
            { 
                if (!compraId.HasValue) 
                { 
                    MessageBox.Show("No hay pedido seleccionado."); 
                    return; 
                } 
                var lista = dbll.ListarDetalle_013AL().Where(x => x.CodCompra_013AL == compraId.Value).ToList(); 
                if (lista.Count == 0) 
                { 
                    MessageBox.Show("No hay productos en el pedido."); 
                    return; 
                } 
                dataGridView1.DataSource = null; 
                dataGridView1.AutoGenerateColumns = true; 
                dataGridView1.DataSource = lista; 
                decimal total = lista.Sum(x => x.Cantidad_013AL * x.PrecioUnitario_013AL); 
                txtTotal.Text = total.ToString(); 
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
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
                DialogResult r = MessageBox.Show("¿Eliminar producto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question); 
                if (r == DialogResult.Yes) 
                { 
                    prbll.RevertirStock_013AL(codProducto, cantidad); 
                    dbll.EliminarDetalle_013AL(codCompra, codProducto); 
                    var lista = dbll.ListarDetalle_013AL().Where(x => x.CodCompra_013AL == compraId.Value).ToList(); 
                    dataGridView1.DataSource = null; 
                    dataGridView1.DataSource = lista; 
                    decimal total = lista.Sum(x => x.Cantidad_013AL * x.PrecioUnitario_013AL); 
                    txtTotal.Text = total.ToString(); 
                    pbll.ActualizarTotalPedido_013AL(compraId.Value, total); 
                    MessageBox.Show("Producto eliminado."); 
                } 
            } 
            catch (Exception ex) { 
                MessageBox.Show(ex.Message); 
            }
        }
    }
}
