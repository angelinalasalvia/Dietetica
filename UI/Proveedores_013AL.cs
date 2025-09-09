using BE_013AL;
using BLL_013AL;
using Servicios_013AL;
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

namespace UI
{
    public partial class Proveedores_013AL : Form, IObserver_013AL
    {
        NegocioBLL_013AL bll = new NegocioBLL_013AL();

        public Proveedores_013AL()
        {
            InitializeComponent();

            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
                        
            CargarProveedores_013AL();

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
        private void CargarProveedores_013AL()
        {
            try
            {
                DataTable dtProveedores = bll.ListarProveedoresDGV_013AL();
                if (dtProveedores != null && dtProveedores.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dtProveedores;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AllowUserToDeleteRows = false;
                    dataGridView1.ReadOnly = false;
                }
                else
                {
                    MessageBox.Show("No hay proveedores para mostrar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView1.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los proveedores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Activar el botón para guardar cambios al terminar de editar una celda
            //btnGuardarCambios.Enabled = true;
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                    int cuitSeleccionado = Convert.ToInt32(filaSeleccionada.Cells["CUIT"].Value);

                    string respuesta = bll.EliminarProveedor_013AL(cuitSeleccionado);

                    if (respuesta == "OK")
                    {
                        MessageBox.Show("Proveedor eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarProveedores_013AL(); // Recargar la lista después de eliminar

                        BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                        Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bbll.AgregarEvento_013AL(user.Login_013AL, "Proveedores", "Eliminar Proveedor", 2);
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el proveedor: " + respuesta, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, selecciona un proveedor de la lista.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al intentar eliminar el proveedor: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Proveedores_Load(object sender, EventArgs e)
        {
            CargarProveedores_013AL();
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];

                txtcuit.Text = filaSeleccionada.Cells["CUIT-013AL"].Value?.ToString();
                txtnombre.Text = filaSeleccionada.Cells["NombreProveedor-013AL"].Value?.ToString();
                txtapellido.Text = filaSeleccionada.Cells["ApellidoProveedor-013AL"].Value?.ToString();
                txtdomicilio.Text = filaSeleccionada.Cells["Domicilio-013AL"].Value?.ToString();
                txtmail.Text = filaSeleccionada.Cells["Mail-013AL"].Value?.ToString();
                txtrazonsocial.Text = filaSeleccionada.Cells["RazonSocial-013AL"].Value?.ToString();
                txttel.Text = filaSeleccionada.Cells["Telefono-013AL"].Value?.ToString();
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

              

                string respuesta = bll.ModificarProveedor2_013AL(
                    
                    Convert.ToInt32(txtcuit.Text),
                    txtapellido.Text,
                    txtnombre.Text,
                     txtdomicilio.Text,
                    
                    txtmail.Text,
                    txtrazonsocial.Text,
                    Convert.ToInt32(txttel.Text)
                   

                );

                MessageBox.Show(respuesta, "Resultado de la operación", MessageBoxButtons.OK, MessageBoxIcon.Information);


                CargarProveedores_013AL();

            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor de la lista.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];

                


                string respuesta = bll.ModificarProveedor_013AL(
                    Convert.ToInt32(txtcuit.Text),
                    txtapellido.Text,

                     txtdomicilio.Text,
                     
                    txtmail.Text,
                    
                    Convert.ToInt32(txttel.Text)

                );

                MessageBox.Show("Exito");

                CargarProveedores_013AL();

            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor de la lista.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

       
    }
}




