using BE_013AL;
using BE_013AL.Composite;
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
        //private PermisoBLL bllper = new PermisoBLL();

        private Usuarios_013AL _usuario;

        public Inicio_013AL()
        {
            InitializeComponent();
            
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            SingletonSession_013AL.Instance.IdiomaActual_013AL = "es";
            //ActualizarIdioma();
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

        private List<Componente_013AL> ObtenerTodosLosPermisos(List<Componente_013AL> permisosCompuestos)
        {
            List<Componente_013AL> lista = new List<Componente_013AL>();

            foreach (var permiso in permisosCompuestos)
            {
                lista.Add(permiso); // agrega el permiso/familia actual

                var hijos = permiso.ObtenerHijos_013AL();
                if (hijos != null && hijos.Count > 0)
                {
                    lista.AddRange(ObtenerTodosLosPermisos(hijos)); // recursivamente agrega hijos
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
                            bool habilitarregiscom = permisos.Any((P) => P.Nombre_013AL ==   "registrarCompraToolStripMenuItem");
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
                                    else if (habilitarregiscom && subItem.Name == "registrarCompraToolStripMenuItem")
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
                    BLLBitacora_013AL bbll = new BLLBitacora_013AL();
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
                UpdateSessionStatus(); // Actualiza el label y desactiva menús
            }
        }

        private void mnuGestorUsuarios_Click(object sender, EventArgs e)
        {
            GestorUsuarios_013AL form = new GestorUsuarios_013AL();
            form.ShowDialog();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
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
                    // No hacer nada si no es un MdiClient
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

        private void menuventa_Click(object sender, EventArgs e)
        {
            GenerarFactura_013AL form = new GenerarFactura_013AL();
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

        private void registrarCompraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegistrarCompra_013AL form = new RegistrarCompra_013AL();
            form.ShowDialog();
        }

        private void recepciónDeProductosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecepciónProductos_013AL form = new RecepciónProductos_013AL();
            form.ShowDialog();
        }
    }
}
    







