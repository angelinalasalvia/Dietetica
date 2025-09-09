using BE_013AL.Composite;
using BLL_013AL;
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
        PermisoBLL_013AL bll = new PermisoBLL_013AL();
        private Familia_013AL FamiliaConfigurada = new Familia_013AL();
        private void GestionarFamilias_Load(object sender, EventArgs e)
        {
            ActualizarListBoxPermisos_013AL();
            ActualizarComboBox_013AL();
            /*List<Componente> listaComponentes = ObtenerComponentes(); // Tu método para obtener la lista
            listBoxFamilia.DataSource = listaComponentes;*/
            listBoxFamilia.DisplayMember = "Nombre_013AL";
            listBoxFamilia.ValueMember = "Cod_013AL";
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

        /*private void ActualizarListBoxFamilia()
        {
            listBoxFamilia.Items.Clear();
            foreach (Componente permiso in FamiliaConfigurada.ObtenerHijos())
            {
                listBoxFamilia.Items.Add($"{permiso.Id} - {permiso.Nombre} - {permiso.Tipo}");
            }
        }*/

        private void ActualizarComboBox_013AL()
        {
            /*cmbFamilia.Items.Clear();
            List<Familia> listaFamilias = bll.TraerListaFamilias();
            foreach (var familia in listaFamilias)
            {
                if (familia is Familia)
                {
                    cmbFamilia.Items.Add($"{familia.Id} - {familia.Nombre}");
                }
            }*/
            List<Familia_013AL> listaFamilias = bll.TraerListaFamilias_013AL();

            cmbFamilia.DataSource = null; // Resetear el DataSource antes de asignarlo nuevamente
            cmbFamilia.DataSource = listaFamilias;
            cmbFamilia.DisplayMember = "Nombre_013AL";
            cmbFamilia.ValueMember = "Cod_013AL";
        }

        private void ActualizarListBoxPermisos_013AL()
        {
            listBoxPermisos.Items.Clear();
            cmbFamilia.Items.Clear(); // Evita duplicados en el ComboBox

            List<Componente_013AL> listaPermisos = bll.TraerListaPermisos_013AL();

            foreach (var componente in listaPermisos)
            {
                string tipo = (componente is Familia_013AL) ? "Familia" : "Permiso";
                listBoxPermisos.Items.Add($"{componente.Cod_013AL} - {componente.Nombre_013AL} - {tipo}");

                // Agregar solo familias al ComboBox (si es necesario)
                if (componente is Familia_013AL)
                {
                    cmbFamilia.Items.Add(new KeyValuePair<int, string>(componente.Cod_013AL, componente.Nombre_013AL));
                }
            }

            // Configurar DisplayMember y ValueMember fuera del foreach
            cmbFamilia.DisplayMember = "Value"; // Muestra el nombre
            cmbFamilia.ValueMember = "Key";
        }

        private void btnAgregarPermiso_Click(object sender, EventArgs e)
        {
            if (listBoxPermisos.SelectedItems.Count > 0)
            {
                Componente_013AL componenteSeleccionado = TraerComponenteDeListBox(listBoxPermisos);

                if (componenteSeleccionado is Familia_013AL familiaSeleccionada)
                {
                    // Verificar que no haya recursión infinita (evitar agregar una familia dentro de sí misma)
                    if (familiaSeleccionada.Cod_013AL == FamiliaConfigurada.Cod_013AL)
                    {
                        MessageBox.Show("No puedes agregar una familia dentro de sí misma.");
                        return;
                    }
                }

                FamiliaConfigurada.AgregarHijo_013AL(componenteSeleccionado);
                ActualizarListBoxFamilia_013AL();
            }
            else
            {
                MessageBox.Show("Seleccione un permiso o familia para agregar");
            }
            /*if (listBoxPermisos.SelectedItems.Count > 0)
            {
                Componente permisoSeleccionado = TraerComponeneteDeListBox(listBoxPermisos);

                FamiliaConfigurada.AgregarHijo(permisoSeleccionado);
                ActualizarListBoxFamilia();

            }
            else { MessageBox.Show("Seleccione un permiso para agregar"); }*/
        }

        private void btnQuitarPermiso_Click(object sender, EventArgs e)
        {
            if (listBoxFamilia.SelectedItems.Count > 0)
            {
                Componente_013AL permisoSeleccionado = TraerComponenteDeListBox(listBoxFamilia);

                FamiliaConfigurada.QuitarHijo_013AL(permisoSeleccionado);
                ActualizarListBoxFamilia_013AL();
            }
            else { MessageBox.Show("Seleccione un permiso del Rol configurado para quitar"); }
        }

        private Componente_013AL TraerComponenteDeListBox(ListBox listbox)
        {
            // Validar si hay un elemento seleccionado
            if (listbox.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un elemento de la lista.");
                return null; // Retornar null si no hay selección
            }

            string[] partes = listbox.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            // Validar que el formato esperado tenga al menos 3 partes
            if (partes.Length < 3)
            {
                MessageBox.Show("Formato incorrecto del elemento seleccionado.");
                return null;
            }

            // Convertir el ID a entero de forma segura
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

                // Cargar hijos si es necesario
                List<Componente_013AL> hijos = bll.TraerListaHijos_013AL(id);
                foreach (var hijo in hijos)
                {
                    nuevaFamilia.AgregarHijo_013AL(hijo);
                }

                return nuevaFamilia;
            }
        }


        private void ActualizarListBoxFamilia_013AL()
        {
            listBoxFamilia.Items.Clear();
            foreach (Componente_013AL componente in FamiliaConfigurada.ObtenerHijos_013AL())
            {
                listBoxFamilia.Items.Add(componente); // Agregamos el objeto, no un string
            }
            listBoxFamilia.DisplayMember = "Nombre"; // Asegurar que se muestra el nombre
            /*listBoxFamilia.Items.Clear();
            foreach (Componente permiso in FamiliaConfigurada.ObtenerHijos())
            {
                listBoxFamilia.Items.Add($"{permiso.Id} - {permiso.Nombre} - {permiso.Tipo}");
            }*/
        }

        private void BloquearBotones()
        {
            btnCrear.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
        }

        private void cmbFamilia_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem != null)
            {
                FamiliaConfigurada.ObtenerHijos_013AL().Clear();
                listBoxFamilia.Items.Clear();

                Familia_013AL familiaSeleccionada = (Familia_013AL)cmbFamilia.SelectedItem;
                int id = familiaSeleccionada.Cod_013AL;

                List<Componente_013AL> listaHijos = bll.TraerListaHijos_013AL(id);
                foreach (var hijo in listaHijos)
                {
                    listBoxFamilia.Items.Add($"{hijo.Cod_013AL} - {hijo.Nombre_013AL} - {hijo.Tipo_013AL}");
                    FamiliaConfigurada.AgregarHijo_013AL(hijo);
                }
            }
            /*if (cmbFamilia.SelectedItem != null)
            {
                FamiliaConfigurada.ObtenerHijos().Clear();
                listBoxFamilia.Items.Clear();
                string[] partes = cmbFamilia.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int id = int.Parse(partes[0].Trim());

                List<Componente> listaHijos = bll.TraerListaHijos(id);
                foreach (var hijo in listaHijos)
                {
                    listBoxFamilia.Items.Add($"{hijo.Id} - {hijo.Nombre} - {hijo.Tipo}");
                    FamiliaConfigurada.AgregarHijo(hijo);
                }
            }*/

        }

        private void ResetearBotones()
        {
            btnCrear.Enabled = true;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            cmbFamilia.SelectedItem = null;

        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            string nombreFamilia = txtNombreFamilia.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreFamilia))
            {
                MessageBox.Show("El nombre de la familia no puede estar vacío.");
                return;
            }

            if (bll.ExisteFamilia_013AL(nombreFamilia))
            {
                MessageBox.Show("El nombre de la familia ya existe. Escriba un nombre diferente.");
                return;
            }

            int respuesta = bll.CrearFamilia_013AL(nombreFamilia);
            MessageBox.Show("Familia creada con éxito.");
            ActualizarComboBox_013AL();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem != null)
            {
                string[] partes = cmbFamilia.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int idFamilia = int.Parse(partes[0].Trim());

                string respuesta = bll.EliminarFamilia_013AL(idFamilia);
                MessageBox.Show(respuesta);

                if (!respuesta.Contains("No se puede eliminar"))
                {
                    ActualizarComboBox_013AL();
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

            // Verificar si el nuevo nombre ya existe en otra familia
            List<Familia_013AL> listaFamilias = bll.TraerListaFamilias_013AL();
            if (listaFamilias.Any(f => f.Cod_013AL != idFamilia && f.Nombre_013AL.Equals(nuevoNombre, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("El nombre de la familia ya existe. Escriba un nombre diferente.");
                return;
            }

            Familia_013AL familiaModificada = new Familia_013AL { Cod_013AL = idFamilia, Nombre_013AL = nuevoNombre };
            bll.ModificarFamilia_013AL(familiaModificada);

            MessageBox.Show("Familia modificada con éxito.");
            ActualizarComboBox_013AL();
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una familia para asignarle permisos.");
                return;
            }

            if (listBoxFamilia.Items.Count == 0)
            {
                MessageBox.Show("No hay permisos o familias para asignar.");
                return;
            }

            // Obtener la familia seleccionada
            Familia_013AL familiaSeleccionada = cmbFamilia.SelectedItem as Familia_013AL;
            if (familiaSeleccionada == null)
            {
                MessageBox.Show("Error al obtener la familia seleccionada.");
                return;
            }

            int idFamilia = familiaSeleccionada.Cod_013AL;

            // Obtener los permisos seleccionados en el ListBox
            List<int> hijos = new List<int>();
            foreach (Componente_013AL item in listBoxFamilia.Items)
            {
                hijos.Add(item.Cod_013AL);
            }

            try
            {
                bool permisosAsignados = bll.AsignarHijosAFamilia_013AL(idFamilia, hijos);

                if (permisosAsignados)
                {
                    MessageBox.Show("Permisos asignados correctamente.");
                }
                else
                {
                    MessageBox.Show("Todos los permisos ya estaban asignados.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asignar permisos: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (cmbFamilia.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una familia antes de intentar eliminar permisos.");
                return;
            }

            if (listBoxFamilia.Items.Count == 0)
            {
                MessageBox.Show("No hay permisos asignados a esta familia para eliminar.");
                return;
            }

            // Obtener el ID de la familia correctamente
            int idFamilia;
            try
            {
                idFamilia = Convert.ToInt32(cmbFamilia.SelectedValue);
            }
            catch
            {
                MessageBox.Show("Error al obtener el ID de la familia. Verifique la configuración del ComboBox.");
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar todos los permisos de esta familia?",
                                                     "Confirmación",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                // Crear una lista temporal para evitar modificar la colección mientras se itera
                List<Componente_013AL> permisosAEliminar = new List<Componente_013AL>();

                // Recorrer TODOS los ítems del ListBox y agregarlos a la lista temporal
                foreach (var item in listBoxFamilia.Items)
                {
                    if (item is Componente_013AL permiso) // Verifica que el item es un Componente válido
                    {
                        permisosAEliminar.Add(permiso);
                    }
                }

                // Ahora eliminar cada permiso en la lista
                foreach (Componente_013AL permiso in permisosAEliminar)
                {
                    if (bll.VerificarPermisoEnFamilia_013AL(permiso.Cod_013AL, idFamilia))
                    {
                        // Si la relación existe, eliminar el permiso
                        FamiliaConfigurada.QuitarHijo_013AL(permiso);
                        bll.VerificarPermisoEnFamilia_013AL(permiso.Cod_013AL, idFamilia);
                        listBoxFamilia.Items.Remove(permiso);
                    }
                    else
                    {
                        // Si la relación NO existe, mostrar mensaje
                        MessageBox.Show($"El permiso '{permiso.Nombre_013AL}' no está asociado a la familia y no puede eliminarse.",
                                        "Información",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                }

                MessageBox.Show("Proceso de eliminación finalizado.");
                ActualizarListBoxFamilia_013AL();
            }
        }
    }
}
