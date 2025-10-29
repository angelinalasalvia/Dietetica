using BE_013AL;
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
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using File = System.IO.File;

namespace UI
{
    public partial class RegistrarCliente_013AL : Form, IObserver_013AL
    {
        public RegistrarCliente_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }
        DataSet dsclientes;
        public void ActualizarIdioma_013AL()
        {
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }

        EventoBLL_013AL bbll = new EventoBLL_013AL();
        ClienteBLL_013AL cbll = new ClienteBLL_013AL();
        Usuarios_013AL user;
        private List<string> eventosPendientes = new List<string>();

        private void button1_Click(object sender, EventArgs e)
        {
            
            DataTable dtclientes = dsclientes.Tables["Cliente"];

            string cuilnuevo = txtcuil.Text;

            
            bool cuilExiste = dtclientes.AsEnumerable().Any(row => row["CUILCliente-013AL"].ToString() == cuilnuevo);

            if (cuilExiste)
            {
                MessageBox.Show("El CUIL ingresado ya pertenece a otro cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", "Error al crear cliente. CUIL ya existe.", 1);
                return; 
            }


            DataRow newRow = dtclientes.NewRow();

            try
            {

                newRow["Nombre-013AL"] = txtnombre.Text;
                newRow["Apellido-013AL"] = txtapellido.Text;
                newRow["CUILCliente-013AL"] = txtcuil.Text;

                newRow["Domicilio-013AL"] = AESCrypto_013AL.Encriptar_013AL(txtdomicilio.Text);

                newRow["Mail-013AL"] = txtmail.Text;
                newRow["Telefono-013AL"] = txttel.Text;

                dtclientes.Rows.Add(newRow);

                eventosPendientes.Add($"Cliente {Convert.ToString(newRow["Nombre-013AL"])} {Convert.ToString(newRow["Apellido-013AL"])} registrado");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                eventosPendientes.Add(ex.Message);
            }

        }


        private void RegistrarCliente_Load(object sender, EventArgs e)
        {
            CargarClientes_013AL();
            eventosPendientes.Clear();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow row = dataGridView1.SelectedRows[0];

                    string cuilNuevo = txtcuil.Text;
                    string cuilActual = row.Cells["CUILCliente-013AL"].Value.ToString();

                    bool cuilExiste = dsclientes.Tables["Cliente"].AsEnumerable()
                        .Any(r => r["CUILCliente-013AL"].ToString() == cuilNuevo && r != row.DataBoundItem);

                    if (cuilExiste)
                    {
                        MessageBox.Show("El CUIL ingresado ya pertenece a otro cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; 
                    }

                    row.Cells["Nombre-013AL"].Value = txtnombre;
                    row.Cells["Apellido-013AL"].Value = txtapellido.Text;
                    row.Cells["CUILCliente-013AL"].Value = txtcuil.Text;
                    row.Cells["Domicilio-013AL"].Value = AESCrypto_013AL.Encriptar_013AL(txtdomicilio.Text);
                    row.Cells["Mail-013AL"].Value = txtmail.Text;
                    row.Cells["Telefono-013AL"].Value = txttel.Text;


                    eventosPendientes.Add($"Cliente {row.Cells["Nombre-013AL"].Value} {row.Cells["Apellido-013AL"].Value} modificado");
                }
                else
                {
                    MessageBox.Show("Seleccione una fila para modificar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                eventosPendientes.Add(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    eventosPendientes.Add($"Cliente {row.Cells["Nombre-013AL"].Value} {row.Cells["Apellido-013AL"].Value} eliminado");

                    dataGridView1.Rows.Remove(row);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                eventosPendientes.Add(ex.Message);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            try
            {
                
                cbll.GuardarClientes_013AL("Cliente", dsclientes);

                this.CargarClientes_013AL();
                MessageBox.Show("Datos actualizados correctamente");


                user = SingletonSession_013AL.Instance.GetUsuario_013AL();

                foreach (var evento in eventosPendientes)
                {
                bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", evento, 2);
                }
                eventosPendientes.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar usuarios: " + ex.Message);
                user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", ex.Message, 2);
            }
        }

        private void CargarClientes_013AL()
        {
            
            dsclientes = cbll.ObtenerClientes_013AL();
            dataGridView1.DataSource = dsclientes.Tables["Cliente"];            
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.CargarClientes_013AL();
        }
        private bool domicilioEncriptado = false;
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow fila = dataGridView1.SelectedRows[0];
                    string domicilio = fila.Cells["Domicilio-013AL"].Value?.ToString();

                    if (string.IsNullOrEmpty(domicilio))
                    {
                        MessageBox.Show("No hay dirección para encriptar/desencriptar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    try
                    {
                        string domicilioDesencriptado = AESCrypto_013AL.Desencriptar_013AL(domicilio);
                        MessageBox.Show("Domicilio desencriptado.");
                        fila.Cells["Domicilio-013AL"].Value = domicilioDesencriptado;

                        bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", $"Domicilio del cliente {fila.Cells["Nombre-013AL"].Value} {fila.Cells["Apellido-013AL"].Value} desencriptado", 3);
                    }
                    catch (CryptographicException)
                    {
                        string domicilioEncriptadoNuevo = AESCrypto_013AL.Encriptar_013AL(domicilio);
                        MessageBox.Show("Domicilio encriptado.");
                        fila.Cells["Domicilio-013AL"].Value = domicilioEncriptadoNuevo;

                        bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", $"Domicilio del cliente {fila.Cells["Nombre-013AL"].Value} {fila.Cells["Apellido-013AL"].Value} encriptado", 3);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error inesperado: " + ex.Message);
                        bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", ex.Message, 3);
                    }

                    if (fila.Cells["Domicilio-013AL"].Value == null)
                    {
                        fila.Cells["Domicilio-013AL"].Value = domicilio;
                    }

                }
                else
                {
                    MessageBox.Show("Seleccione un cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el proceso de encriptación/desencriptación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", ex.Message, 3);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    listBox2.Items.Clear();

                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    Cliente_013AL cliente = new Cliente_013AL
                    {
                        Nombre_013AL = row.Cells["Nombre-013AL"].Value.ToString(),
                        Apellido_013AL = row.Cells["Apellido-013AL"].Value.ToString(),
                        CUIL_013AL = Convert.ToInt32(row.Cells["CUILCliente-013AL"].Value.ToString()),
                        Domicilio_013AL = row.Cells["Domicilio-013AL"].Value.ToString(),
                        Mail_013AL = row.Cells["Mail-013AL"].Value.ToString(),
                        Telefono_013AL = Convert.ToInt32(row.Cells["Telefono-013AL"].Value.ToString())
                       
                    };


                    string formato = "XML";
                    saveFileDialog1.Filter = $"{formato} Files|*.{formato.ToLower()}";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {

                        SerializarXML_013AL(cliente, saveFileDialog1.FileName);
                        MostrarArchivoSerializado_013AL(saveFileDialog1.FileName);

                        MessageBox.Show($"Usuario serializado en formato {formato} con éxito.");
                        
                        user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", $"Cliente {cliente.Nombre_013AL} {cliente.Apellido_013AL} serializado a XML", 2);
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un usuario para serializar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al serializar: " + ex.Message);
                bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", ex.Message, 2);

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string formato = "XML";
                    Cliente_013AL cliente = null;

                    listBox1.Items.Clear();


                    cliente = DeserializarXML_013AL(openFileDialog1.FileName);


                    if (cliente != null)
                    {
                        MostrarDatosDeserializados_013AL(cliente);
                        user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bbll.AgregarEvento_013AL(user.Login_013AL, "Registrar Cliente", $"Cliente {cliente.Nombre_013AL} {cliente.Apellido_013AL} deserializado desde XML", 2);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al deserializar: " + ex.Message);
            }
        }
        private void SerializarXML_013AL(Cliente_013AL cliente, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Cliente_013AL));
                serializer.Serialize(fs, cliente);
            }
        }
        private Cliente_013AL DeserializarXML_013AL(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Cliente_013AL));
                return (Cliente_013AL)serializer.Deserialize(fs);
            }
        }
        private void MostrarArchivoSerializado_013AL(string path)
        {
            listBox1.Items.Clear();
            string[] lineas = File.ReadAllLines(path);
            foreach (string linea in lineas)
            {
                listBox1.Items.Add(linea);
            }
        }

         
        private void MostrarDatosDeserializados_013AL(Cliente_013AL cliente)
        {
            listBox2.Items.Clear();
            listBox2.Items.Add($"Nombre_013AL: {cliente.Nombre_013AL}");
            listBox2.Items.Add($"Apellido_013AL: {cliente.Apellido_013AL}");
            listBox2.Items.Add($"CUIL_013AL: {cliente.CUIL_013AL}");
            listBox2.Items.Add($"Domicilio_013AL: {cliente.Domicilio_013AL}");
            listBox2.Items.Add($"Mail_013AL: {cliente.Mail_013AL}");
            listBox2.Items.Add($"Telefono_013AL: {cliente.Telefono_013AL}");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
        }
               
    }
}
