using BE_013AL;
using BLL;
using BLL_013AL;
using Servicios_013AL;
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
    public partial class Productos_013AL : Form, IObserver_013AL
    {
        public Productos_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }
        ProductoBLL_013AL bll = new ProductoBLL_013AL();
        private byte[] imagenBytes;
        public void ActualizarIdioma_013AL()
        {
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /*string respuesta = "";
            respuesta = bll.AgregarProducto(txtnombre.Text, Convert.ToInt32(txtstock.Text), Convert.ToInt32(txtprecio.Text), Convert.ToByte(txtimagen.Text), txtdesc.Text);
            */
            if (imagenBytes != null)
            {
                // Llamar al método para agregar el producto
                string respuesta = bll.AgregarProducto_013AL(
                    txtnombre.Text,
                    Convert.ToInt32(txtstock.Text),
                    Convert.ToInt32(txtprecio.Text),
                    imagenBytes, 
                    txtdesc.Text
                );

                MessageBox.Show(respuesta, "Resultado de la operación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarProductos_013AL();


                EventoBLL_013AL bbll = new EventoBLL_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bbll.AgregarEvento_013AL(user.Login_013AL, "Productos", "Registrar Producto", 2);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una imagen antes de registrar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Seleccionar una imagen";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Muestra la ruta de la imagen en el TextBox (txtimagen)
                    txtimagen.Text = openFileDialog.FileName;

                    // Muestra la imagen seleccionada en el PictureBox
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);

                    // Guarda la imagen en bytes para su posterior uso
                    imagenBytes = File.ReadAllBytes(openFileDialog.FileName);
                }
            }
        }

        private void CargarProductos_013AL()
        {
            DataTable dtProductos = bll.ListarProductosConImagen_013AL();
            dataGridView1.AllowUserToAddRows = false;
            // Verifica si hay productos para mostrar
            if (dtProductos.Rows.Count > 0)
            {
                // Asigna el DataTable al DataGridView
                dataGridView1.DataSource = dtProductos;

                // Recorre cada fila para asignar la imagen si es que existe
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Verifica que el valor de la celda no sea null o DBNull antes de convertirlo
                    if (row.Cells["Imagen-013AL"].Value != DBNull.Value && row.Cells["Imagen-013AL"].Value != null)
                    {
                        // Verifica que la celda contenga un arreglo de bytes y tenga contenido
                        if (row.Cells["Imagen-013AL"].Value is byte[] imagenBytes && imagenBytes.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imagenBytes))
                            {
                                // Asigna la imagen convertida al valor de la celda
                                row.Cells["Imagen-013AL"].Value = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            // Si el arreglo de bytes está vacío, deja la celda en blanco o asigna una imagen por defecto
                            row.Cells["Imagen-013AL"].Value = null; // O asigna una imagen predeterminada.
                        }
                    }
                    else
                    {
                        // Si no hay imagen, deja la celda en blanco o asigna una imagen por defecto
                        row.Cells["Imagen-013AL"].Value = null; // O asigna una imagen predeterminada.
                    }
                }

                dataGridView1.RowTemplate.Height = 150; // Incrementa este valor para ver la imagen más grande

                // Ajusta el ancho de la columna de la imagen para que se vea correctamente
                dataGridView1.Columns["Imagen-013AL"].Width = 150; // Ajusta el ancho según necesites

                // Ajusta el modo de presentación de las imágenes para que se redimensionen proporcionalmente
                DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dataGridView1.Columns["Imagen-013AL"];
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                // Ajustar el modo de tamaño de columnas para que se expandan de manera adecuada
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            }
            else
            {
                MessageBox.Show("No hay productos con Bit_Lo_Bo = 1 para mostrar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Productos_Load(object sender, EventArgs e)
        {
            CargarProductos_013AL();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count > 0)
            {
                
                
                // Obtener la fila seleccionada
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];

                // Suponiendo que la columna que almacena el ID del producto se llama "ID" (ajusta si tiene otro nombre)
                int idProducto = Convert.ToInt32(filaSeleccionada.Cells["CodProducto-013AL"].Value);

                // Verificar que la imagen no sea nula
                if (imagenBytes != null)
                {
                    // Llamar al método para modificar el producto
                    string respuesta = bll.ModificarProducto_013AL(
                        idProducto, // Pasar el ID del producto
                        txtnombre.Text,
                        Convert.ToInt32(txtstock.Text),
                        Convert.ToInt32(txtprecio.Text),
                        imagenBytes,
                        txtdesc.Text
                    );

                    MessageBox.Show(respuesta, "Resultado de la operación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar la lista de productos para reflejar los cambios
                    CargarProductos_013AL();

                    EventoBLL_013AL bbll = new EventoBLL_013AL();
                    Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Productos", "Modificar Producto", 2);
                }
                else
                {
                    MessageBox.Show("Por favor, selecciona una imagen antes de modificar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto de la lista para modificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            /*string resultado;
            resultado = bll.ModificarProducto();
            CargarProductos();*/
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener la fila seleccionada
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];

                // Suponiendo que la columna que almacena el ID del producto se llama "ID" (ajusta si tiene otro nombre)
                int idProducto = Convert.ToInt32(filaSeleccionada.Cells["ID"].Value);

                // Verificar que la imagen no sea nula
                
                    // Llamar al método para modificar el producto
                    string respuesta = bll.EliminarProducto_013AL(
                        idProducto // Pasar el ID del producto   
                    );

                    MessageBox.Show(respuesta, "Resultado de la operación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recargar la lista de productos para reflejar los cambios
                CargarProductos_013AL();

                EventoBLL_013AL bbll = new EventoBLL_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bbll.AgregarEvento_013AL(user.Login_013AL, "Productos", "Eliminar Producto", 2);

            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto de la lista para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CodProducto-013AL"].Value);
                Productos_C_013AL form = new Productos_C_013AL(idProducto);
                form.ShowDialog();
                CargarProductos_013AL();
            }
            else
            {
                MessageBox.Show("Selecciona un producto para ver su historial.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
