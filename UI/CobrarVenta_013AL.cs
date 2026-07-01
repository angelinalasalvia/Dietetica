using BE_013AL;
using BE_013AL.Composite;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UI
{
    public partial class CobrarVenta_013AL : Form, IObserver_013AL
    {
        PedidoBLL_013AL blln = new PedidoBLL_013AL();
        ClienteBLL_013AL cbll = new ClienteBLL_013AL();
        DetalleBLL_013AL dbll = new DetalleBLL_013AL();
        ProductoBLL_013AL prbll = new ProductoBLL_013AL();

        private int pedidoSeleccionado; 
        private Pedido_013AL pedidoActual; 
        private Cliente_013AL clienteActual;
        private List<Detalle_013AL> detallesActuales;

        public CobrarVenta_013AL()
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
        private void CargarPedidosAprobados() 
        { 
            dataGridView1.DataSource = null; 
            dataGridView1.DataSource = blln.ListarPedidosAprobados_013AL(); 
        }
        private void CobrarVenta_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Efectivo"); 
            comboBox1.Items.Add("Tarjeta Débito"); 
            txtTarjeta.Enabled = false; CargarPedidosAprobados();
            ActualizarIdioma_013AL();
        }
        private List<Detalle_013AL> detallesVenta;
        Usuarios_013AL user;
        EventoBLL_013AL bbll = new EventoBLL_013AL();
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (pedidoActual == null) 
                { 
                    MessageBox.Show("Seleccione un pedido."); 
                    return; 
                } 
                if (comboBox1.Text == "") 
                { 
                    MessageBox.Show("Seleccione método de pago."); 
                    return; 
                } 
                if (comboBox1.Text == "Tarjeta Débito") 
                { 
                    if (txtTarjeta.Text.Length != 16 || !txtTarjeta.Text.All(char.IsDigit)) 
                    { 
                        MessageBox.Show("Tarjeta inválida."); 
                        return; 
                    } 
                } 
                blln.ActualizarMetodoPago_013AL(pedidoSeleccionado, comboBox1.Text); 
                blln.ActualizarEstadoPedido_013AL(pedidoSeleccionado, "Cobrado"); 
                MessageBox.Show("Venta cobrada correctamente."); 
                btnFactura.Enabled = true; 
                CargarPedidosAprobados();
                try
                {
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Pedido", $"Pedido número {pedidoActual.CodCompra_013AL} cobrado.", 3);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }
        }
        private void btnFactura_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (pedidoActual == null) 
                { 
                    MessageBox.Show("Debe seleccionar un pedido."); 
                    return; 
                } 
                if (detallesActuales == null || detallesActuales.Count == 0) 
                { 
                    MessageBox.Show("No hay detalles.");
                    return; 
                } 
                string numeroTarjeta = "0"; 
                if (comboBox1.Text == "Tarjeta Débito") 
                { 
                    numeroTarjeta = txtTarjeta.Text; 
                } 
                GenerarFacturaPDF_013AL(pedidoActual, clienteActual, detallesActuales, numeroTarjeta); 
                MessageBox.Show("Factura generada correctamente."); 
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
                if (dataGridView1.SelectedRows.Count == 0) 
                { 
                    MessageBox.Show("Seleccione un pedido."); 
                    return; 
                } 
                pedidoSeleccionado = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CodCompra_013AL"].Value); 
                pedidoActual = blln.BuscarPedidoPorId_013AL(pedidoSeleccionado); 
                clienteActual = cbll.BuscarClientePorCUIL_013AL(pedidoActual.CUIL_013AL); 
                detallesActuales = dbll.ListarDetallePorCompra_013AL(pedidoSeleccionado); 
                txtcuil.Text = clienteActual.CUIL_013AL.ToString();
                txttotal.Text = pedidoActual.Total_013AL.ToString("0.00");
                MessageBox.Show("Pedido cargado."); 
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message); 
            }
        }

        private void GenerarFacturaPDF_013AL(Pedido_013AL pedido, Cliente_013AL cliente, List<Detalle_013AL> detalles, string numt)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF|*.pdf";
            saveFileDialog.Title = "Guardar Factura";
            saveFileDialog.FileName = "Factura_" + pedido.CodCompra_013AL + ".pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    decimal total = detalles.Sum(p => p.Cantidad_013AL * p.PrecioUnitario_013AL);

                    // Encabezado
                    pdfDoc.Add(new Paragraph("Dietetica Eat Healthy"));
                    pdfDoc.Add(new Paragraph(" ")); 

                    // Datos de la factura
                    pdfDoc.Add(new Paragraph($"FACTURA #{pedido.CodCompra_013AL}"));
                    pdfDoc.Add(new Paragraph($"F. de Emi.: {pedido.Fecha_013AL.ToShortDateString()}"));
                    pdfDoc.Add(new Paragraph($"CUIL Cliente: {cliente.CUIL_013AL}"));
                    pdfDoc.Add(new Paragraph($"Método de Pago: {pedido.MetPago_013AL}"));
                    if (numt != "0")
                    {
                        pdfDoc.Add(new Paragraph($"Número de Tarjeta: {numt}"));
                    }
                    pdfDoc.Add(new Paragraph(" ")); 

                    // Tabla de detalles
                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1, 1, 1 });
                    
                    table.AddCell("Prod.");
                    table.AddCell("Cant.");
                    table.AddCell("Precio Unitario");

                    foreach (var item in detalles)
                    {
                        string nombreProducto = prbll.ObtenerNombreProducto_013AL(item.CodProducto_013AL);
                        table.AddCell(nombreProducto);
                        table.AddCell(item.Cantidad_013AL.ToString());
                        table.AddCell(item.PrecioUnitario_013AL.ToString("C"));
                    }

                    pdfDoc.Add(table);

                    pdfDoc.Add(new Paragraph($"TOTAL: " + total.ToString("C", CultureInfo.CreateSpecificCulture("es-AR"))));

                    pdfDoc.Close();
                }

                MessageBox.Show("Factura generada correctamente");
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            txtTarjeta.Enabled = comboBox1.Text == "Tarjeta Débito";
        }
    }
}
