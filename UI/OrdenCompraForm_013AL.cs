using BE_013AL;
using BLL_013AL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Servicios_013AL;

namespace UI
{
    public partial class OrdenCompraForm_013AL : Form, IObserver_013AL
    {
        public OrdenCompraForm_013AL()
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
        NegocioBLL_013AL bll = new NegocioBLL_013AL();
        private void button1_Click(object sender, EventArgs e)
        {
            Proveedores_013AL proveedores = new Proveedores_013AL();
            proveedores.ShowDialog();
        }

        private void OrdenCompra_Load(object sender, EventArgs e)
        {
            CargarCboCOD_013AL();
            ConfigurarDataGridView_013AL();
        }
        private class ComboBoxItem
        { 
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        public void CargarCboCOD_013AL()
        {
            var listacod = bll.ListarSCotizacion_013AL();
            comboBox1.Items.Clear();

            foreach (var cod in listacod)
            {
                string detalle = $"{cod.CodSCotizacion_013AL}";

                comboBox1.Items.Add(new ComboBoxItem
                {
                    Text = detalle,
                    Value = cod
                });
            }
        }
        public void CargarProveedor_013AL()
        {
            if (comboBox1.SelectedItem is ComboBoxItem selectedItem)
            {
                var SolicitudSeleccionada = (SolicitudCotizacion_013AL)selectedItem.Value;
                int cuitProveedor = SolicitudSeleccionada.CUITProveedor_013AL;

                textBox1.Text = Convert.ToString(cuitProveedor);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor válido.");
            }
        }

        private void ConfigurarDataGridView_013AL()
        {
            // Limpia las columnas si ya existen
            dataGridView1.Columns.Clear();

            // Añade columnas al DataGridView
            dataGridView1.Columns.Add("IdProducto", "ID Producto");
            dataGridView1.Columns.Add("Nombre", "Nombre Producto");
            dataGridView1.Columns.Add("Cantidad", "Cantidad");
            dataGridView1.Columns.Add("PrecioUnitario", "Precio Unitario");

            // Formato para la columna de Precio Unitario
            dataGridView1.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C2";
        }

        private void ActualizarDataGridView_013AL()
        {
            dataGridView1.Rows.Clear();

            /*if (!dataGridView1.Columns.Contains("PrecioUnitario"))
            {
                DataGridViewTextBoxColumn precioColumn = new DataGridViewTextBoxColumn();
                precioColumn.Name = "PrecioUnitario";
                precioColumn.HeaderText = "Precio Unitario";
                precioColumn.ValueType = typeof(int);
                precioColumn.DefaultCellStyle.Format = "C2"; 
                dataGridView1.Columns.Add(precioColumn);
            }
            */
            
            if (comboBox1.SelectedItem is ComboBoxItem selectedItem)
            {
                var solicitudSeleccionada = (SolicitudCotizacion_013AL)selectedItem.Value;
                int codsc = solicitudSeleccionada.CodSCotizacion_013AL;

                var detalleSolicitud = bll.ListarProductosOC_013AL(codsc);

                if (detalleSolicitud != null)
                {
                    var producto = bll.ObtenerProductoPorId_013AL(detalleSolicitud.CodProducto_013AL);

                    if (producto != null)
                    {
                        dataGridView1.Rows.Add(producto.CodProducto_013AL, producto.Nombre_013AL, detalleSolicitud.Cantidad_013AL, producto.Precio_013AL);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el producto.");
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró ningún detalle de productos para la solicitud seleccionada.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un código válido.");
            }
        }

        /*private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "PrecioUnitario")
            {
                
                if (!decimal.TryParse(e.FormattedValue.ToString(), out _))
                {
                    MessageBox.Show("Por favor, ingresa un valor numérico válido para el precio unitario.");
                    e.Cancel = true; 
                }
            }
        }*/
      

        private void button2_Click(object sender, EventArgs e)
        {
            GuardarOrdenCompraEnBaseDeDatos_013AL();


            BLLBitacora_013AL bbll = new BLLBitacora_013AL();
            Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
            bbll.AgregarEvento_013AL(user.Login_013AL, "OrdenCompra", "Generar Orden Compra", 2);
        }

        private void GuardarOrdenCompraEnBaseDeDatos_013AL()
        {
            try
            {
                
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Por favor, selecciona un proveedor.");
                    return;
                }

                // Obtener los valores necesarios
                if (comboBox1.SelectedItem is ComboBoxItem selectedItem)
                {
                    var solicitudSeleccionada = (SolicitudCotizacion_013AL)selectedItem.Value;
                    int idSolicitud = solicitudSeleccionada.CodSCotizacion_013AL;
                    int cuitProveedor = Convert.ToInt32(textBox1.Text);
                    
                    int total = 0;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["PrecioUnitario"].Value != null && row.Cells["Cantidad"].Value != null)
                        {
                            int precioUnitario = Convert.ToInt32(row.Cells["PrecioUnitario"].Value);
                            int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                            total += precioUnitario * cantidad;
                        }
                    }

                    OrdenCompra_013AL nuevaOrden = new OrdenCompra_013AL
                    {
                        CodSolicitud_013AL = idSolicitud,
                        CUITProveedor_013AL = cuitProveedor,
                        Total_013AL = total 
                    };

                    
                    bll.GuardarOrdenCompra_013AL(nuevaOrden); 

                    int codOrdenCompra = bll.ObtenerCodOrdenCompra_013AL(nuevaOrden.CodSolicitud_013AL, nuevaOrden.CUITProveedor_013AL, DateTime.Now);

                    GenerarPDFOrdenCompra_013AL(codOrdenCompra, nuevaOrden, total);
                    
                }
                else
                {
                    MessageBox.Show("Por favor, selecciona una solicitud válida.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al guardar la orden de compra: " + ex.Message);
            }
        }

        private void GenerarPDFOrdenCompra_013AL(int codOrdenCompra, OrdenCompra_013AL orden, int totalGeneral)
        {
            try
            {
                // Ruta donde se guardará el PDF
                string pdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"OrdenCompra_{codOrdenCompra}.pdf");

                // Crear el documento PDF
                Document document = new Document();
                PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create));
                document.Open();

                // Título y detalles generales
                document.Add(new Paragraph("Orden de Compra"));
                document.Add(new Paragraph($"Fecha: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph($"Código de Orden: {codOrdenCompra}"));
                document.Add(new Paragraph(" "));

                // Información del Proveedor
                document.Add(new Paragraph("Datos del Proveedor:"));
                document.Add(new Paragraph($"CUIT: {orden.CUITProveedor_013AL}"));
                Proveedor_013AL proveedor = bll.ObtenerDatosProveedor_013AL(codOrdenCompra);
                document.Add(new Paragraph($"Razón Social: {proveedor.RazonSocial_013AL}"));
                document.Add(new Paragraph($"Nombre Completo: {proveedor.ApellidoProveedor_013AL} {proveedor.NombreProveedor_013AL}"));
                document.Add(new Paragraph($"Telefono: {proveedor.Telefono_013AL}"));
                document.Add(new Paragraph($"Mail: {proveedor.Mail_013AL}"));
                document.Add(new Paragraph(" "));

                // Información de la Empresa
                document.Add(new Paragraph("Datos de la Empresa:"));
                document.Add(new Paragraph("Nombre Empresa: Eat Healthy"));
                document.Add(new Paragraph("Dirección de entrega: Av. Hipólito Yrigoyen 9963"));
                document.Add(new Paragraph("Código Postal: B1834"));
                document.Add(new Paragraph(" "));

                // Crear la tabla de productos
                PdfPTable table = new PdfPTable(5);
                table.AddCell("ID Producto");
                table.AddCell("Nombre Producto");
                table.AddCell("Cantidad");
                table.AddCell("Precio Unitario");
                table.AddCell("Total");

                // Agregar los datos de productos desde el DataGridView
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["IdProducto"].Value != null && row.Cells["Nombre"].Value != null &&
                        row.Cells["Cantidad"].Value != null && row.Cells["PrecioUnitario"].Value != null)
                    {
                        string idProducto = row.Cells["IdProducto"].Value.ToString();
                        string nombreProducto = row.Cells["Nombre"].Value.ToString();
                        int cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value);
                        decimal precioUnitario = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value);
                        decimal totalIndividual = cantidad * precioUnitario;

                        // Agregar los datos a la tabla
                        table.AddCell(idProducto);
                        table.AddCell(nombreProducto);
                        table.AddCell(cantidad.ToString());
                        table.AddCell(precioUnitario.ToString("C2"));
                        table.AddCell(totalIndividual.ToString("C2"));
                    }
                }

                // Agregar la tabla al documento
                document.Add(table);

                // Total general al final
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph($"Total: {totalGeneral.ToString("C2")}"));

                // Cerrar el documento
                document.Close();

                MessageBox.Show("PDF de la Orden de Compra generado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al generar el PDF: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ActualizarDataGridView_013AL();
            CargarProveedor_013AL();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PagarProducto_013AL form = new PagarProducto_013AL();
            form.ShowDialog();
        }
    }
}
