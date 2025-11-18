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
        FacturaBLL_013AL fbll = new FacturaBLL_013AL();
        DetalleBLL_013AL dbll = new DetalleBLL_013AL();
        ClienteBLL_013AL cbll = new ClienteBLL_013AL();


        public GenerarFactura_013AL()
        {
            InitializeComponent();            
            btnProductos.Enabled = true; 
            btnBuscarCliente.Enabled = false; 
            btnCobrar.Enabled = false;
            btnFactura.Enabled = false;
            button1.Enabled = false;
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }
        private List<Detalle_013AL> carritoTemporal = new List<Detalle_013AL>();

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
            //NUEVO
            if (!facturaCobrada && carritoTemporal.Count > 0)
            {
                foreach (var item in carritoTemporal)
                {
                    prbll.RevertirStock_013AL(item.CodProducto_013AL, item.Cantidad_013AL);
                }
            }
            //NUEVO
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
                    LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaColumnas_013AL(dataGridView2, this.Name);
                    MessageBox.Show("¡Cliente encontrado!" + cliente.Nombre_013AL + " " + cliente.Apellido_013AL, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    btnCobrar.Enabled = true; // sólo ahora se habilita el cobro
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar cliente.");
            }
        }
        string numeroTarjeta;
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            CobrarVenta_013AL form = new CobrarVenta_013AL(Convert.ToInt32(txtTotal.Text), Convert.ToInt32(textBox1.Text), Convert.ToInt32(compraId.Value), carritoTemporal);
            form.ShowDialog();
            
                numeroTarjeta = form.Tag as string; 

                btnProductos.Enabled = false;

                btnBuscarCliente.Enabled = false;

            btnFactura.Enabled = true;

            facturaCobrada = true;

            carritoTemporal.Clear();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (carritoTemporal.Count > 0)
            {
                var listaFinal = carritoTemporal
                    .GroupBy(p => p.CodProducto_013AL)
                    .Select(g =>
                    {
                        var item = g.First();
                        item.Cantidad_013AL = g.Sum(x => x.Cantidad_013AL);
                        // No es necesario recalcular subtotal, se hace automáticamente en la propiedad
                        return item;
                    })
                    .ToList();

                dataGridView1.DataSource = null;
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CodCompra", DataPropertyName = "CodCompra_013AL", Name = "CodCompra_013AL" });
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CodProducto", DataPropertyName = "CodProducto_013AL", Name = "CodProducto_013AL" });
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cantidad", DataPropertyName = "Cantidad_013AL", Name = "Cantidad_013AL" });
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Precio Unitario", DataPropertyName = "PrecioUnitario_013AL", Name = "PrecioUnitario_013AL" });
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Subtotal", DataPropertyName = "Subtotal_013AL", Name = "Subtotal_013AL" });

                
                dataGridView1.DataSource = listaFinal;

                LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaColumnas_013AL(dataGridView1, this.Name);

                decimal total = listaFinal.Sum(p => p.Subtotal_013AL);
                txtTotal.Text = total.ToString();
            }
            else
            {
                MessageBox.Show("No hay productos agregados.");
            }

            /* var productosCompra = blln.ListarCompraProducto_013AL().Where(p => p.CodCompra_013AL == compraId.Value).ToList();

            if (productosCompra.Count > 0)
            {
                
                foreach (var prod in productosCompra)
                {
                    if (!carritoTemporal.Any(p => p.CodCompra_013AL == prod.CodCompra_013AL && p.CodProducto_013AL == prod.CodProducto_013AL))
                        carritoTemporal.Add(prod);
                }*/


            /*var listaActual = (List<Detalle_013AL>)dataGridView1.DataSource ?? new List<Detalle_013AL>();
            listaActual.AddRange(productosCompra);


            var listaFinal = listaActual.Distinct().ToList();


            dataGridView1.DataSource = null; 
            dataGridView1.AutoGenerateColumns = false;

            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CodCompra", DataPropertyName = "CodCompra_013AL", Name = "CodCompra_013AL" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CodProducto", DataPropertyName = "CodProducto_013AL", Name = "CodProducto_013AL" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cantidad", DataPropertyName = "Cantidad_013AL", Name = "Cantidad_013AL" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Precio Unitario", DataPropertyName = "PrecioUnitario_013AL", Name = "PrecioUnitario_013AL" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Subtotal", DataPropertyName = "Subtotal_013AL", Name = "Subtotal_013AL" }); // calculado

            dataGridView1.DataSource = listaFinal;


            decimal total = listaFinal.Sum(p => p.Subtotal_013AL);
            txtTotal.Text = total.ToString();*/

            /*  if (carritoTemporal.Count > 0)
              {
                  var listaFinal = carritoTemporal.Distinct().ToList();

                  dataGridView1.DataSource = null;
                  dataGridView1.AutoGenerateColumns = false;

                  dataGridView1.Columns.Clear();
                  dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CodCompra", DataPropertyName = "CodCompra_013AL", Name = "CodCompra_013AL" });
                  dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "CodProducto", DataPropertyName = "CodProducto_013AL", Name = "CodProducto_013AL" });
                  dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Cantidad", DataPropertyName = "Cantidad_013AL", Name = "Cantidad_013AL" });
                  dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Precio Unitario", DataPropertyName = "PrecioUnitario_013AL", Name = "PrecioUnitario_013AL" });
                  dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Subtotal", DataPropertyName = "Subtotal_013AL", Name = "Subtotal_013AL" });

                  dataGridView1.DataSource = listaFinal;

                  decimal total = listaFinal.Sum(p => p.Subtotal_013AL);
                  txtTotal.Text = total.ToString();
              }
              else
              {
                  MessageBox.Show("No hay productos agregados.");
              }


          }
          else
          {
              MessageBox.Show("No se encontraron productos para esta compra.");
          }*/


        }

        private void btnFactura_Click(object sender, EventArgs e)
        {
            
            
                var comprasCliente = fbll.ListarFactura_013AL();
                var comprasProducto = dbll.ListarDetalle_013AL();

                var compraCliente = comprasCliente.FirstOrDefault(r => r.CodCompra_013AL == compraId.Value);
                var compraProductos = comprasProducto.Where(r => r.CodCompra_013AL == compraId.Value).ToList();

                if (compraCliente != null && compraProductos.Count > 0)
                {
                   
                    dataGridView1.DataSource = compraProductos;
                    LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaColumnas_013AL(dataGridView1, this.Name);


                GenerarFacturaPDF_013AL(compraCliente, compraProductos, numeroTarjeta);
                }
                else
                {
                    
                    dataGridView1.DataSource = null;
                    MessageBox.Show("Compra no encontrada.");
                }
            
            
            try
            {
                string resultado;
                EventoBLL_013AL bbll = new EventoBLL_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                resultado = bbll.AgregarEvento_013AL(user.Login_013AL, "Ventas", "Generar Factura", 3);
            }
            catch (Exception ex) { }


        }
        private void GenerarFacturaPDF_013AL(Factura_013AL compraCliente, List<Detalle_013AL> compraProductos, string numt)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF|*.pdf";
            saveFileDialog.Title = "Guardar Factura";
            saveFileDialog.FileName = "Factura_" + compraCliente.CodCompra_013AL + ".pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    decimal baseImponible = compraProductos.Sum(p => p.Cantidad_013AL * p.PrecioUnitario_013AL);
                    decimal iva = baseImponible * 0.21m;
                    decimal totalConIVA = baseImponible + iva;

                    // Encabezado
                    pdfDoc.Add(new Paragraph("Dietetica Eat Healthy"));
                    //pdfDoc.Add(new Paragraph("Calle Llacuna, 132, Barcelona (08022), España"));
                    //pdfDoc.Add(new Paragraph("tomas.vinas@tuftretail.com"));
                    //pdfDoc.Add(new Paragraph("678644309"));
                    pdfDoc.Add(new Paragraph(" ")); // Espacio en blanco

                    // Datos de la factura
                    pdfDoc.Add(new Paragraph($"FACTURA #{compraCliente.CodCompra_013AL}"));
                    pdfDoc.Add(new Paragraph($"F. de Emi.: {compraCliente.Fecha_013AL.ToShortDateString()}"));
                    pdfDoc.Add(new Paragraph($"Vencimiento: {compraCliente.Fecha_013AL.AddDays(30).ToShortDateString()}"));
                    pdfDoc.Add(new Paragraph($"CUIL Cliente: {compraCliente.CUIL_013AL}"));
                    //pdfDoc.Add(new Paragraph($"Total: {compraCliente.Total_013AL:C}"));
                    pdfDoc.Add(new Paragraph($"Método de Pago: {compraCliente.MetPago_013AL}"));
                    if (numt != "0")
                    {
                        pdfDoc.Add(new Paragraph($"Número de Tarjeta: {numt}"));
                    }
                    pdfDoc.Add(new Paragraph(" ")); // Espacio en blanco

                    // Tabla de detalles

                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1, 1, 1});
                    // Encabezado de la tabla
                    
                    table.AddCell("Prod.");
                    table.AddCell("Cant.");
                    table.AddCell("Precio Unitario");

                    foreach (var item in compraProductos)
                    {
                        string nombreProducto = prbll.ObtenerNombreProducto_013AL(item.CodProducto_013AL);
                        table.AddCell(nombreProducto); 
                        table.AddCell(item.Cantidad_013AL.ToString());
                        table.AddCell(item.PrecioUnitario_013AL.ToString("C"));
                    }
                    
                        pdfDoc.Add(table);

                    // Totales
                    pdfDoc.Add(new Paragraph($"Base Imponible: {baseImponible.ToString("C", CultureInfo.CreateSpecificCulture("es-AR"))}"));
                    pdfDoc.Add(new Paragraph($"IVA (21%): {iva.ToString("C", CultureInfo.CreateSpecificCulture("es-AR"))}"));
                    pdfDoc.Add(new Paragraph($"Total con IVA: {totalConIVA.ToString("C", CultureInfo.CreateSpecificCulture("es-AR"))}"));


                    pdfDoc.Close();
                }

                MessageBox.Show("Factura generada correctamente");
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int compraid = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CodCompra_013AL"].Value);
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
