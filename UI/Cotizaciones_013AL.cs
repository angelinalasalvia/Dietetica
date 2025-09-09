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

        NegocioBLL_013AL bll = new NegocioBLL_013AL();
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
            /*var productosPocoStock = bll.ListarProductosPocoStock();

            if (productosPocoStock.Any())
            {
                Console.WriteLine("Atención! Productos con poco stock:");
                foreach (var producto in productosPocoStock)
                {
                    Console.WriteLine($"- {producto.Nombre}: {producto.Stock} unidades");
                }
            }
            else
            {
                Console.WriteLine("No hay productos con poco stock.");
            }*/
            
            dataGridView1.Columns.Add("IdProducto", "ID Producto");
            dataGridView1.Columns.Add("NombreProducto", "Nombre Producto");
            dataGridView1.Columns.Add("Cantidad", "Cantidad");
            CargarProveedores_013AL();
            CargarProductos_013AL();

        }
        private void CargarProveedores_013AL()
        {
            var listaProveedores = bll.ListarProveedores_013AL();
            cboproveedor.Items.Clear();

            foreach (var proveedor in listaProveedores)
            {
                string detalleProveedor = $"{proveedor.NombreProveedor_013AL} {proveedor.ApellidoProveedor_013AL} - {proveedor.RazonSocial_013AL}";

                // Agrega el objeto proveedor como elemento del ComboBox
                cboproveedor.Items.Add(new ComboBoxItem
                {
                    Text = detalleProveedor,
                    Value = proveedor
                });
            }

            /*var listaProveedores = bll.ListarProveedores();

            cboproveedor.Items.Clear();

            foreach (var proveedor in listaProveedores)
            {
                string detalleProveedor = $"{proveedor.NombreProveedor} {proveedor.ApellidoProveedor} - {proveedor.RazonSocial}";
                cboproveedor.Items.Add(detalleProveedor);
            }*/
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

            var listaProductos = bll.ListarProductosPocoStock_013AL();
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
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
           if (cboproveedor.SelectedItem is ComboBoxItem selectedItem)
            {
                var proveedorSeleccionado = (Proveedor_013AL)selectedItem.Value;
                int cuitProveedor = proveedorSeleccionado.CUIT_013AL;

                
                idSolicitudCotizacion = bll.AgregarSCotizacion_013AL(cuitProveedor);
                MessageBox.Show("Solicitud de cotización creada con ID: " + idSolicitudCotizacion);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor válido.");
            }
            /*if (cboproveedor.SelectedItem is ComboBoxItem selectedItem)
            {
                var proveedorSeleccionado = (Proveedor)selectedItem.Value;
                int cuitProveedor = proveedorSeleccionado.CUIT;

                string resultado = bll.AgregarSCotizacion(cuitProveedor);
                MessageBox.Show(resultado);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor válido.");
            }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            

            /*if (comboBox1.SelectedItem is ComboBoxItem selectedItem && idSolicitudCotizacion > 0)
            {
                var productoSeleccionado = (Producto)selectedItem.Value;
                int idprod = productoSeleccionado.IdProducto;

                // Llama a la BLL para agregar el detalle de la solicitud de cotización
                string resultado = bll.AgregarDetalleSC(idSolicitudCotizacion, idprod, Convert.ToInt32(textBox1.Text));
                MessageBox.Show(resultado);
            }
            else
            {
               MessageBox.Show("Por favor, selecciona un producto y asegúrate de haber creado una solicitud de cotización.");
            }*/

            /*if (comboBox1.SelectedItem is ComboBoxItem selectedItem)
            {
                var productoSeleccionado = (Producto)selectedItem.Value;
                int idprod = productoSeleccionado.IdProducto;

                string resultado = bll.AgregarDetalleSC( ,idprod, Convert.ToInt32(textBox1.Text));
                MessageBox.Show(resultado);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor válido.");
            }*/
        }

        private void ActualizarDataGridView_013AL()
        {
            dataGridView1.Rows.Clear();

            // Agregar los detalles a las filas del DataGridView
            foreach (var detalle in detallesCotizacion)
            {
                var producto = bll.ObtenerProductoPorId_013AL(detalle.CodProducto_013AL);
                string nombreProducto = producto != null ? producto.Nombre_013AL : "Desconocido";

                dataGridView1.Rows.Add(detalle.CodProducto_013AL, nombreProducto, detalle.Cantidad_013AL);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (idSolicitudCotizacion > 0 && cboproveedor.SelectedItem is ComboBoxItem selectedItem)
            {
                var proveedorSeleccionado = (Proveedor_013AL)selectedItem.Value;
                var detallesProductos = detallesCotizacion;

                if (proveedorSeleccionado != null && detallesProductos.Count > 0)
                {
                    // Generar PDF con los datos del proveedor y productos
                    GenerarSolicitudCotizacionPDF_013AL(proveedorSeleccionado, detallesProductos);


                    BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                    Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Cotizaciones", "Generar Solicitud Cotizacion", 2);
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
                string filePath = Path.Combine(desktopPath, "SolicitudCotizacion.pdf");

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
                table.AddCell("ID Producto");
                table.AddCell("Nombre Producto");
                table.AddCell("Cantidad");

                foreach (var detalle in detallesProductos)
                {
                    var producto = bll.ObtenerProductoPorId_013AL(detalle.CodProducto_013AL);
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
                int idProducto = Convert.ToInt32(filaSeleccionada.Cells["IdProducto"].Value);

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
                        string respuesta = bll.EliminarDetalleSC_013AL(idProducto, codSCotizacion);

                        // Verificar la respuesta y mostrar un mensaje al usuario
                        if (respuesta == "OK")
                        {
                            MessageBox.Show("El producto ha sido eliminado de la solicitud de cotización.",
                                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            
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

                // Verificar que la cantidad sea un número válido
                if (int.TryParse(textBox1.Text, out cantidad) && cantidad > 0)
                {
                    // Agregar el detalle a la solicitud de cotización
                    string resultado = bll.AgregarDetalleSC_013AL(idSolicitudCotizacion, idProducto, cantidad);
                    MessageBox.Show(resultado);

                    // Crear un nuevo detalle y agregarlo a la lista de detalles
                    var detalle = new DetalleSolicitudC_013AL
                    {
                        CodSCotizacion_013AL = idSolicitudCotizacion,
                        CodProducto_013AL = idProducto,
                        Cantidad_013AL = cantidad
                    };
                    detallesCotizacion.Add(detalle);

                    // Actualizar el DataGridView
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
    }
}
