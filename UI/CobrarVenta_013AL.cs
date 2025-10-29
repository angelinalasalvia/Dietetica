using BE_013AL;
using BE_013AL.Composite;
using BLL;
using BLL_013AL;
using Servicios;
using Servicios_013AL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UI
{
    public partial class CobrarVenta_013AL : Form, IObserver_013AL
    {
        FacturaBLL_013AL blln = new FacturaBLL_013AL();
        /*public int IdCompra;
        public SeleccionarProducto(int id)
        {
            InitializeComponent();
            IdCompra = id;
        }*/

        public int total_013AL;
        public int CUIL_013AL;
        public int IdC_013AL;
        
        public CobrarVenta_013AL(int tot, int cuil, int idc, List<Detalle_013AL> detalles)
        {
            InitializeComponent();
            total_013AL = tot;
            CUIL_013AL = cuil;
            IdC_013AL = idc;
            detallesVenta = detalles;
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
        private void CobrarVenta_Load(object sender, EventArgs e)
        {
            txtcuil.Text = CUIL_013AL.ToString();
            txttotal.Text = total_013AL.ToString();
            //txtTarjeta.ReadOnly = true;
            ActualizarIdioma_013AL();
        }
        private List<Detalle_013AL> detallesVenta;
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            //string respuesta = "";

            if (comboBox1.Text == "Tarjeta Débito")
            {
                
                // Validar que el campo de número de tarjeta tenga exactamente 16 dígitos
                if (txtTarjeta.Text.Length != 16 || !txtTarjeta.Text.All(char.IsDigit))
                {
                    MessageBox.Show("El número de la tarjeta debe tener exactamente 16 dígitos numéricos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            /*CAMBIO TRANSACCION respuesta = blln.AgregarCompraCliente_013AL(IdC_013AL, CUIL_013AL, comboBox1.Text);
            MessageBox.Show("Se realizó el cobro de la venta con éxito.");*/
            
            //NUEVO
            //List<Detalle_013AL> detalles = blln.ListarCompraProducto_013AL()
            //.Where(x => x.CodCompra_013AL == IdC_013AL).ToList();

            Factura_013AL factura = new Factura_013AL
            {
                CodCompra_013AL = IdC_013AL,
                CUIL_013AL = CUIL_013AL,
                MetPago_013AL = comboBox1.Text
            };

            string resultado = blln.RegistrarVentaCompleta_013AL(factura, detallesVenta);

            if (resultado == "OK")
            {
                MessageBox.Show("Venta registrada correctamente");
            }
            //NUEVO

            try
            {
                string resultadoevento;
                EventoBLL_013AL bbll = new EventoBLL_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                resultadoevento = bbll.AgregarEvento_013AL(user.Login_013AL, "Ventas", "Cobrar Venta", 3);
            }
            catch (Exception ex) { }

            string numeroTarjeta;

            // Si el método de pago es "Efectivo", siempre se asigna "0"
            if (comboBox1.Text == "Efectivo")
            {
                numeroTarjeta = "0";
            }
            // Si el método de pago es "Tarjeta Débito", se toma el número ingresado en el textbox
            else if (comboBox1.Text == "Tarjeta Débito")
            {
                numeroTarjeta = txtTarjeta.Text;
            }
            else
            {
                // En caso de otros métodos de pago, puedes decidir cómo manejarlo (por ejemplo, dejarlo vacío o asignar "0")
                numeroTarjeta = "0";
            }

            this.Tag = numeroTarjeta;


        }
    }
}
