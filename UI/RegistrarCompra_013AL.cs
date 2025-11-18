using BLL;
using BLL_013AL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class RegistrarCompra_013AL : Form, IObserver_013AL
    {
        public RegistrarCompra_013AL()
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
        ProductoBLL_013AL prbll = new ProductoBLL_013AL();
        OrdenCompraBLL_013AL ocbll = new OrdenCompraBLL_013AL();
        private void CargarProductos()
        {
            DataTable dtProductos = prbll.ListarProductosConImagen_013AL();
            dataGridView1.AllowUserToAddRows = false;

            if (dtProductos.Rows.Count > 0)
            {
                dataGridView1.DataSource = dtProductos;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Imagen-013AL"].Value != DBNull.Value && row.Cells["Imagen-013AL"].Value != null)
                    {
                        if (row.Cells["Imagen-013AL"].Value is byte[] imagenBytes && imagenBytes.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imagenBytes))
                            {
                                row.Cells["Imagen-013AL"].Value = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            row.Cells["Imagen-013AL"].Value = null;
                        }
                    }
                    else
                    {
                        row.Cells["Imagen-013AL"].Value = null;
                    }
                }

                dataGridView1.RowTemplate.Height = 150;
                dataGridView1.Columns["Imagen-013AL"].Width = 150;

                DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["Imagen-013AL"];
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                MessageBox.Show("No hay productos con Bit_Lo_Bo = 1 para mostrar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RegistrarCompra_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener el ID del producto seleccionado y el stock actual
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CodProducto-013AL"].Value);
                int stockActual = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Stock-013AL"].Value);

                // Obtener la cantidad ingresada en textBox1
                if (int.TryParse(textBox1.Text, out int cantidadIngresada))
                {
                    // Sumar el stock actual con la cantidad ingresada
                    int nuevoStock = stockActual + cantidadIngresada;

                    // Llamar al método RegistrarCompra para actualizar el stock en la base de datos
                    string resultado = ocbll.RegistrarCompra_013AL(idProducto, nuevoStock);

                    if (resultado == "OK")
                    {
                        MessageBox.Show("Compra registrada correctamente.");
                        // Refrescar la lista de productos
                        CargarProductos();
                    }
                    else
                    {
                        MessageBox.Show("Error al registrar la compra.");
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, ingresa una cantidad válida.");
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto.");
            }
        }
    }
}
