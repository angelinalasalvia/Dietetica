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
        PedidoBLL_013AL pbll = new PedidoBLL_013AL();
        DetalleBLL_013AL dbll = new DetalleBLL_013AL();
        public int IdCompraActual;
        //private List<Detalle_013AL> carritoTemporal;
        public SeleccionarProducto_013AL(int id)
        {
            InitializeComponent();
            IdCompraActual = id;
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
                int cantidad = Convert.ToInt32(Interaction.InputBox("Ingrese cantidad:")); 
                string str = ((PictureBox)sender).Tag.ToString(); 
                Producto_013AL producto = prbll.Devolver_Producto_Buscado_x_Id_013AL(Convert.ToInt32(str)); 
                if (cantidad <= 0) 
                { 
                    MessageBox.Show("Cantidad inválida."); 
                    return; 
                } 
                if (cantidad > producto.Stock_013AL) 
                { 
                    MessageBox.Show("Stock insuficiente."); 
                    return; 
                }
                if (cantidad > 10000)
                {
                    MessageBox.Show("Cantidad máxima permitida es 10.000.");
                    return;
                }
                Detalle_013AL detalleExistente = dbll.ObtenerDetalle_013AL(IdCompraActual, producto.CodProducto_013AL); 
                if (detalleExistente != null) 
                { 
                    int nuevaCantidad = detalleExistente.Cantidad_013AL + cantidad; 
                    dbll.ActualizarCantidadDetalle_013AL(IdCompraActual, producto.CodProducto_013AL, nuevaCantidad); 
                } 
                else 
                { 
                    Detalle_013AL detalle = new Detalle_013AL() 
                    { 
                        CodCompra_013AL = IdCompraActual, 
                        CodProducto_013AL = producto.CodProducto_013AL, 
                        Cantidad_013AL = cantidad, 
                        PrecioUnitario_013AL = producto.Precio_013AL 
                    }; 
                    dbll.AgregarDetalle_013AL(detalle); 
                } 
                prbll.DescontarStock_013AL(producto.CodProducto_013AL, cantidad); 
                decimal total = dbll.CalcularTotalPedido_013AL(IdCompraActual); 
                pbll.ActualizarTotalPedido_013AL(IdCompraActual, total); 
                //MessageBox.Show("Producto agregado.");
                MessageBox.Show($"Producto '{producto.Nombre_013AL}' agregado.\nCantidad: {cantidad}\nSubtotal: {(producto.Precio_013AL * cantidad)}",
                    "Producto Agregado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadItems_013AL(); 
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
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
