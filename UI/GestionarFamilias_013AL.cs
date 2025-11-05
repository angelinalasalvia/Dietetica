using BE_013AL;
using BE_013AL.Composite;
using BLL;
using BLL_013AL;
using Microsoft.VisualBasic.ApplicationServices;
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

namespace UI
{
    public partial class GestionarFamilias_013AL : Form, IObserver_013AL
    {
        public GestionarFamilias_013AL()
        {
            InitializeComponent();
        }
        PermisoBLL_013AL pbll = new PermisoBLL_013AL();
        FamiliaBLL_013AL fbll = new FamiliaBLL_013AL();
        RolBLL_013AL rbll = new RolBLL_013AL();
        EventoBLL_013AL bll = new EventoBLL_013AL();
        Usuarios_013AL user;

        private Familia_013AL FamiliaConfigurada = new Familia_013AL();
        private void GestionarFamilias_Load(object sender, EventArgs e)
        {
            ActualizarListBoxPermisos_013AL();
            ActualizarComboBox_013AL();
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

        private List<int> ObtenerPermisosDeFamilia(Familia_013AL familia)
        {
            List<int> listaPermisos = new List<int>();

            foreach (var hijo in familia.ObtenerHijos_013AL())
            {
                if (hijo is Permiso_013AL permiso)
                {
                    listaPermisos.Add(permiso.Cod_013AL);
                }
                else if (hijo is Familia_013AL subFamilia)
                {
                    
                    listaPermisos.AddRange(ObtenerPermisosDeFamilia(subFamilia));
                }
            }

            return listaPermisos.Distinct().ToList(); 
        }

        private void MostrarFamiliaEnTreeView(Familia_013AL familia)
        {
            treeViewFamilia.Nodes.Clear();

            TreeNode nodoRaiz = new TreeNode(familia.Nombre_013AL)
            {
                Tag = familia
            };

            treeViewFamilia.Nodes.Add(nodoRaiz);

            AgregarHijosAlTreeNode(familia, nodoRaiz);

            treeViewFamilia.ExpandAll();
        }

        private void AgregarHijosAlTreeNode(Familia_013AL familia, TreeNode nodoPadre)
        {
            foreach (var hijo in familia.ObtenerHijos_013AL())
            {
                TreeNode nodoHijo = new TreeNode($"{hijo.Nombre_013AL} ({hijo.Tipo_013AL})")
                {
                    Tag = hijo
                };

                nodoPadre.Nodes.Add(nodoHijo);

                if (hijo is Familia_013AL subFamilia)
                {
                    AgregarHijosAlTreeNode(subFamilia, nodoHijo);
                }
            }
        }


        private void CargarHijosEnTreeNode(Familia_013AL familia, TreeNode nodoPadre)
        {
            foreach (var hijo in familia.ObtenerHijos_013AL())
            {
                TreeNode nuevoNodo = new TreeNode($"{hijo.Nombre_013AL} ({hijo.Tipo_013AL})")
                {
                    Tag = hijo
                };

                nodoPadre.Nodes.Add(nuevoNodo);

                if (hijo is Familia_013AL subFamilia)
                {
                    CargarHijosEnTreeNode(subFamilia, nuevoNodo);
                }
            }
        }


        private void cmbFamilia_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem is Familia_013AL familiaSeleccionada)
            {
                try
                {
                    familiaSeleccionada.ObtenerHijos_013AL().Clear();

                    CargarHijosDesdeBDRecursivo(familiaSeleccionada);

                    MostrarFamiliaEnTreeView(familiaSeleccionada);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la familia: " + ex.Message);
                }
            }
        }

        private void CargarHijosDesdeBDRecursivo(Familia_013AL familia)
        {
            List<Rol_013AL> hijos = rbll.TraerListaHijos_013AL(familia.Cod_013AL);

            foreach (var hijo in hijos)
            {
                familia.AgregarHijo_013AL(hijo);

                if (hijo is Familia_013AL subFamilia)
                {
                    CargarHijosDesdeBDRecursivo(subFamilia);
                }
            }
        }

        private List<Familia_013AL> CargarTodasLasFamiliasDesdeBD()
        {
            List<Familia_013AL> familias = fbll.TraerListaFamilias_013AL();

            foreach (var fam in familias)
            {
                fam.ObtenerHijos_013AL().Clear();
                CargarHijosDesdeBDRecursivo(fam);
            }

            return familias;
        }

        private bool ExisteEnJerarquia(Familia_013AL familiaRaiz, int idComponenteBuscado)
        {
            foreach (var hijo in familiaRaiz.ObtenerHijos_013AL())
            {
                if (hijo.Cod_013AL == idComponenteBuscado)
                    return true;

                if (hijo is Familia_013AL subFamilia)
                {
                    if (ExisteEnJerarquia(subFamilia, idComponenteBuscado))
                        return true;
                }
            }

            return false;
        }

        private void ActualizarComboBox_013AL()
        {
            List<Familia_013AL> listaFamilias = fbll.TraerListaFamilias_013AL();

            cmbFamilia.DataSource = null; 
            cmbFamilia.DataSource = listaFamilias;
            cmbFamilia.DisplayMember = "Nombre_013AL";
            cmbFamilia.ValueMember = "Cod_013AL";
        }

        private void ActualizarListBoxPermisos_013AL()
        {
            listBoxPermisos.Items.Clear();
            cmbFamilia.Items.Clear(); 

            List<Rol_013AL> listaPermisos = pbll.TraerListaPermisos_013AL();

            foreach (var componente in listaPermisos)
            {
                string tipo = (componente is Familia_013AL) ? "Familia" : "Permiso";
                listBoxPermisos.Items.Add($"{componente.Cod_013AL} - {componente.Nombre_013AL} - {tipo}");

                
                if (componente is Familia_013AL)
                {
                    cmbFamilia.Items.Add(new KeyValuePair<int, string>(componente.Cod_013AL, componente.Nombre_013AL));
                }
            }

            cmbFamilia.DisplayMember = "Value"; 
            cmbFamilia.ValueMember = "Key";
        }

        private void btnAgregarPermiso_Click(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una familia antes de agregar permisos.");
                return;
            }

            if (listBoxPermisos.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un permiso o familia para agregar.");
                return;
            }

            Familia_013AL familiaSeleccionada = (Familia_013AL)cmbFamilia.SelectedItem;
            Rol_013AL componenteSeleccionado = TraerComponenteDeListBox(listBoxPermisos);

            if (componenteSeleccionado == null)
                return;

            if (componenteSeleccionado is Familia_013AL fam && fam.Cod_013AL == familiaSeleccionada.Cod_013AL)
            {
                MessageBox.Show("No puedes agregar una familia dentro de sí misma.");
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "No se puede agregar una familia dentro de sí misma.", 3);
                return;
            }

            if (componenteSeleccionado is Familia_013AL familiaAAgregar)
            {
                List<int> permisosFamiliaNueva = ObtenerPermisosDeFamilia(familiaAAgregar);
                List<int> permisosFamiliaDestino = ObtenerPermisosDeFamilia(familiaSeleccionada);

                if (permisosFamiliaNueva.Intersect(permisosFamiliaDestino).Any())
                {
                    MessageBox.Show("No se puede agregar esta familia porque contiene permisos que ya existen en la familia destino o sus subfamilias.");

                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "No se pudo agregar la familia porque contiene permisos que ya existen en la familia destino o sus subfamilias.", 3);
                    return;
                }
            }

            List<Familia_013AL> todasLasFamilias = CargarTodasLasFamiliasDesdeBD();
            Familia_013AL familiaRaiz = todasLasFamilias
                .FirstOrDefault(f => ExisteEnJerarquia(f, familiaSeleccionada.Cod_013AL)) ?? familiaSeleccionada;

            if (ExisteEnJerarquia(familiaRaiz, componenteSeleccionado.Cod_013AL))
            {
                MessageBox.Show("Este componente ya existe en otra familia dentro de la jerarquía.");
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "El componente ya existe en otra familia dentro de la jerarquía.", 3);

                return;
            }

            try
            {
                bool agregado = fbll.AsignarHijosAFamilia_013AL(
                    familiaSeleccionada.Cod_013AL,
                    new List<int> { componenteSeleccionado.Cod_013AL });

                if (agregado)
                {
                    familiaSeleccionada.AgregarHijo_013AL(componenteSeleccionado);
                    MostrarFamiliaEnTreeView(familiaSeleccionada);
                    MessageBox.Show("Componente agregado correctamente a la familia."); 
                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "Componente agregado correctamente a la familia.", 3);

                }
                else
                {
                    MessageBox.Show("El componente ya estaba asignado a la familia.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar componente: " + ex.Message);
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", $"Error al agregar el componente a la familia. {ex.Message}", 3);

            }
        }

        private void btnQuitarPermiso_Click(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una familia antes de intentar quitar permisos o subfamilias.");
                return;
            }

            if (treeViewFamilia.SelectedNode == null)
            {
                MessageBox.Show("Seleccione un permiso o familia del árbol para quitar.");
                return;
            }

            Rol_013AL componenteSeleccionado = treeViewFamilia.SelectedNode.Tag as Rol_013AL;
            if (componenteSeleccionado == null)
            {
                MessageBox.Show("El elemento seleccionado no es válido.");
                return;
            }

            Familia_013AL familiaSeleccionada = (Familia_013AL)cmbFamilia.SelectedItem;

            
            DialogResult resultado = MessageBox.Show(
                $"¿Está seguro de que desea quitar '{componenteSeleccionado.Nombre_013AL}' de la familia '{familiaSeleccionada.Nombre_013AL}'?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes)
                return;

            try
            {
                bool existeRelacion = fbll.VerificarPermisoEnFamilia_013AL(componenteSeleccionado.Cod_013AL, familiaSeleccionada.Cod_013AL);
                if (!existeRelacion)
                {
                    MessageBox.Show("Este componente no está asociado directamente a la familia seleccionada.");
                    return;
                }

                string respuesta = fbll.EliminarPermisoDeFamilia_013AL(componenteSeleccionado.Cod_013AL, familiaSeleccionada.Cod_013AL);
                MessageBox.Show(respuesta);

                
                familiaSeleccionada.QuitarHijo_013AL(componenteSeleccionado);

                
                familiaSeleccionada.ObtenerHijos_013AL().Clear();
                CargarHijosDesdeBDRecursivo(familiaSeleccionada);
                MostrarFamiliaEnTreeView(familiaSeleccionada);
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "Se quito el componente de la familia.", 3);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al quitar el componente: " + ex.Message);
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", $"Error al quitar el componente. {ex.Message}", 3);

            }
        }

        private Rol_013AL TraerComponenteDeListBox(ListBox listbox)
        {
            
            if (listbox.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un elemento de la lista.");
                return null; 
            }

            string[] partes = listbox.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length < 3)
            {
                MessageBox.Show("Formato incorrecto del elemento seleccionado.");
                return null;
            }

            if (!int.TryParse(partes[0].Trim(), out int id))
            {
                MessageBox.Show("Error al obtener el ID del elemento.");
                return null;
            }

            string nombre = partes[1].Trim();
            string tipo = partes[2].Trim();

            if (tipo == "Simple")
            {
                return new Permiso_013AL { Cod_013AL = id, Nombre_013AL = nombre, Tipo_013AL = "Simple" };
            }
            else
            {
                Familia_013AL nuevaFamilia = new Familia_013AL { Cod_013AL = id, Nombre_013AL = nombre, Tipo_013AL = "Familia" };

                List<Rol_013AL> hijos = rbll.TraerListaHijos_013AL(id);
                foreach (var hijo in hijos)
                {
                    nuevaFamilia.AgregarHijo_013AL(hijo);
                }

                return nuevaFamilia;
            }
        }


        private void btnCrear_Click(object sender, EventArgs e)
        {
            string nombreFamilia = txtNombreFamilia.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreFamilia))
            {
                MessageBox.Show("El nombre de la familia no puede estar vacío.");
                return;
            }

            if (fbll.ExisteFamilia_013AL(nombreFamilia))
            {
                MessageBox.Show("El nombre de la familia ya existe. Escriba un nombre diferente.");
                return;
            }
            try
            {
                int respuesta = fbll.CrearFamilia_013AL(nombreFamilia);
                MessageBox.Show("Familia creada con éxito.");
                ActualizarComboBox_013AL();
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "La familia se creó correctamente.", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la familia: " + ex.Message);
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "No se pudo agregar la familia.", 3);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem != null)
            {
                string[] partes = cmbFamilia.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int idFamilia = int.Parse(partes[0].Trim());

                string respuesta = fbll.EliminarFamilia_013AL(idFamilia);
                MessageBox.Show(respuesta);

                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "Familia eliminada correctamente.", 3);

                if (!respuesta.Contains("No se puede eliminar"))
                {
                    ActualizarComboBox_013AL();

                    user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bll.AgregarEvento_013AL(user.Login_013AL, "Gestor Perfiles", "No se pudo eliminar familia.", 3);
                }
            }
            else
            {
                MessageBox.Show("Seleccione una familia para eliminar.");
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem == null || string.IsNullOrWhiteSpace(txtNombreFamilia.Text))
            {
                MessageBox.Show("Seleccione una familia y escriba un nuevo nombre para modificar.");
                return;
            }

            string nuevoNombre = txtNombreFamilia.Text.Trim();
            string[] partes = cmbFamilia.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int idFamilia = int.Parse(partes[0].Trim());

            List<Familia_013AL> listaFamilias = fbll.TraerListaFamilias_013AL();
            if (listaFamilias.Any(f => f.Cod_013AL != idFamilia && f.Nombre_013AL.Equals(nuevoNombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("El nombre de la familia ya existe. Escriba un nombre diferente.");
                return;
            }

            Familia_013AL familiaModificada = new Familia_013AL { Cod_013AL = idFamilia, Nombre_013AL = nuevoNombre };
            try { 
            fbll.ModificarFamilia_013AL(familiaModificada);

            MessageBox.Show("Familia modificada con éxito.");
            ActualizarComboBox_013AL();
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "La familia se modifico correctamente", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar la familia: " + ex.Message);

                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bll.AgregarEvento_013AL(user.Login_013AL, "Gestionar Familias", "Error al modificar la familia", 3);
            }

        }

    }
}
