using BE_013AL;
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
using Microsoft.VisualBasic;
using BLL;

namespace UI
{
    public partial class SeleccionarProducto_013AL : Form, IObserver_013AL
    {
        ProductoBLL_013AL prbll = new ProductoBLL_013AL();
        public int IdCompra;
        private List<Detalle_013AL> carritoTemporal;
        public SeleccionarProducto_013AL(int id, List<Detalle_013AL> carrito)
        {
            InitializeComponent();
            IdCompra = id;
            carritoTemporal = carrito;  
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);

            txtBuscar = new TextBox(); 
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Width = 200;

            Button btnBuscar = new Button();
            btnBuscar.Text = "Buscar";
            btnBuscar.Click += BtnBuscar_Click;

            this.Controls.Add(txtBuscar);
            this.Controls.Add(btnBuscar);

            txtBuscar.Location = new Point(10, 10);
            btnBuscar.Location = new Point(220, 10);
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
        public void LoadItems_013AL(string filtro = null)
        {
            panelitem.Controls.Clear();

            var productos = string.IsNullOrEmpty(filtro)
                ? prbll.Devolver_Lista_Productos_013AL()
                : prbll.BuscarProductoxNombre_013AL(filtro);

            foreach (Producto_013AL producto in productos)
            {
                if (producto.Stock_013AL <= 0) continue; 

                PictureBox pic = new PictureBox
                {
                    BackgroundImageLayout = ImageLayout.Stretch,
                    Width = 150,
                    Height = 150,
                    Cursor = Cursors.Hand,
                    Tag = producto.CodProducto_013AL
                };

                MemoryStream ms = new MemoryStream(producto.Imagen_013AL);
                Bitmap btm = new Bitmap(ms);

                pic.BackgroundImage = btm;

                Label lblPrice = new Label
                {
                    Text = "Precio: " + producto.Precio_013AL,
                    BackColor = Color.FromArgb(202, 81, 0),
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    Tag = producto.CodProducto_013AL
                };

                Label lblstock = new Label
                {
                    Text = "Stock: " + producto.Stock_013AL,
                    BackColor = Color.FromArgb(202, 81, 0),
                    AutoSize = true,
                    Dock = DockStyle.Left,
                    Tag = producto.CodProducto_013AL
                };

                Label lblname = new Label
                {
                    Text = producto.Nombre_013AL,
                    Dock = DockStyle.Bottom,
                    ForeColor = Color.White,
                    Tag = producto.CodProducto_013AL,
                    BackColor = Color.FromArgb(36, 36, 36),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                pic.Controls.Add(lblstock);
                pic.Controls.Add(lblPrice);
                pic.Controls.Add(lblname);

                panelitem.Controls.Add(pic);

                pic.Click += SalesItem;
            }
        }

        private void SalesItem(object sender, EventArgs e)
        {
            try
            {
                int Cantidad = Convert.ToInt32(Interaction.InputBox("Ingrese la cantidad que desea comprar: "));
                string str = ((PictureBox)sender).Tag.ToString();
                Producto_013AL producto = prbll.Devolver_Producto_Buscado_x_Id_013AL(Convert.ToInt32(str));

                if (Cantidad <= 0)
                {
                    MessageBox.Show("La cantidad ingresada debe ser mayor a cero.");
                    return;
                }

                if (Cantidad > producto.Stock_013AL)
                {
                    MessageBox.Show("La cantidad solicitada supera el stock disponible.");
                    return;
                }

                // Ver si ya existe el producto en el carrito
                var detalleExistente = carritoTemporal
                    .FirstOrDefault(p => p.CodProducto_013AL == producto.CodProducto_013AL);

                if (detalleExistente != null)
                {
                    detalleExistente.Cantidad_013AL += Cantidad;
                    
                }
                else
                {
                    carritoTemporal.Add(new Detalle_013AL
                    {
                        CodCompra_013AL = IdCompra,
                        CodProducto_013AL = producto.CodProducto_013AL,
                        Cantidad_013AL = Cantidad,
                        PrecioUnitario_013AL = producto.Precio_013AL,
                        
                    });
                }

                // Descontar stock real
                prbll.DescontarStock_013AL(producto.CodProducto_013AL, Cantidad);

                MessageBox.Show($"Producto '{producto.Nombre_013AL}' agregado.\nCantidad: {Cantidad}\nSubtotal: {(producto.Precio_013AL * Cantidad)}",
                    "Producto Agregado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadItems_013AL(); // Refrescar vista
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al agregar el producto: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            /*try
            {
                int Cantidad = Convert.ToInt32(Interaction.InputBox("Ingrese la cantidad que desea comprar: "));
                string str = ((PictureBox)sender).Tag.ToString();
                Producto_013AL producto = bll.Devolver_Producto_Buscado_x_Id_013AL(Convert.ToInt32(str));

                if (Cantidad <= 0)
                {
                    MessageBox.Show("La cantidad ingresada debe ser mayor a cero.");
                    return;
                }

                if (Cantidad > producto.Stock_013AL)
                {
                    MessageBox.Show("La cantidad solicitada supera el stock disponible.");
                    return;
                }


                var listaProductosCompra = bll.ListarCompraProducto_013AL();
                var detalleExistente = listaProductosCompra
                    .FirstOrDefault(p => p.CodCompra_013AL == IdCompra && p.CodProducto_013AL == producto.CodProducto_013AL);

                string respuesta;
                
                bll.DescontarStock_013AL(producto.CodProducto_013AL, Cantidad);

                if (detalleExistente != null)
                {
                    
                    int nuevaCantidad = detalleExistente.Cantidad_013AL + Cantidad;
                    respuesta = bll.ActualizarCantidadCompraProducto_013AL(IdCompra, producto.CodProducto_013AL, nuevaCantidad);
                }
                else
                {
                    
                    respuesta = bll.AgregarCompraProducto_013AL(IdCompra, producto.CodProducto_013AL, Cantidad, producto.Precio_013AL);
                }
              

                // Mostrar feedback al usuario
                MessageBox.Show($"Producto '{producto.Nombre_013AL}' agregado.\nCantidad: {Cantidad}\nSubtotal: {(producto.Precio_013AL * Cantidad)}",
                    "Producto Agregado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Opcional: Actualizar los productos en pantalla para reflejar el stock
                LoadItems_013AL();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al agregar el producto: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void FinalizarCompra_Click(object sender, EventArgs e)
        {
            try
            {
                // Aquí podrías realizar validaciones finales, como calcular totales o generar una factura
                MessageBox.Show("Compra finalizada con éxito.", "Finalizar Compra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Cerrar el formulario de selección
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al finalizar la compra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        } 
        
        private void SeleccionarProducto_Load(object sender, EventArgs e)
        {
            panelitem.AutoScroll = true;
            LoadItems_013AL();
        }
        

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            var txtBuscar = Controls.OfType<TextBox>().FirstOrDefault(t => t.Name == "txtBuscar");
            if (txtBuscar != null)
            {
                string filtro = txtBuscar.Text.Trim();
                LoadItems_013AL(filtro);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            LoadItems_013AL();
        }
    }
}
