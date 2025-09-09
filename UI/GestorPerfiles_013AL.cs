using BE_013AL;
using BE_013AL.Composite;
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
        PermisoBLL_013AL bll = new PermisoBLL_013AL();
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
       
        private Familia_013AL RolConfigurado = new Familia_013AL();


        private Familia_013AL rolAModifcarOEliminar = new Familia_013AL();

        private void btnAgregarPermiso_Click(object sender, EventArgs e)
        {
            if (listBoxPermisos.SelectedItems.Count > 0)
            {
                Componente_013AL permisoSeleccionado = TraerComponeneteDeListBox_013AL(listBoxPermisos);
                if (!ExisteConflicto_013AL(permisoSeleccionado))
                {
                    RolConfigurado.AgregarHijo_013AL(permisoSeleccionado);
                    ActualizarListBoxRol_013AL();
                }
            }
            else { MessageBox.Show("Seleccione un permiso para agregar"); }
        }

        private void GestorPerfiles_Load(object sender, EventArgs e)
        {
            ActualizarListBoxPermisosYFamilias_013AL();
            ActualizarComboBox_013AL();

            listBoxFamilias.SelectedIndexChanged += listBoxFamilias_SelectedIndexChanged;
        }

        

        private bool ExisteConflicto_013AL(Componente_013AL permisoSeleccionado)
        {
            if (permisoSeleccionado is Permiso_013AL) 
            {
               
                if (EstaEnFamiliaDelRol_013AL(permisoSeleccionado.Cod_013AL, RolConfigurado))
                {
                    
                    return true;
                }

               
                Componente_013AL comp = bll.VerificarSiEstaEnFamilia_013AL(permisoSeleccionado.Cod_013AL);
                if (comp != null)
                {
                    Componente_013AL yaEstaEnElRol = RolConfigurado.ObtenerHijos_013AL().FirstOrDefault(p => p.Cod_013AL == comp.Cod_013AL);
                    if (yaEstaEnElRol != null)
                    {
                        MessageBox.Show($"El permiso ya está en la familia {comp.Cod_013AL} dentro del rol.");
                        return true;
                    }
                }
            }
            else 
            {
                
                List<Componente_013AL> listaHijos = bll.TraerListaHijos_013AL(permisoSeleccionado.Cod_013AL);
                foreach (var hijo in listaHijos)
                {
                    if (EstaEnFamiliaDelRol_013AL(hijo.Cod_013AL, RolConfigurado))
                    {
                        MessageBox.Show($"La familia seleccionada contiene el permiso {hijo.Cod_013AL}, que ya está dentro de otra familia en el rol.");
                        return true;
                    }

                    Componente_013AL yaEstaEnElRol = RolConfigurado.ObtenerHijos_013AL().FirstOrDefault(p => p.Cod_013AL == hijo.Cod_013AL);
                    if (yaEstaEnElRol != null)
                    {
                        MessageBox.Show($"La familia seleccionada tiene al permiso {hijo.Cod_013AL} ya seleccionado directamente en el rol.");
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
        private void ActualizarListBoxRol_013AL()
        {
            listBoxRol.Items.Clear();
            foreach (Componente_013AL permiso in RolConfigurado.ObtenerHijos_013AL())
            {
                listBoxRol.Items.Add($"{permiso.Cod_013AL} - {permiso.Nombre_013AL} - {permiso.Tipo_013AL}");
            }
        }
        private void ResetearBotones_013AL()
        {
            btnCrear.Enabled = true;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;

            cmbRoles.SelectedItem = null;
            
        }
        private void ActualizarListBoxPermisosYFamilias_013AL()
        {
            listBoxPermisos.Items.Clear();
            List<Componente_013AL> listaPermisos = bll.TraerListaPermisosHijo_013AL();
            foreach (var permiso in listaPermisos)
            {
                if (permiso is Permiso_013AL)
                {
                    listBoxPermisos.Items.Add($"{permiso.Cod_013AL} - {permiso.Nombre_013AL} - {permiso.Tipo_013AL}");
                }
            }


            listBoxFamilias.Items.Clear();
            List<Familia_013AL> listaFamilias = bll.TraerListaFamilias_013AL();
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
            List<Familia_013AL> listaRoles = bll.TraerListaRoles_013AL();
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
                Componente_013AL familiaSeleccionada = TraerComponeneteDeListBox_013AL(listBoxFamilias);
                List<Componente_013AL> lista = bll.TraerListaHijos_013AL(familiaSeleccionada.Cod_013AL);

                foreach (Componente_013AL permiso in lista)
                {
                    listBoxPermisoFamilia.Items.Add($"{permiso.Cod_013AL} - {permiso.Nombre_013AL} - {permiso.Tipo_013AL}");
                }
            }
        }

        private Componente_013AL TraerComponeneteDeListBox_013AL(ListBox listbox)
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
            Componente_013AL componenteSeleccionado;
            if (tipo == "Simple")
            {
                componenteSeleccionado = new Permiso_013AL { Cod_013AL = id, Nombre_013AL = nombre, Tipo_013AL = "Simple" };
            }
            else { componenteSeleccionado = new Familia_013AL { Cod_013AL = id, Nombre_013AL = nombre, Tipo_013AL = "Familia" }; }
            return componenteSeleccionado;
        }

       
        private void BloquearBotones()
        {
            btnCrear.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;

        }

        private void cmbRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem != null)
            {
                RolConfigurado.ObtenerHijos_013AL().Clear();
                listBoxRol.Items.Clear();
                string[] partes = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int id = int.Parse(partes[0].Trim());
                string nombre = partes[1].Trim();
                Familia_013AL rol = new Familia_013AL() { Cod_013AL = id, Nombre_013AL = nombre };
                List<Componente_013AL> lista = bll.TraerListaPermisosRol_013AL(rol.Cod_013AL);

                foreach (Componente_013AL permiso in lista)
                {
                    listBoxRol.Items.Add($"{permiso.Cod_013AL} - {permiso.Nombre_013AL} - {permiso.Tipo_013AL}");
                    RolConfigurado.AgregarHijo_013AL(permiso);
                }
            }
        }

        private void btnAgregarFamilia_Click(object sender, EventArgs e)
        {
            if (listBoxFamilias.SelectedItems.Count > 0)
            {
                Componente_013AL permisoSeleccionado = TraerComponeneteDeListBox_013AL(listBoxFamilias);
                if (!ExisteConflicto_013AL(permisoSeleccionado))
                {
                    RolConfigurado.AgregarHijo_013AL(permisoSeleccionado);
                    ActualizarListBoxRol_013AL();
                }
            }
            else { MessageBox.Show("Seleccione una familia para agregar"); }
        }

        private void btnQuitarPermiso_Click(object sender, EventArgs e)
        {
            if (listBoxRol.SelectedItems.Count > 0)
            {
                Componente_013AL permisoSeleccionado = TraerComponeneteDeListBox_013AL(listBoxRol);

                RolConfigurado.QuitarHijo_013AL(permisoSeleccionado);
                ActualizarListBoxRol_013AL();
            }
            else { MessageBox.Show("Seleccione un permiso del Rol configurado para quitar"); }
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem != null)
            {
                string[] partes = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int idRol = int.Parse(partes[0].Trim());

                foreach (var permiso in RolConfigurado.ObtenerHijos_013AL())
                {
                    
                    if (ExisteConflicto_013AL(permiso))
                    {
                        MessageBox.Show($"El permiso '{permiso.Nombre_013AL}' ya está contenido dentro de una familia del rol.");
                        continue; 
                    }

                    
                    bool existeEnPermisosFamilia = bll.VerificarPermisosRol_013AL(idRol, permiso.Cod_013AL);
                    if (existeEnPermisosFamilia)
                    {
                        MessageBox.Show($"El permiso '{permiso.Nombre_013AL}' ya está asignado al rol.");
                        continue; 
                    }

                   
                    bll.InsertarFamiliaRol_013AL(idRol, permiso.Cod_013AL);
                    MessageBox.Show("Permisos asignados a Rol correctamente");
                }

                
            }
            else
            {
                MessageBox.Show("Seleccione un rol antes de aplicar.");
            }
        }

        private void btnEliminarRelacion_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un rol antes de intentar eliminar un permiso o familia.");
                return;
            }

            if (listBoxRol.Items.Count == 0)
            {
                MessageBox.Show("Seleccione al menos un permiso o familia del rol configurado para eliminar.");
                return;
            }

            
            string[] partes = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int idRol = int.Parse(partes[0].Trim());

            
            List<Componente_013AL> permisosAEliminar = new List<Componente_013AL>();

            foreach (var item in listBoxRol.Items)
            {
                
                Console.WriteLine($"Tipo de item: {item.GetType().FullName}"); 
                if (item is Componente_013AL permiso)
                {
                    permisosAEliminar.Add(permiso);
                }
            }

            if (permisosAEliminar.Count == 0)
            {
                MessageBox.Show("No se pudo obtener la información de los permisos o familias seleccionados.");
                return;
            }

            
            bool algunEliminado = false;

            foreach (Componente_013AL permiso in permisosAEliminar)
            {
                
                if (bll.VerificarPermisosRol_013AL(idRol, permiso.Cod_013AL))
                {
                    bool eliminado = bll.VerificarPermisosRol_013AL(idRol, permiso.Cod_013AL);
                    if (eliminado)
                    {
                        RolConfigurado.QuitarHijo_013AL(permiso);
                        algunEliminado = true;
                    }
                }
            }

            
            if (algunEliminado)
            {
                ActualizarListBoxRol_013AL();
                MessageBox.Show("Permiso(s) o familia(s) eliminados correctamente.");
            }
            else
            {
                MessageBox.Show("No se pudo eliminar ningún permiso o familia. Verifique las relaciones.");
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

            if (bll.ExisteRol_013AL(nombreRol))
            {
                MessageBox.Show("El nombre del rol ya existe. Escriba un nombre diferente.");
                return;
            }

            int respuesta = bll.CrearRol_013AL(nombreRol);
            MessageBox.Show("Rol creado con éxito.");
            ActualizarComboBox_013AL();
            //int respuesta = bll.CrearRol(txtNombreRol.Text);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem != null)
            {
                string[] partes = cmbRoles.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                int idRol = int.Parse(partes[0].Trim());

                
                if (bll.RolTieneRelaciones_013AL(idRol))
                {
                    MessageBox.Show("No se puede eliminar el rol porque tiene permisos o familias asociadas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este rol?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resultado == DialogResult.Yes)
                {
                    bll.EliminarRol_013AL(idRol);
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

                string resultado = bll.ModificarRol_013AL(id, nuevoNombre);

                MessageBox.Show(resultado);

                ActualizarComboBox_013AL();
            }
            else
            {
                MessageBox.Show("Seleccione un rol para modificar.");
            }
        }
    }
}








/*private void btnGuardarPatente_Click(object sender, EventArgs e)
        {
            /*try
            {
            string respuesta = "";
            respuesta = bll.AgregarPatenteHijo(txtpatente.Text);
            CargarPatentesCombo();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }*/





/*
 public void CargarFamiliasCombo()
 {
     try 
     {
         List<Familia> fam = bll.ListarFamilias();
         cboFamilias.DisplayMember = "Nombre";
         cboFamilias.ValueMember = "Id";
         cboFamilias.DataSource = fam;

     }
     catch (Exception ex)
     {
         MessageBox.Show(ex.Message);
     }
 }

 public void CargarPatentesCombo()
 {
     try
     {
     List<Permiso> permisos = bll.ListarPermisos();
     cboPatentes.DisplayMember = "Nombre";
     cboPatentes.ValueMember = "Id";
     cboPatentes.DataSource = permisos;
     }
     catch (Exception ex)
     {
         MessageBox.Show(ex.Message);
     }

 }

 public void CargarRolesCombo()
 {
     try
     {
         List<Rol> rol = bll.ListarRoles();

         cborol.DisplayMember = "Nombre";
         cborol.ValueMember = "Id";
         cborol.DataSource = rol;
     }
     catch (Exception ex)
     {
         MessageBox.Show(ex.Message);
     }
 }

 private void cmdAgregarPatente_Click(object sender, EventArgs e)
 {
     try
     {
     TreeNode nodoPatentes = treeConfigurarFamilia.Nodes["Permiso"] ?? treeConfigurarFamilia.Nodes.Add("Permiso", "Permiso");
     Permiso patenteSeleccionada = (Permiso)cboPatentes.SelectedItem;

     if (cboPatentes.SelectedItem != null)
     {
         TreeNode nodoPatente = new TreeNode(patenteSeleccionada.Nombre)
         {
             Tag = patenteSeleccionada.Id // Guardar el Id en la propiedad Tag
         };
         if (!nodoPatentes.Nodes.Cast<TreeNode>().Any(n => (int)n.Tag == patenteSeleccionada.Id))
         {
             nodoPatentes.Nodes.Add(nodoPatente);
         }
     }
     }
     catch (Exception ex)
     {
         MessageBox.Show(ex.Message);
     }



 }

 private void cmdAgregarFamilia_Click(object sender, EventArgs e)
 {
     /*try
     {
     TreeNode nodoFamilias = treeConfigurarFamilia.Nodes["Familias"] ?? treeConfigurarFamilia.Nodes.Add("Familias", "Familias");
     Permiso patenteSeleccionada = (Permiso)cboFamilias.SelectedItem;
     treeConfigurarFamilia.Nodes.Add(patenteSeleccionada.Nombre);

     if (cboFamilias.SelectedItem != null)
     {

         TreeNode nodoFamilia = new TreeNode(patenteSeleccionada.Nombre)
         {
             Tag = patenteSeleccionada.Id // Guardar el Id en la propiedad Tag
         };
         if (!nodoFamilias.Nodes.Cast<TreeNode>().Any(n => (int)n.Tag == patenteSeleccionada.Id))
         {
             nodoFamilias.Nodes.Add(nodoFamilia);
         }
     }
     }
     catch (Exception ex)
     {
         MessageBox.Show(ex.Message);
     }

     /*TreeNode nodoFamilias = treeConfigurarFamilia.Nodes["Familias"] ?? treeConfigurarFamilia.Nodes.Add("Familias", "Familias");
     Permiso familiaSeleccionada = (Permiso)cboFamilias.SelectedItem;

     if (familiaSeleccionada != null)
     {
         TreeNode nodoFamilia = new TreeNode(familiaSeleccionada.Nombre)
         {
             Tag = familiaSeleccionada.Id // Guardar el Id en la propiedad Tag
         };

         if (!nodoFamilias.Nodes.Cast<TreeNode>().Any(n => (int)n.Tag == familiaSeleccionada.Id))
         {
             nodoFamilias.Nodes.Add(nodoFamilia);

             // Cargar los hijos (permisos) de la familia
             List<Permiso> hijos = bll.ListarHijos(familiaSeleccionada.Id);
             foreach (var hijo in hijos)
             {
                 TreeNode nodoHijo = new TreeNode(hijo.Nombre)
                 {
                     Tag = hijo.Id
                 };
                 nodoFamilia.Nodes.Add(nodoHijo);
             }
         }
     }
}

private void InitializeTreeView()
{
/*treeConfigurarFamilia.Nodes.Clear();

List<Permiso> familias = bll.ListarFamilias();
foreach (var familia in familias)
{
    TreeNode nodoFamilia = new TreeNode(familia.Nombre)
    {
        Tag = familia.Id
    };
    treeConfigurarFamilia.Nodes.Add(nodoFamilia);

    // Cargar los hijos (permisos) de la familia
    List<Permiso> hijos = bll.ListarHijos(familia.Id);
    foreach (var hijo in hijos)
    {
        TreeNode nodoHijo = new TreeNode(hijo.Nombre)
        {
            Tag = hijo.Id
        };
        nodoFamilia.Nodes.Add(nodoHijo);
    }
}
}

private void cmdGuardarFamilia_Click(object sender, EventArgs e)
{
string respuesta = "";
int idHijo = 0;
int idPadre = 0;

try
{
foreach (TreeNode node in treeConfigurarFamilia.Nodes)
{
    if (node.Text == "Permiso")
    {
        foreach (TreeNode childNode in node.Nodes)
        {
            idHijo = bll.BuscarId(childNode.Text);

        }
    }
    if (node.Text == "Familia")
    {
        foreach (TreeNode childNode in node.Nodes)
        {
            idPadre = bll.BuscarId(childNode.Text);

        }
    }
    /*if (node.Text == "Rol")
    {
        foreach (TreeNode childNode in node.Nodes)
        {
            idRol = bll.BuscarIdRol(childNode.Text);

        }
    }
}


    bool existeEnPermisosFamilia = bll.VerificarPermisosFamilia(idPadre, idHijo);

    if (existeEnPermisosFamilia)
    {
    MessageBox.Show("No se puede agregar porque ya existe esta relación.");
    }
 else   {

    Familia familia = new Familia();
            familia.Id = idPadre;
    Permiso permiso = new Permiso();
            permiso.Id = idHijo;
    respuesta = bll.InsertarFamiliaPatente(familia, permiso);
            string resultado;
            BLLBitacora bbll = new BLLBitacora();
            Usuarios user = SingletonSesion.Instance.GetUsuario();
            resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Agregar Permiso a Familia", 4);

     if (respuesta == "OK")
     {
     MessageBox.Show("Datos guardados correctamente.");
     }
     else
     {
     MessageBox.Show("Error al guardar: " + respuesta);
     }
        }            
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}

}



private void button3_Click_1(object sender, EventArgs e)
{
    try
    {
    string respuesta = "";
    Familia familiaSeleccionada = (Familia)cboFamilias.SelectedItem;
    bool existeEnPermisosComponente = bll.VerificarFamiliaEnPermisosComponente(familiaSeleccionada.Id);
    bool existeEnRolPermiso = bll.VerificarPermisosEnRolPermisos(Convert.ToInt32(familiaSeleccionada.Id));

    if (existeEnRolPermiso)
    {
        MessageBox.Show("No se puede eliminar el permiso porque está asociado con un rol.");
    }
    if (existeEnPermisosComponente)
    {
        MessageBox.Show("No se puede eliminar porque está asociado con un permiso.");
    }
    else
    {
        respuesta = bll.EliminarFamilia(Convert.ToInt32(familiaSeleccionada.Id));
            string resultado;
            BLLBitacora bbll = new BLLBitacora();
            Usuarios user = SingletonSesion.Instance.GetUsuario();
            resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Eliminar Familia", 4);
            MessageBox.Show("Exito");
        CargarFamiliasCombo();
    }
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }

}

private void button2_Click_1(object sender, EventArgs e)
{
try
{
TreeNode nodorol = treeView1.Nodes["Rol"] ?? treeView1.Nodes.Add("Rol", "Rol");
Rol rolseleccionado = (Rol)cborol.SelectedItem;
//treeConfigurarFamilia.Nodes.Add(rolseleccionado.Nombre);

if (cboPatentes.SelectedItem != null)
{

    TreeNode nodoPatente = new TreeNode(rolseleccionado.Nombre)
    {
        Tag = rolseleccionado.Id_Rol 
    };
    if (!nodorol.Nodes.Cast<TreeNode>().Any(n => (int)n.Tag == rolseleccionado.Id_Rol))
    {
        nodorol.Nodes.Add(nodoPatente);
    }
}
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}
}

private void button7_Click_1(object sender, EventArgs e)
{
     string respuesta = "";
     Rol rolSeleccionado = (Rol)cborol.SelectedItem;


     try
     {
     bool existeEnRolPermiso = bll.VerificarRolEnRolPermisos(Convert.ToInt32(rolSeleccionado.Id_Rol));
     bool existeEnUsuario = bll.VerificarRolEnUsuario(Convert.ToInt32(rolSeleccionado.Id_Rol));
     if (existeEnRolPermiso)
     {
         MessageBox.Show("No se puede eliminar el rol porque está asociado con una familia.");
     }
     if (existeEnUsuario)
     {
         MessageBox.Show("No se puede eliminar el rol porque está asociado con un usuario.");
     }
     else
     {
         respuesta = bll.EliminarRol(Convert.ToInt32(rolSeleccionado.Id_Rol));
            string resultado;
            BLLBitacora bbll = new BLLBitacora();
            Usuarios user = SingletonSesion.Instance.GetUsuario();
            resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Eliminar Rol", 4);
            MessageBox.Show("Rol eliminado con éxito.");
         CargarRolesCombo();
     }
     }
     catch (Exception ex)
     {
         MessageBox.Show(ex.Message);
     }
}

private void button4_Click_1(object sender, EventArgs e)
{
/*   try
{
string respuesta = "";
Componente patenteSeleccionada = (Componente)cboPatentes.SelectedItem;
bool existeEnPermisosFamilia = bll.VerificarPatenteEnPermisosFamilia(patenteSeleccionada.Id);
if (existeEnPermisosFamilia)
{
    MessageBox.Show("No se puede eliminar porque está asociado con una familia o patente.");
}
else
{
    respuesta = bll.EliminarFamilia(Convert.ToInt32(patenteSeleccionada.Id));
    MessageBox.Show("Exito");
    CargarPatentesCombo();
}
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}

}

private void button6_Click_1(object sender, EventArgs e)
{
/*  try
{
string respuesta = "";
Componente patenteSeleccionada = (Componente)cboPatentes.SelectedItem;
respuesta = bll.ModificarPatentes(Convert.ToInt32(patenteSeleccionada.Id), txtpatente.Text);
MessageBox.Show("Exito");
CargarPatentesCombo();
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}

}

private void cmdAgregarFamilia_Click_2(object sender, EventArgs e)
{
try
{
TreeNode nodoFamilias = treeConfigurarFamilia.Nodes["Familia"] ?? treeConfigurarFamilia.Nodes.Add("Familia", "Familia");
Familia familiaSeleccionada = (Familia)cboFamilias.SelectedItem;

if (cboFamilias.SelectedItem != null)
{
    TreeNode nodoFamilia = new TreeNode(familiaSeleccionada.Nombre)
    {
        Tag = familiaSeleccionada.Id // Guardar el Id en la propiedad Tag
    };
    if (!nodoFamilias.Nodes.Cast<TreeNode>().Any(n => (int)n.Tag == familiaSeleccionada.Id))
    {
        nodoFamilias.Nodes.Add(nodoFamilia);
    }
}
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}


}

private void button5_Click_1(object sender, EventArgs e)
{
try
{
string respuesta = "";
Familia familiaSeleccionada = (Familia)cboFamilias.SelectedItem;
respuesta = bll.ModificarPermisos(Convert.ToInt32(familiaSeleccionada.Id), txtNombreFamilia.Text);
        string resultado;
        BLLBitacora bbll = new BLLBitacora();
        Usuarios user = SingletonSesion.Instance.GetUsuario();
        resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Modificar Familia", 4);
        MessageBox.Show("Exito");
CargarFamiliasCombo();

}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}

}

private void button9_Click_1(object sender, EventArgs e)
{
string respuesta = "";
int idHijo = 0;
int idPadre = 0;
//int idRol = 0;
try {
foreach (TreeNode node in treeConfigurarFamilia.Nodes)
{
    if (node.Text == "Permiso")
    {
        foreach (TreeNode childNode in node.Nodes)
        {
            idHijo = bll.BuscarId(childNode.Text);



        }
    }
    if (node.Text == "Familia")
    {
        foreach (TreeNode childNode in node.Nodes)
        {
            idPadre = bll.BuscarId(childNode.Text);

        }
    }
    /*if (node.Text == "Rol")
    {
        foreach (TreeNode childNode in node.Nodes)
        {
            idRol = bll.BuscarIdRol(childNode.Text);

        }
    }


} }
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}

respuesta = bll.Eliminarpermisosfamilia(idPadre, idHijo);
    string resultado;
    BLLBitacora bbll = new BLLBitacora();
    Usuarios user = SingletonSesion.Instance.GetUsuario();
    resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Eliminar Permiso de Familia", 4);

    if (respuesta == "OK")
{
    MessageBox.Show("Datos eliminados correctamente.");
}
else
{
    MessageBox.Show("Error al eliminar: " + respuesta);
}
}

private void button1_Click_2(object sender, EventArgs e)
{
try
{
string respuesta = "";
respuesta = bll.AgregarRol(textBox1.Text);
        string resultado;
        BLLBitacora bbll = new BLLBitacora();
        Usuarios user = SingletonSesion.Instance.GetUsuario();
        resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Registrar Rol", 4);
        CargarRolesCombo();
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}



}

private void button8_Click_1(object sender, EventArgs e)
{
try
{
string respuesta = "";
Rol rolSeleccionado = (Rol)cborol.SelectedItem;
respuesta = bll.ModificarRol(Convert.ToInt32(rolSeleccionado.Id_Rol), textBox1.Text);
        string resultado;
        BLLBitacora bbll = new BLLBitacora();
        Usuarios user = SingletonSesion.Instance.GetUsuario();
        resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Modificar Rol", 4);
        MessageBox.Show("Exito");
CargarRolesCombo();
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}

}

private void btnguardarfamilia_Click(object sender, EventArgs e)
{
try
{
string respuesta = "";
respuesta = bll.AgregarFamiliaPadre(txtNombreFamilia.Text);

            string resultado;
            BLLBitacora bbll = new BLLBitacora();
            Usuarios user = SingletonSesion.Instance.GetUsuario();
            resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Registrar Familia", 4);

CargarFamiliasCombo();
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}

}

private void button10_Click(object sender, EventArgs e)
{
treeConfigurarFamilia.Nodes.Clear();
}

private void button12_Click(object sender, EventArgs e)
{
    try
    {
        TreeNode nodoFamilias = treeView1.Nodes["Familia"] ?? treeView1.Nodes.Add("Familia", "Familia");
        Familia familiaSeleccionada = (Familia)cboFamilias.SelectedItem;

        if (cboFamilias.SelectedItem != null)
        {
            TreeNode nodoFamilia = new TreeNode(familiaSeleccionada.Nombre)
            {
                Tag = familiaSeleccionada.Id // Guardar el Id en la propiedad Tag
            };
            if (!nodoFamilias.Nodes.Cast<TreeNode>().Any(n => (int)n.Tag == familiaSeleccionada.Id))
            {
                nodoFamilias.Nodes.Add(nodoFamilia);
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }

}

private void button11_Click(object sender, EventArgs e)
{
    string respuesta = "";

    int idFamilia = 0;
    int idRol = 0;
    try
    {
        foreach (TreeNode node in treeView1.Nodes)
        {
            if (node.Text == "Rol")
            {
                foreach (TreeNode childNode in node.Nodes)
                {
                    idRol = bll.BuscarIdRol(childNode.Text);

                }
            }
            if (node.Text == "Familia")
            {
                foreach (TreeNode childNode in node.Nodes)
                {
                    idFamilia = bll.BuscarId(childNode.Text);

                }
            }

        }


        bool existeEnPermisosFamilia = bll.VerificarPermisosRol(idRol, idFamilia);

        if (existeEnPermisosFamilia)
        {
            MessageBox.Show("No se puede agregar porque ya existe esta relación.");
        }
        else
        {
            Rol rol = new Rol();
            rol.Id_Rol = idRol;
            Familia familia = new Familia();
            familia.Id = idFamilia;

            respuesta = bll.InsertarFamiliaRol(rol, familia);
            string resultado;
            BLLBitacora bbll = new BLLBitacora();
            Usuarios user = SingletonSesion.Instance.GetUsuario();
            resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Agregar Familia a Rol", 4);
            if (respuesta == "OK")
            {
                MessageBox.Show("Datos guardados correctamente.");
            }
            else
            {
                MessageBox.Show("Error al guardar: " + respuesta);
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }
}

private void button6_Click(object sender, EventArgs e)
{
    string respuesta = "";

    int idFamilia = 0;
    int idRol = 0;
    try
    {
        foreach (TreeNode node in treeView1.Nodes)
        {
            if (node.Text == "Rol")
            {
                foreach (TreeNode childNode in node.Nodes)
                {
                    idRol = bll.BuscarIdRol(childNode.Text);

                }
            }
            if (node.Text == "Familia")
            {
                foreach (TreeNode childNode in node.Nodes)
                {
                    idFamilia = bll.BuscarId(childNode.Text);

                }
            }



        }
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
    }

    respuesta = bll.Eliminarfamiliarol(idRol, idFamilia);
    string resultado;
    BLLBitacora bbll = new BLLBitacora();
    Usuarios user = SingletonSesion.Instance.GetUsuario();
    resultado = bbll.AgregarEvento(user.NombreUsuario, "Gestión Perfiles", "Eliminar Familia de Rol", 4);

    if (respuesta == "OK")
    {
        MessageBox.Show("Datos eliminados correctamente.");
    }
    else
    {
        MessageBox.Show("Error al eliminar: " + respuesta);
    }
}

private void button4_Click(object sender, EventArgs e)
{
    treeView1.Nodes.Clear();
}*/













/*PermisosFamilia obj = new PermisosFamilia();
foreach (TreeNode nodoPrincipal in treeConfigurarFamilia.Nodes)
{
obj.IdFam = nodoPrincipal.Text; // "Patentes" o "Familias"

foreach (TreeNode nodoSecundario in nodoPrincipal.Nodes)
{

    obj.IdPat = nodoSecundario.Text;*/

/*List<Permiso> familias = bll.ListarFamilias();
List<Permiso> patentes = bll.ListarPatentes();
string respuesta = "";

foreach (TreeNode nodoPrincipal in treeConfigurarFamilia.Nodes)
{
    string tipo = nodoPrincipal.Text; // "Patentes" o "Familias"

    foreach (TreeNode nodoSecundario in nodoPrincipal.Nodes)
    {
        PermisosFamilia permisoFamilia = new PermisosFamilia();
        permisoFamilia.IdPat = (int)nodoSecundario.Tag; // Obtener el Id del nodo

        if (tipo == "Patentes")
        {
            // Guardar patente con una familia específica
            foreach (TreeNode nodoFamilia in treeConfigurarFamilia.Nodes["Familias"].Nodes)
            {
                int familiaId = (int)nodoFamilia.Tag;
                Familia familia = (Familia)familias.FirstOrDefault(f => f.Id == familiaId);
                Patente patente = (Patente)patentes.FirstOrDefault(p => p.Id == permisoFamilia.IdPat);

                if (familia != null && patente != null)
                {
                    permisoFamilia.IdFam = familia.Id;
                    respuesta = bll.AgregarPermisosFamilia(permisoFamilia);

                    if (respuesta != "OK")
                    {
                        MessageBox.Show("Error al guardar: " + respuesta);
                        return; // Detener si hay un error
                    }
                }
            }
        }
        else if (tipo == "Familias")
        {
            // Guardar familia con una patente específica
            foreach (TreeNode nodoPatente in treeConfigurarFamilia.Nodes["Patentes"].Nodes)
            {
                int patenteId = (int)nodoPatente.Tag;
                Familia familia = (Familia)familias.FirstOrDefault(f => f.Id == permisoFamilia.IdFam);
                Patente patente = (Patente)patentes.FirstOrDefault(p => p.Id == patenteId);

                if (familia != null && patente != null)
                {
                    permisoFamilia.IdPat = patente.Id;
                    respuesta = bll.AgregarPermisosFamilia(permisoFamilia);

                    if (respuesta != "OK")
                    {
                        MessageBox.Show("Error al guardar: " + respuesta);
                        return; // Detener si hay un error
                    }
                }
            }
        }
    }
}

MessageBox.Show("Datos guardados correctamente.");
}*/





