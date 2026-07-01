using BE_013AL;
using BLL_013AL;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using Servicios_013AL;
using BLL;

namespace UI
{
    public partial class Cotizaciones_013AL : Form, IObserver_013AL
    {
        public Cotizaciones_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }

        ProveedorBLL_013AL pbll = new ProveedorBLL_013AL();
        ProductoBLL_013AL prbll = new ProductoBLL_013AL();
        SolicitudCotizacionBLL_013AL scbll = new SolicitudCotizacionBLL_013AL();
        DetalleSolicitudBLL_013AL dsbll = new DetalleSolicitudBLL_013AL();

        private int idSolicitudCotizacion;
        private List<DetalleSolicitudC_013AL> detallesCotizacion = new List<DetalleSolicitudC_013AL>();

        public void ActualizarIdioma_013AL()
        {
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }

        private void Cotizaciones_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("CodProducto", "Cod Producto");
            dataGridView1.Columns.Add("NombreProducto", "Nombre Producto");
            dataGridView1.Columns.Add("Cantidad", "Cantidad");
            CargarProveedores_013AL();
            CargarProductos_013AL();
        }
        private void CargarProveedores_013AL()
        {
            var listaProveedores = pbll.ListarProveedores_013AL();
            cboproveedor.Items.Clear();

            foreach (var proveedor in listaProveedores)
            {
                string detalleProveedor = $"{proveedor.NombreProveedor_013AL} {proveedor.ApellidoProveedor_013AL} - {proveedor.RazonSocial_013AL}";

                cboproveedor.Items.Add(new ComboBoxItem
                {
                    Text = detalleProveedor,
                    Value = proveedor
                });
            }
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

        private void CargarProductos_013AL()
        {

            var listaProductos = prbll.ListarProductosPocoStock_013AL();
            comboBox1.Items.Clear();

            foreach (var productos in listaProductos)
            {
                string detalleProducto = $"{productos.Nombre_013AL}";
                //comboBox1.Items.Add(detalleProducto); // Esta línea agrega un string al ComboBox

                comboBox1.Items.Add(new ComboBoxItem // Esta línea agrega un objeto ComboBoxItem al ComboBox
                {
                    Text = detalleProducto,
                    Value = productos
                });
            }

        }
        private void Preregistrar_Click(object sender, EventArgs e)
        {
            PreregistrarProveedor_013AL form = new PreregistrarProveedor_013AL();
            form.ShowDialog();
            CargarProveedores_013AL();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           if (cboproveedor.SelectedItem is ComboBoxItem selectedItem)
            {
                var proveedorSeleccionado = (Proveedor_013AL)selectedItem.Value;
                int cuitProveedor = proveedorSeleccionado.CUIT_013AL;

                
                idSolicitudCotizacion = scbll.AgregarSCotizacion_013AL(cuitProveedor);

                detallesCotizacion.Clear();
                ActualizarDataGridView_013AL();

                MessageBox.Show("Solicitud de cotización creada con ID: " + idSolicitudCotizacion);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor válido.");
            }
        }

        private void ActualizarDataGridView_013AL()
        {
            dataGridView1.Rows.Clear();

            // Agregar los detalles a las filas del DataGridView
            foreach (var detalle in detallesCotizacion)
            {
                var producto = prbll.ObtenerProductoPorId_013AL(detalle.CodProducto_013AL);
                string nombreProducto = producto != null ? producto.Nombre_013AL : "Desconocido";

                dataGridView1.Rows.Add(detalle.CodProducto_013AL, nombreProducto, detalle.Cantidad_013AL);
            }
        }
        EventoBLL_013AL bbll = new EventoBLL_013AL();
        Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
        private void button3_Click(object sender, EventArgs e)
        {
            if (idSolicitudCotizacion > 0 && cboproveedor.SelectedItem is ComboBoxItem selectedItem)
            {
                var proveedorSeleccionado = (Proveedor_013AL)selectedItem.Value;
                var detallesProductos = detallesCotizacion;

                if (proveedorSeleccionado != null && detallesProductos.Count > 0)
                {
                    GenerarSolicitudCotizacionPDF_013AL(proveedorSeleccionado, detallesProductos);                    
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cotizaciones", $"Solicitud {idSolicitudCotizacion} generada", 3);
                }
                else
                {
                    MessageBox.Show("No se encontró la información del proveedor o no hay productos en la solicitud.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor y asegúrate de tener una solicitud de cotización creada.");
            }

        }
        private void GenerarSolicitudCotizacionPDF_013AL(Proveedor_013AL proveedor, List<DetalleSolicitudC_013AL> detallesProductos)
        {
            // Aquí suponemos que usas una librería para crear PDF como iTextSharp.
            using (var doc = new Document())
            {
                // Obtener la ruta del escritorio del usuario y combinarla con el nombre del archivo PDF
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktopPath, $"SolicitudCotizacion_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                // Crear el archivo PDF en la ruta especificada
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();

                // Título del PDF
                doc.Add(new Paragraph("Solicitud de Cotización", FontFactory.GetFont("Arial", 16)));
                doc.Add(new Paragraph("\n"));
                doc.Add(new Paragraph($"Fecha: {DateTime.Now.ToShortDateString()}"));
                doc.Add(new Paragraph("\n"));
                // Información del proveedor
                doc.Add(new Paragraph($"Proveedor: {proveedor.NombreProveedor_013AL} {proveedor.ApellidoProveedor_013AL}"));
                doc.Add(new Paragraph($"Razón Social: {proveedor.RazonSocial_013AL}"));
                doc.Add(new Paragraph($"CUIT: {proveedor.CUIT_013AL}"));
                doc.Add(new Paragraph("\n"));

                // Crear tabla para los productos
                PdfPTable table = new PdfPTable(3); // 3 columnas: ID Producto, Nombre Producto, Cantidad
                table.AddCell("Cod Producto");
                table.AddCell("Nombre Producto");
                table.AddCell("Cantidad");

                foreach (var detalle in detallesProductos)
                {
                    var producto = prbll.ObtenerProductoPorId_013AL(detalle.CodProducto_013AL);
                    string nombreProducto = producto != null ? producto.Nombre_013AL : "Desconocido";

                    table.AddCell(detalle.CodProducto_013AL.ToString());
                    table.AddCell(nombreProducto);
                    table.AddCell(detalle.Cantidad_013AL.ToString());
                }

                doc.Add(table);
                doc.Close();
            }

            MessageBox.Show("PDF de la solicitud de cotización generado con éxito.");
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener la fila seleccionada
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];

                // Obtener el IdProducto de la fila seleccionada
                int idProducto = Convert.ToInt32(filaSeleccionada.Cells["CodProducto"].Value);

                // Buscar el CodSCotizacion en la lista de detalles usando el IdProducto
                var detalle = detallesCotizacion.FirstOrDefault(d => d.CodProducto_013AL == idProducto);

                if (detalle != null)
                {
                    int codSCotizacion = detalle.CodSCotizacion_013AL;

                    // Confirmar la eliminación con el usuario (opcional)
                    var resultado = MessageBox.Show("¿Estás seguro de que deseas eliminar este producto de la solicitud?",
                                                    "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (resultado == DialogResult.Yes)
                    {
                        // Llamar al método de la capa BLL para eliminar el detalle
                        string respuesta = dsbll.EliminarDetalleSC_013AL(idProducto, codSCotizacion);

                        // Verificar la respuesta y mostrar un mensaje al usuario
                        if (respuesta == "OK")
                        {
                            detallesCotizacion.Remove(detalle);

                            ActualizarDataGridView_013AL();

                            MessageBox.Show(
                                "El producto ha sido eliminado de la solicitud de cotización.",
                                "Éxito",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Hubo un error al intentar eliminar el producto de la solicitud. Por favor, inténtalo nuevamente.",
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró la información del detalle de la solicitud.",
                                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para eliminar.",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is ComboBoxItem selectedItem && idSolicitudCotizacion > 0)
            {
                var productoSeleccionado = (Producto_013AL)selectedItem.Value;
                int idProducto = productoSeleccionado.CodProducto_013AL;
                int cantidad;

                if (int.TryParse(textBox1.Text, out cantidad) && cantidad > 0 && cantidad <= 10000)
                {
                    var detalleExistente = detallesCotizacion.FirstOrDefault(d => d.CodProducto_013AL == idProducto);

                    if (detalleExistente != null)
                    {
                        detalleExistente.Cantidad_013AL += cantidad;

                        string result =
                            dsbll.ActualizarCantidadDetalleSC_013AL(
                                idSolicitudCotizacion,
                                idProducto,
                                detalleExistente.Cantidad_013AL);

                        MessageBox.Show("Cantidad actualizada.");
                    }
                    else
                    {
                        string result =
                            dsbll.AgregarDetalleSC_013AL(
                                idSolicitudCotizacion,
                                idProducto,
                                cantidad);

                        MessageBox.Show(result);

                        detallesCotizacion.Add(new DetalleSolicitudC_013AL
                        {
                            CodSCotizacion_013AL = idSolicitudCotizacion,
                            CodProducto_013AL = idProducto,
                            Cantidad_013AL = cantidad
                        });
                    }
                    ActualizarDataGridView_013AL();
                }
                else
                {
                    MessageBox.Show("Por favor, ingresa una cantidad válida.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto y asegúrate de haber creado una solicitud de cotización.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (idSolicitudCotizacion <= 0)
            {
                MessageBox.Show("No hay una solicitud activa.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar la solicitud completa?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta == DialogResult.Yes)
            {
                string resultado =
                    scbll.EliminarSCotizacion_013AL(idSolicitudCotizacion);

                if (resultado == "OK")
                {
                    detallesCotizacion.Clear();

                    ActualizarDataGridView_013AL();

                    cboproveedor.SelectedIndex = -1;
                    comboBox1.SelectedIndex = -1;
                    textBox1.Clear();

                    idSolicitudCotizacion = 0;

                    MessageBox.Show(
                        "Solicitud eliminada correctamente.");
                }
            }
        }
    }
}
