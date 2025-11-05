using BE_013AL;
using BE_013AL.Composite;
using BLL;
using BLL_013AL;
using Microsoft.VisualBasic.ApplicationServices;
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
    public partial class GestorPerfiles_013AL : Form, IObserver_013AL
    {
        PermisoBLL_013AL pbll = new PermisoBLL_013AL();
        RolBLL_013AL rbll = new RolBLL_013AL();
        FamiliaBLL_013AL fbll = new FamiliaBLL_013AL();
        EventoBLL_013AL bll = new EventoBLL_013AL();
        Usuarios_013AL user;
        public GestorPerfiles_013AL()
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

        private void GestorPerfiles_Load(object sender, EventArgs e) 
        { 
            ActualizarListBoxPermisosYFamilias_013AL(); 
            ActualizarComboBox_013AL();
            listBoxFamilias.SelectedIndexChanged += listBoxFamilias_SelectedIndexChanged;

        }

        private Familia_013AL RolConfigurado = new Familia_013AL();

        private Familia_013AL rolAModifcarOEliminar = new Familia_013AL();

        private void btnAgregarPermiso_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un rol antes de agregar permisos.");
                return;
            }

            string[] partesRol = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int idRol = int.Parse(partesRol[0].Trim());

            if (listBoxPermisos.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleccione un permiso para agregar");
                return;
            }

            Rol_013AL permisoSeleccionado = TraerComponeneteDeListBox_013AL(listBoxPermisos);
            if (permisoSeleccionado == null) return;

            if (ExisteConflicto_013AL(permisoSeleccionado))
            {
                return;
            }

            string insertado = fbll.InsertarFamiliaRol_013AL(idRol, permisoSeleccionado.Cod_013AL); 
            if (insertado == "OK")
            {
                RolConfigurado.AgregarHijo_013AL(permisoSeleccionado);
                MostrarRolEnTreeView(RolConfigurado);
                MessageBox.Show("Permiso agregado al rol correctamente.");
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "Permiso agregado al rol correctamente.", 3);
            }
            else
            {
                MessageBox.Show("No se pudo agregar el permiso. Puede que ya exista la relación.");
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "No se pudo agregar el permiso al rol", 3);
            }
        }






        private void MostrarRolEnTreeView(Familia_013AL rol)
        {
            treeViewRol.Nodes.Clear();

            CargarHijosDesdeBDRecursivoParaRol(rol);

            TreeNode nodoRaiz = new TreeNode($"{rol.Nombre_013AL} ({rol.Cod_013AL})")
            {
                Tag = rol
            };

            treeViewRol.Nodes.Add(nodoRaiz);
            AgregarHijosAlTreeNodeRol(rol, nodoRaiz);
            treeViewRol.ExpandAll();
        }

        private void AgregarHijosAlTreeNodeRol(Familia_013AL familia, TreeNode nodoPadre)
        {
            foreach (var hijo in familia.ObtenerHijos_013AL())
            {
                string tipo = hijo is Familia_013AL ? "Familia" : "Permiso";

                TreeNode nodoHijo = new TreeNode($"{hijo.Nombre_013AL} ({tipo}) - ID: {hijo.Cod_013AL}")
                {
                    Tag = hijo
                };

                nodoPadre.Nodes.Add(nodoHijo);

                if (hijo is Familia_013AL subFamilia)
                {
                    CargarHijosDesdeBDRecursivoParaRol(subFamilia);
                    AgregarHijosAlTreeNodeRol(subFamilia, nodoHijo);
                }
            }
        }


        private void CargarHijosDesdeBDRecursivoParaRol(Familia_013AL familia)
        {
            List<Rol_013AL> hijos = rbll.TraerListaHijos_013AL(familia.Cod_013AL);
            foreach (var hijo in hijos)
            {
                if (!familia.ObtenerHijos_013AL().Any(h => h.Cod_013AL == hijo.Cod_013AL))
                    familia.AgregarHijo_013AL(hijo);

                if (hijo is Familia_013AL subFamilia)
                {
                    CargarHijosDesdeBDRecursivoParaRol(subFamilia);
                }
            }
        }





        private bool ExisteConflicto_013AL(Rol_013AL permisoSeleccionado)
        {
            if (EstaEnFamiliaDelRol_013AL(permisoSeleccionado.Cod_013AL, RolConfigurado))
            {
                MessageBox.Show($"El componente {permisoSeleccionado.Nombre_013AL} ya existe dentro del rol (en otra familia).");
                return true;
            }

            if (permisoSeleccionado is Familia_013AL familiaSeleccionada)
            {
                List<Rol_013AL> hijos = rbll.TraerListaHijos_013AL(familiaSeleccionada.Cod_013AL);
                foreach (var hijo in hijos)
                {
                    if (EstaEnFamiliaDelRol_013AL(hijo.Cod_013AL, RolConfigurado))
                    {
                        MessageBox.Show($"No se puede agregar '{familiaSeleccionada.Nombre_013AL}' porque contiene '{hijo.Nombre_013AL}', que ya existe dentro del rol.");
                        return true;
                    }
                }
            }

            return false;
        }

        private bool EstaEnFamiliaDelRol_013AL(int idPermiso, Familia_013AL familia)
        {
            foreach (var componente in familia.ObtenerHijos_013AL())
            {
                if (componente.Cod_013AL == idPermiso)
                    return true; 

                if (componente is Familia_013AL subFamilia)
                {
                    if (EstaEnFamiliaDelRol_013AL(idPermiso, subFamilia)) 
                        return true;
                }
            }
            return false;
        }
        
        private void ActualizarListBoxPermisosYFamilias_013AL()
        {
            listBoxPermisos.Items.Clear();
            List<Rol_013AL> listaPermisos = pbll.TraerListaPermisosHijo_013AL();
            foreach (var permiso in listaPermisos)
            {
                if (permiso is Permiso_013AL)
                {
                    listBoxPermisos.Items.Add($"{permiso.Cod_013AL} - {permiso.Nombre_013AL} - {permiso.Tipo_013AL}");
                }
            }


            listBoxFamilias.Items.Clear();
            List<Familia_013AL> listaFamilias = fbll.TraerListaFamilias_013AL();
            foreach (var permiso in listaFamilias)
            {
                if (permiso is Familia_013AL)
                {
                    listBoxFamilias.Items.Add($"{permiso.Cod_013AL} - {permiso.Nombre_013AL} - {permiso.Tipo_013AL}");
                }
            }

        }
        private void ActualizarComboBox_013AL()
        {
            cmbRoles.Items.Clear();
            List<Familia_013AL> listaRoles = rbll.TraerListaRoles_013AL();
            foreach (var rol in listaRoles)
            {
                cmbRoles.Items.Add($"{rol.Cod_013AL} - {rol.Nombre_013AL}");
            }
        }

        private void listBoxFamilias_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxPermisoFamilia.Items.Clear();
            if (listBoxFamilias.SelectedItems.Count > 0)
            {
                Rol_013AL familiaSeleccionada = TraerComponeneteDeListBox_013AL(listBoxFamilias);
                List<Rol_013AL> lista = rbll.TraerListaHijos_013AL(familiaSeleccionada.Cod_013AL);

                foreach (Rol_013AL permiso in lista)
                {
                    listBoxPermisoFamilia.Items.Add($"{permiso.Cod_013AL} - {permiso.Nombre_013AL} - {permiso.Tipo_013AL}");
                }
            }
        }

        
        private Rol_013AL TraerComponeneteDeListBox_013AL(ListBox listbox)
        {
            string[] partes = listbox.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length < 3)
            {
                MessageBox.Show("El formato del elemento seleccionado no es válido.");
                return null;
            }

            if (!int.TryParse(partes[0].Trim(), out int id))
            {
                MessageBox.Show("El ID del permiso no es un número válido.");
                return null;
            }
            string nombre = partes[1].Trim();
            string tipo = partes[2].Trim();
            Rol_013AL componenteSeleccionado;
            if (tipo == "Simple")
            {
                componenteSeleccionado = new Permiso_013AL { Cod_013AL = id, Nombre_013AL = nombre, Tipo_013AL = "Simple" };
            }
            else { componenteSeleccionado = new Familia_013AL { Cod_013AL = id, Nombre_013AL = nombre, Tipo_013AL = "Familia" }; }
            return componenteSeleccionado;
        }

       
        private void btnAgregarFamilia_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un rol antes de agregar permisos.");
                return;
            }

            string[] partesRol = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int idRol = int.Parse(partesRol[0].Trim());

            if (listBoxFamilias.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleccione una familia para agregar");
                return;
            }

            Rol_013AL permisoSeleccionado = TraerComponeneteDeListBox_013AL(listBoxFamilias);
            if (permisoSeleccionado == null) return;

            if (ExisteConflicto_013AL(permisoSeleccionado))
            {
                return;
            }

            
            string insertado = fbll.InsertarFamiliaRol_013AL(idRol, permisoSeleccionado.Cod_013AL);

            if (insertado == "OK")
            {
                RolConfigurado.AgregarHijo_013AL(permisoSeleccionado);
                MostrarRolEnTreeView(RolConfigurado);
                MessageBox.Show("Familia agregado al rol correctamente.");

                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "Permiso/familia agregado al rol correctamente.", 3);
            }
            else
            {
                MessageBox.Show("No se pudo agregar el familia. Puede que ya exista la relación.");
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "No se pudo agregar el familia al rol.", 3);
            }
        }

        private void btnQuitarPermiso_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un rol antes de intentar quitar permisos.");
                return;
            }

            if (treeViewRol.SelectedNode == null)
            {
                MessageBox.Show("Seleccione un permiso o familia del rol configurado para quitar.");
                return;
            }

            string[] partesRol = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int idRol = int.Parse(partesRol[0].Trim());

            Rol_013AL permisoSeleccionado = treeViewRol.SelectedNode.Tag as Rol_013AL;
            if (permisoSeleccionado == null)
            {
                MessageBox.Show("Elemento seleccionado inválido.");
                return;
            }

            DialogResult dr = MessageBox.Show($"¿Eliminar {permisoSeleccionado.Nombre_013AL} del rol?", "Confirmar", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            bool eliminado = rbll.EliminarPermisoRol_013AL(idRol, permisoSeleccionado.Cod_013AL); 
            if (eliminado)
            {
                RolConfigurado.QuitarHijo_013AL(permisoSeleccionado);
                MostrarRolEnTreeView(RolConfigurado);
                MessageBox.Show("Eliminado correctamente del rol.");
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "Permiso/familia eliminado correctamente del rol.", 3);
            }
            else
            {
                MessageBox.Show("No se pudo eliminar la relación en la BD.");
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "No se pudo eliminar Permiso/familia del rol.", 3);
            }
        }

        



        private void btnGestionarFamilias_Click(object sender, EventArgs e)
        {
            GestionarFamilias_013AL form = new GestionarFamilias_013AL();
            form.ShowDialog();
            ActualizarListBoxPermisosYFamilias_013AL();
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            string nombreRol = txtNombreRol.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                MessageBox.Show("El nombre del rol no puede estar vacío.");
                return;
            }

            if (rbll.ExisteRol_013AL(nombreRol))
            {
                MessageBox.Show("El nombre del rol ya existe. Escriba un nombre diferente.");
                return;
            }

            try
            {
                int respuesta = rbll.CrearRol_013AL(nombreRol);
                MessageBox.Show("Rol creado con éxito.");
                ActualizarComboBox_013AL();
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "Se creó nuevo rol", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el rol: {ex.Message}");
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", $"Error al crear rol {ex.Message}", 3);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem != null)
            {
                string[] partes = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int idRol = int.Parse(partes[0].Trim());

                
                if (rbll.RolTieneRelaciones_013AL(idRol))
                {
                    MessageBox.Show("No se puede eliminar el rol porque tiene permisos o familias asociadas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este rol?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resultado == DialogResult.Yes)
                {
                    try
                    {
                        rbll.EliminarRol_013AL(idRol);
                        MessageBox.Show("Rol eliminado correctamente.");
                        ActualizarComboBox_013AL();

                        user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", $"Se eliminó rol: {cmbRoles.Text} ", 3);
                    }
                    catch(Exception ex) { 
                        MessageBox.Show($"Error al eliminar el rol: {ex.Message}");
                        user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", $"Error al eliminar rol {cmbRoles.Text}: {ex.Message}", 3);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un rol para eliminar.");
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem != null)
            {
                string[] partes = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int id = int.Parse(partes[0].Trim());
                string nombreActual = partes[1].Trim();
                string nuevoNombre = txtNombreRol.Text.Trim();

                if (string.IsNullOrEmpty(nuevoNombre))
                {
                    MessageBox.Show("Ingrese un nuevo nombre para el rol.");
                    return;
                }

                try
                {
                    string resultado = rbll.ModificarRol_013AL(id, nuevoNombre);

                    MessageBox.Show(resultado);

                    ActualizarComboBox_013AL();

                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", $"Se modifico el rol {cmbRoles.Text} por {nuevoNombre}", 3);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al modificar el rol: {ex.Message}");
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", $"Error al modificar el rol {cmbRoles.Text}: {ex.Message}", 3);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un rol para modificar.");
            }
        }

        private void cmbRoles_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem != null)
            {
                RolConfigurado = new Familia_013AL(); 
                string[] partes = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int id = int.Parse(partes[0].Trim());
                string nombre = partes[1].Trim();

                RolConfigurado.Cod_013AL = id;
                RolConfigurado.Nombre_013AL = nombre;

                List<Rol_013AL> lista = rbll.TraerListaPermisosRol_013AL(RolConfigurado.Cod_013AL);

                RolConfigurado.ObtenerHijos_013AL().Clear();
                foreach (var permiso in lista)
                {
                    RolConfigurado.AgregarHijo_013AL(permiso);
                    if (permiso is Familia_013AL fam)
                    {
                        CargarHijosDesdeBDRecursivoParaRol(fam);
                    }
                }

                MostrarRolEnTreeView(RolConfigurado);
            }
        
        }
    }
}