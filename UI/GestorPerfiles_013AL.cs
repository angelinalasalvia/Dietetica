using BE_013AL;
using BE_013AL.Composite;
using BLL;
using BLL_013AL;
using Servicios;
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
        public GestorPerfiles_013AL()
        {
            InitializeComponent();
            /*CargarPatentesCombo();
            CargarFamiliasCombo();
            CargarRolesCombo();*/
            //InitializeTreeView();
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

            // obtener id de rol
            string[] partesRol = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int idRol = int.Parse(partesRol[0].Trim());

            // comprobar si seleccionó en listBoxPermisos
            if (listBoxPermisos.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleccione un permiso para agregar");
                return;
            }

            Rol_013AL permisoSeleccionado = TraerComponeneteDeListBox_013AL(listBoxPermisos);
            if (permisoSeleccionado == null) return;

            if (ExisteConflicto_013AL(permisoSeleccionado))
            {
                // Los mensajes de conflicto los maneja ExisteConflicto_
                return;
            }

            // Intentar insertar en BD: usá el método BLL que tengas (en tu código principal usabas InsertarFamiliaRol_013AL en btnAplicar)
            string insertado = fbll.InsertarFamiliaRol_013AL(idRol, permisoSeleccionado.Cod_013AL); // si no existe, crear wrapper en BLL

            if (insertado == "OK")
            {
                // Guardar en memoria y refrescar vista
                RolConfigurado.AgregarHijo_013AL(permisoSeleccionado);
                MostrarRolEnTreeView(RolConfigurado);
                MessageBox.Show("Permiso/familia agregado al rol correctamente.");
            }
            else
            {
                MessageBox.Show("No se pudo agregar el permiso/familia. Puede que ya exista la relación.");
            }
        }






        private void MostrarRolEnTreeView(Familia_013AL rol)
        {
            treeViewRol.Nodes.Clear();

            // Asegurarse de que el rol tenga toda su estructura cargada antes de mostrar
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



        // --- Cargar hijos del rol desde BD recursivamente (como en GestionarFamilias) ---
        private void CargarHijosDesdeBDRecursivoParaRol(Familia_013AL familia)
        {
            List<Rol_013AL> hijos = rbll.TraerListaHijos_013AL(familia.Cod_013AL);
            foreach (var hijo in hijos)
            {
                // Evitar duplicados en memoria
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
            // Verificar si el componente ya está dentro del rol (en cualquier nivel)
            if (EstaEnFamiliaDelRol_013AL(permisoSeleccionado.Cod_013AL, RolConfigurado))
            {
                MessageBox.Show($"El componente {permisoSeleccionado.Nombre_013AL} ya existe dentro del rol (en otra familia).");
                return true;
            }

            // Si el seleccionado es una familia, verificar sus hijos también
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
            //int id = int.Parse(partes[0].Trim());
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

            // obtener id de rol
            string[] partesRol = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int idRol = int.Parse(partesRol[0].Trim());

            // comprobar si seleccionó en listBoxPermisos
            if (listBoxFamilias.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleccione una familia para agregar");
                return;
            }

            Rol_013AL permisoSeleccionado = TraerComponeneteDeListBox_013AL(listBoxFamilias);
            if (permisoSeleccionado == null) return;

            if (ExisteConflicto_013AL(permisoSeleccionado))
            {
                // Los mensajes de conflicto los maneja ExisteConflicto_
                return;
            }

            // Intentar insertar en BD: usá el método BLL que tengas (en tu código principal usabas InsertarFamiliaRol_013AL en btnAplicar)
            string insertado = fbll.InsertarFamiliaRol_013AL(idRol, permisoSeleccionado.Cod_013AL); // si no existe, crear wrapper en BLL

            if (insertado == "OK")
            {
                RolConfigurado.AgregarHijo_013AL(permisoSeleccionado);
                MostrarRolEnTreeView(RolConfigurado);
                MessageBox.Show("Permiso/familia agregado al rol correctamente.");
            }
            else
            {
                MessageBox.Show("No se pudo agregar el permiso/familia. Puede que ya exista la relación.");
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

            // Pregunta confirmación
            DialogResult dr = MessageBox.Show($"¿Eliminar {permisoSeleccionado.Nombre_013AL} del rol?", "Confirmar", MessageBoxButtons.YesNo);
            if (dr != DialogResult.Yes) return;

            bool eliminado = rbll.EliminarPermisoRol_013AL(idRol, permisoSeleccionado.Cod_013AL); // ya existe en BLL/DAL
            if (eliminado)
            {
                RolConfigurado.QuitarHijo_013AL(permisoSeleccionado);
                MostrarRolEnTreeView(RolConfigurado);
                MessageBox.Show("Eliminado correctamente del rol.");
            }
            else
            {
                MessageBox.Show("No se pudo eliminar la relación en la BD.");
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

            int respuesta = rbll.CrearRol_013AL(nombreRol);
            MessageBox.Show("Rol creado con éxito.");
            ActualizarComboBox_013AL();
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
                    rbll.EliminarRol_013AL(idRol);
                    MessageBox.Show("Rol eliminado correctamente.");
                    ActualizarComboBox_013AL();
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

                string resultado = rbll.ModificarRol_013AL(id, nuevoNombre);

                MessageBox.Show(resultado);

                ActualizarComboBox_013AL();
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
                RolConfigurado = new Familia_013AL(); // reset
                string[] partes = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int id = int.Parse(partes[0].Trim());
                string nombre = partes[1].Trim();

                RolConfigurado.Cod_013AL = id;
                RolConfigurado.Nombre_013AL = nombre;

                // Traer los permisos/familias raíz asignados al rol (solo primer nivel)
                List<Rol_013AL> lista = rbll.TraerListaPermisosRol_013AL(RolConfigurado.Cod_013AL);

                // Limpiar y poblar la estructura en memoria recursivamente
                RolConfigurado.ObtenerHijos_013AL().Clear();
                foreach (var permiso in lista)
                {
                    RolConfigurado.AgregarHijo_013AL(permiso);
                    // si es familia, cargar sus hijos recursivamente desde BD
                    if (permiso is Familia_013AL fam)
                    {
                        CargarHijosDesdeBDRecursivoParaRol(fam);
                    }
                }

                // Mostrar el rol completo en treeView
                MostrarRolEnTreeView(RolConfigurado);
            }
        
        }
    }
}