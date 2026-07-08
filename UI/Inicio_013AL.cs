using BE;
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace UI
{
    public partial class Inicio_013AL : Form, IObserver_013AL
    {
        private UsuarioBLL_013AL bllUsuarios = new UsuarioBLL_013AL();
        private LoteBLL_013AL loteBLL = new LoteBLL_013AL();
        private Usuarios_013AL _usuario;

        public Inicio_013AL()
        {
            InitializeComponent();
            
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            SingletonSession_013AL.Instance.IdiomaActual_013AL = "es";

            var lm = LanguageManager_013AL.ObtenerInstancia_013AL();
            var bllTraduccion = new TraduccionBLL_013AL();
            lm.ObtenerTraduccionesPorIdioma = (id) => bllTraduccion.ObtenerPorIdioma(id);
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
        private void MostrarAvisoLotesProximosAVencer()
        {
            Usuarios_013AL usuario = SingletonSession_013AL.Instance.GetUsuario_013AL();

            // Almacenista
            if (usuario.CodRol_013AL != 14)
                return;

            loteBLL.ActualizarEstadosLotes_013AL();
            loteBLL.ActualizarStockPorVencimientos_013AL();

            int cantidad = loteBLL.ContarLotesProximosAVencer_013AL();

            if (cantidad > 0)
            {
                DialogResult r = MessageBox.Show(
                    $"Existen {cantidad} lotes próximos a vencer.\n\n¿Desea revisarlos ahora?",
                    "Aviso",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (r == DialogResult.Yes)
                {
                    Lotes_013AL frm = new Lotes_013AL();
                    frm.ShowDialog();
                }
            }
        }
        SolicitudPromocionBLL_013AL solicitudbll = new SolicitudPromocionBLL_013AL();
        private void MostrarAvisoSolicitudesPendientes()
        {
            Usuarios_013AL usuario = SingletonSession_013AL.Instance.GetUsuario_013AL();

            if (usuario.CodRol_013AL != 15) // Supervisor
                return;

            int cantidad = solicitudbll.ContarSolicitudesPendientes_013AL();

            if (cantidad > 0)
            {
                DialogResult r = MessageBox.Show(

                    $"Existen {cantidad} solicitudes de promoción pendientes.\n\n¿Desea revisarlas ahora?",

                    "Solicitudes pendientes",

                    MessageBoxButtons.YesNo,

                    MessageBoxIcon.Information);

                if (r == DialogResult.Yes)
                {
                    SolicitudPromocionForm_013AL frm = new SolicitudPromocionForm_013AL();

                    frm.ShowDialog();
                }
            }
        }
        private List<Rol_013AL> ObtenerTodosLosPermisos(List<Rol_013AL> permisosCompuestos)
        {
            List<Rol_013AL> lista = new List<Rol_013AL>();

            foreach (var permiso in permisosCompuestos)
            {
                lista.Add(permiso); 

                var hijos = permiso.ObtenerHijos_013AL();
                if (hijos != null && hijos.Count > 0)
                {
                    lista.AddRange(ObtenerTodosLosPermisos(hijos));
                }
            }

            return lista;
        }

        private void ShowLoginForm()
        {
            using (Login_013AL loginForm = new Login_013AL())
            {
                loginForm.ShowDialog();
                if (SingletonSession_013AL.Instance.IsLoggedIn_013AL())
                {
                    _usuario = SingletonSession_013AL.Instance.LoggedInUser_013AL;
                    UpdateSessionStatus();

                    MostrarAvisoLotesProximosAVencer();
                    MostrarAvisoSolicitudesPendientes();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private PermisoBLL_013AL permisoBLL = new PermisoBLL_013AL();

        
        private void UpdateSessionStatus()
        {
            if (SingletonSession_013AL.Instance.IsLoggedIn_013AL())
            {
                Usuarios_013AL usuarios = SingletonSession_013AL.Instance.GetUsuario_013AL();
                toolStripStatusLabel.Text = $"Usuario: {usuarios.Login_013AL} (Conectado)";

                cambiarContraseñaToolStripMenuItem.Enabled = true;

                var permisos = ObtenerTodosLosPermisos(SingletonSession_013AL.Instance.Permisos_013AL);

                menuStrip1.Items["mnuGestores"].Enabled = false;
                menuStrip1.Items["menuventa"].Enabled = false;
                menuStrip1.Items["menucliente"].Enabled = false;
                
                menuStrip1.Items["menureportes"].Enabled = false;
                menuStrip1.Items["menuayuda"].Enabled = false;
                menuStrip1.Items["comprasToolStripMenuItem"].Enabled = false;
                
                

                foreach (var permiso in permisos)
                {
                    if (permiso.Nombre_013AL == "mnuGestores")
                    {
                        menuStrip1.Items["mnuGestores"].Enabled = true;
                    }
                    if (permiso.Nombre_013AL == "menuventa")
                    {
                        menuStrip1.Items["menuventa"].Enabled = true;
                    }
                    
                    if (permiso.Nombre_013AL == "menureportes")
                    {
                        menuStrip1.Items["menureportes"].Enabled = true;
                    }
                    if (permiso.Nombre_013AL == "menuayuda")
                    {
                        menuStrip1.Items["menuayuda"].Enabled = true;
                    }
                   
                    if (permisos.Any((P)=> P.Nombre_013AL == "comprasToolStripMenuItem"))
                    {
                        var comprasMenuItem = menuStrip1.Items["comprasToolStripMenuItem"] as ToolStripMenuItem;

                        if (comprasMenuItem != null)
                        {
                            comprasMenuItem.Enabled = true;

                            
                            bool habilitarCotizacion = permisos.Any((P) => P.Nombre_013AL == "cotizacionesToolStripMenuItem");
                            bool habilitarOrdenCompra = permisos.Any((P) => P.Nombre_013AL == "ordenDeCompraToolStripMenuItem");
                            bool habilitarregiscom = permisos.Any((P) => P.Nombre_013AL == "pagarProductosToolStripMenuItem");
                            bool habilitarrecepprod = permisos.Any((P) => P.Nombre_013AL == "recepciónDeProductosToolStripMenuItem");


                            foreach (ToolStripItem item in comprasMenuItem.DropDownItems)
                            {
                                if (item is ToolStripMenuItem subItem)
                                {
                                    subItem.Enabled = false; 
                                    if (habilitarCotizacion && subItem.Name == "cotizacionesToolStripMenuItem")
                                    {
                                        subItem.Enabled = true; 
                                    }
                                    else if (habilitarOrdenCompra && subItem.Name == "ordenDeCompraToolStripMenuItem")
                                    {
                                        subItem.Enabled = true; 
                                    }
                                    else if (habilitarregiscom && subItem.Name == "pagarProductosToolStripMenuItem")
                                    {
                                        subItem.Enabled = true;
                                    }
                                    else if (habilitarrecepprod && subItem.Name == "recepciónDeProductosToolStripMenuItem")
                                    {
                                        subItem.Enabled = true;
                                    }
                                }
                            }
                        }
                    }
                    if (permisos.Any((P) => P.Nombre_013AL == "menucliente"))
                    {
                        var comprasMenuItem = menuStrip1.Items["menucliente"] as ToolStripMenuItem;

                        if (comprasMenuItem != null)
                        {
                            comprasMenuItem.Enabled = true;

                            
                            bool habilitarClientes = permisos.Any((P) => P.Nombre_013AL == "clientesToolStripMenuItem");
                            bool habilitarProductos = permisos.Any((P) => P.Nombre_013AL == "productosToolStripMenuItem");
                            bool habilitarProductosC = permisos.Any((P) => P.Nombre_013AL == "productosCToolStripMenuItem");
                            bool habilitarProveedor = permisos.Any((P) => P.Nombre_013AL == "proveedoresToolStripMenuItem");

                            foreach (ToolStripItem item in comprasMenuItem.DropDownItems)
                            {
                                if (item is ToolStripMenuItem subItem)
                                {
                                    subItem.Enabled = false; 
                                    if (habilitarClientes && subItem.Name == "clientesToolStripMenuItem")
                                    {
                                        subItem.Enabled = true; 
                                    }
                                    else if (habilitarProductos && subItem.Name == "productosToolStripMenuItem")
                                    {
                                        subItem.Enabled = true; 
                                    }
                                    else if (habilitarProductosC && subItem.Name == "productosCToolStripMenuItem")
                                    {
                                        subItem.Enabled = true; 
                                    }
                                    else if (habilitarProveedor && subItem.Name == "proveedoresToolStripMenuItem")
                                    {
                                        subItem.Enabled = true; 
                                    }
                                    
                                }
                            }
                        }
                    }
                    if (permisos.Any((P) => P.Nombre_013AL == "menuventa"))
                    {
                        var ventasMenuItem = menuStrip1.Items["menuventa"] as ToolStripMenuItem;

                        if (ventasMenuItem != null)
                        {
                            ventasMenuItem.Enabled = true;


                            bool habilitarPedido = permisos.Any((P) => P.Nombre_013AL == "menuGenerarPedido");
                            bool habilitarControl = permisos.Any((P) => P.Nombre_013AL == "menuControlFormal");
                            bool habilitarCobrar = permisos.Any((P) => P.Nombre_013AL == "menuCobrarVenta");

                            foreach (ToolStripItem item in ventasMenuItem.DropDownItems)
                            {
                                if (item is ToolStripMenuItem subItem)
                                {
                                    subItem.Enabled = false;
                                    if (habilitarPedido && subItem.Name == "menuGenerarPedido")
                                    {
                                        subItem.Enabled = true;
                                    }
                                    else if (habilitarControl && subItem.Name == "menuControlFormal")
                                    {
                                        subItem.Enabled = true;
                                    }
                                    else if (habilitarCobrar && subItem.Name == "menuCobrarVenta")
                                    {
                                        subItem.Enabled = true;
                                    }
                                }
                            }
                        }
                    }

                }
                
            }
            if (SingletonSession_013AL.Instance.IsLoggedIn_013AL() == false)
            {
                toolStripStatusLabel.Text = "No hay ningún usuario conectado.";

                DesactivarTodosLosMenus();

                
            }
        }
        private void DesactivarTodosLosMenus()
        {
            menuStrip1.Items["mnuGestores"].Enabled = false;
            menuStrip1.Items["menuventa"].Enabled = false;
            menuStrip1.Items["menucliente"].Enabled = false;
            
            menuStrip1.Items["menureportes"].Enabled = false;
            menuStrip1.Items["menuayuda"].Enabled = false;
            menuStrip1.Items["comprasToolStripMenuItem"].Enabled = false;
        }
        private void iniciarSesionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ShowLoginForm();
        }

        private void cerrarSesionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (SingletonSession_013AL.Instance.IsLoggedIn_013AL() == false)
            {
                MessageBox.Show("No hay ningún usuario conectado.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                SingletonSession_013AL.Instance.Logout_013AL();
                try
                {
                    string resultado;
                    EventoBLL_013AL bbll = new EventoBLL_013AL();
                    Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    resultado = bbll.AgregarEvento_013AL(user.Login_013AL, "Inicio de Sesión", "Logout", 1);
                }
                catch (Exception ex) { }
                UpdateSessionStatus();
                toolStripStatusLabel.Text = "No hay ningun usuario conectado.";
                cambiarContraseñaToolStripMenuItem.Enabled = false;
            }
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CambiarContraseña_013AL form = new CambiarContraseña_013AL();
            form.ShowDialog();

            if (form.SesionCerrada)
            {
                UpdateSessionStatus(); 
            }
        }

        private void mnuGestorUsuarios_Click(object sender, EventArgs e)
        {
            GestorUsuarios_013AL form = new GestorUsuarios_013AL();
            form.ShowDialog();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            var lm = LanguageManager_013AL.ObtenerInstancia_013AL();
            lm.CargarIdioma_013AL();  // carga el idioma por defecto (español)
            lm.Notificar_013AL();

            if (SingletonSession_013AL.Instance.IsLoggedIn_013AL() == false)
            {
                toolStripStatusLabel.Text = "No hay ningún usuario conectado.";
                cambiarContraseñaToolStripMenuItem.Enabled = false;

                var permisos = ObtenerTodosLosPermisos(SingletonSession_013AL.Instance.Permisos_013AL);

                menuStrip1.Items["mnuGestores"].Enabled = false;
                menuStrip1.Items["menuventa"].Enabled = false;
                menuStrip1.Items["menucliente"].Enabled = false;

                menuStrip1.Items["menureportes"].Enabled = false;
                menuStrip1.Items["menuayuda"].Enabled = false;
                menuStrip1.Items["comprasToolStripMenuItem"].Enabled = false;

                foreach (var permiso in permisos)
                {
                    if (permiso.Nombre_013AL == "mnuGestores")
                    {
                        menuStrip1.Items["mnuGestores"].Enabled = true;
                    }
                    if (permiso.Nombre_013AL == "menuventa")
                    {
                        menuStrip1.Items["menuventa"].Enabled = true;
                    }
                    if (permiso.Nombre_013AL == "menucliente")
                    {
                        menuStrip1.Items["menucliente"].Enabled = true;
                    }

                    if (permiso.Nombre_013AL == "menureportes")
                    {
                        menuStrip1.Items["menureportes"].Enabled = true;
                    }
                    if (permiso.Nombre_013AL == "menuayuda")
                    {
                        menuStrip1.Items["menuayuda"].Enabled = true;
                    }
                    if (permiso.Nombre_013AL == "comprasToolStripMenuItem")
                    {
                        menuStrip1.Items["comprasToolStripMenuItem"].Enabled = true;
                    }
                }

            
            }
            foreach (Control ctl in this.Controls)
            {
                try
                {
                    if (ctl is MdiClient mdiClient)
                    {
                        mdiClient.BackColor = this.BackColor;
                    }
                }
                catch (InvalidCastException)
                {
                    MessageBox.Show("Error al cambiar el color de fondo del MDI Client.");
                }

            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            toolStripSesion.Text = "Usuario";
        }

        private void mnuGestorPermisos_Click(object sender, EventArgs e)
        {
            GestorPerfiles_013AL form = new GestorPerfiles_013AL();
            form.ShowDialog();
        }

        private void cambiarIdiomaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CambiarIdioma_013AL form = new CambiarIdioma_013AL();
            form.ShowDialog();
        }

        private void eventosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BitácoraEventos_013AL form = new BitácoraEventos_013AL();
            form.ShowDialog();
        }

        private void respaldosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Respaldos_013AL respaldos = new Respaldos_013AL();
            respaldos.ShowDialog();
        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrarCliente_013AL form = new RegistrarCliente_013AL();
            form.ShowDialog();
        }

        private void cotizacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cotizaciones_013AL form = new Cotizaciones_013AL();
            form.ShowDialog();
        }

        private void productosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Productos_013AL form = new Productos_013AL();
            form.ShowDialog();
        }

        private void productosCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Productos_C_013AL form = new Productos_C_013AL();
            form.ShowDialog();
        }

        private void proveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Proveedores_013AL form = new Proveedores_013AL();
            form.ShowDialog();
        }

        private void ordenDeCompraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdenCompraForm_013AL form = new OrdenCompraForm_013AL();
            form.ShowDialog();
        }


        private void recepciónDeProductosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecepciónProductos_013AL form = new RecepciónProductos_013AL();
            form.ShowDialog();
        }

        private void gestorDeIdiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GestionarIdiomas_013AL form = new GestionarIdiomas_013AL();
            form.ShowDialog();
        }

        private void generarPedidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrarPedido_013AL form = new RegistrarPedido_013AL();
            form.ShowDialog();
        }

        private void controlFormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlFormal_013AL form = new ControlFormal_013AL();
            form.ShowDialog();
        }

        private void menuCobrarVenta_Click(object sender, EventArgs e)
        {
            CobrarVenta_013AL form = new CobrarVenta_013AL();
            form.ShowDialog();
        }

        private void pagarProductosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PagarProducto_013AL form = new PagarProducto_013AL();
            form.ShowDialog();
        }
    }
}
    







