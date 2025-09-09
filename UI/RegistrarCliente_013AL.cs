using BE_013AL;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Controls;
using Servicios_013AL;
using System.Security.Cryptography;

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
        private void button1_Click(object sender, EventArgs e)
        {
            UsuarioBLL_013AL bll = new UsuarioBLL_013AL();
            DataTable dtclientes = dsclientes.Tables["Cliente"];

            string cuilnuevo = txtcuil.Text;

            // Verificar si el DNI ya existe en la tabla
            bool cuilExiste = dtclientes.AsEnumerable().Any(row => row["CUILCliente-013AL"].ToString() == cuilnuevo);

            if (cuilExiste)
            {
                MessageBox.Show("El CUIL ingresado ya pertenece a otro cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Detener la ejecución
            }


            DataRow newRow = dtclientes.NewRow();

            try
            {

                newRow["Nombre-013AL"] = txtnombre.Text;
                newRow["Apellido-013AL"] = txtapellido.Text;
                newRow["CUILCliente-013AL"] = txtcuil.Text;

                // Encriptar el domicilio antes de guardar
                newRow["Domicilio-013AL"] = AESCrypto_013AL.Encriptar_013AL(txtdomicilio.Text);

                newRow["Mail-013AL"] = txtmail.Text;
                newRow["Telefono-013AL"] = txttel.Text;

                dtclientes.Rows.Add(newRow);

                BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bbll.AgregarEvento_013AL(user.Login_013AL, "Clientes", "Registrar Cliente", 2);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




            /*UsuarioBLL bll = new UsuarioBLL();
            string respuesta = "";
            respuesta = bll.AgregarCliente(txtnombre.Text, txtapellido.Text, Convert.ToInt32(txtcuil.Text), txtdomicilio.Text, txtmail.Text, Convert.ToInt32(txttel.Text));
            try
            {
                string resultado;
                BLLBitacora bbll = new BLLBitacora();
                Usuarios user = SingletonSesion.Instance.GetUsuario();
                resultado = bbll.AgregarEvento(user.NombreUsuario, "Ventas", "Registrar Cliente", 2);
            }
            catch (Exception ex) { }*/
        }

        private void RegistrarCliente_Load(object sender, EventArgs e)
        {
            CargarClientes_013AL();
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

                    // Verificar si el nuevo DNI ya existe en otro usuario
                    bool cuilExiste = dsclientes.Tables["Cliente"].AsEnumerable()
                        .Any(r => r["CUILCliente-013AL"].ToString() == cuilNuevo && r != row.DataBoundItem);

                    if (cuilExiste)
                    {
                        MessageBox.Show("El CUIL ingresado ya pertenece a otro cliente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Detener la ejecución
                    }

                    row.Cells["Nombre-013AL"].Value = txtnombre;
                    row.Cells["Apellido-013AL"].Value = txtapellido.Text;
                    row.Cells["CUILCliente-013AL"].Value = txtcuil.Text;
                    row.Cells["Domicilio-013AL"].Value = AESCrypto_013AL.Encriptar_013AL(txtdomicilio.Text);
                    row.Cells["Mail-013AL"].Value = txtmail.Text;
                    row.Cells["Telefono-013AL"].Value = txttel.Text;
                    
                   
                    BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                    Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Clientes", "Modificar Cliente", 2);
                }
                else
                {
                    MessageBox.Show("Seleccione una fila para modificar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    dataGridView1.Rows.Remove(row);
                }
                BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bbll.AgregarEvento_013AL(user.Login_013AL, "Clientes", "Eliminar Cliente", 2);
            
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            try
            {
                // Guardar los usuarios en la base de datos
                UsuarioBLL_013AL bll = new UsuarioBLL_013AL();
                // Antes de guardar, encriptar todas las contraseñas en el DataSet
                /*foreach (DataRow row in dsUsuarios.Tables["Usuario"].Rows)
                {
                    string contraseñaEncriptada = HashHelper.CalcularSHA256(row["Contraseña"].ToString());
                    row["Contraseña"] = contraseñaEncriptada;
                }*/
                // Llamar al método de la BLL para guardar
                bll.GuardarClientes_013AL("Cliente", dsclientes);

                this.CargarClientes_013AL();
                MessageBox.Show("Datos actualizados correctamente");

                BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                bbll.AgregarEvento_013AL(user.Login_013AL, "Clientes", "Guardar Modificaciones", 2);
            
                }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar usuarios: " + ex.Message);
            }
        }

        private void CargarClientes_013AL()
        {
            UsuarioBLL_013AL usuarioBLL = new UsuarioBLL_013AL();
            dsclientes = usuarioBLL.ObtenerClientes_013AL();

            
            /*foreach (DataRow row in dsclientes.Tables["Cliente"].Rows)
            {
                if (!string.IsNullOrEmpty(row["Domicilio-013AL"].ToString()))
                {
                    row["Domicilio-013AL"] = AESCrypto_013AL.Desencriptar_013AL(row["Domicilio-013AL"].ToString());
                }
            }*/

            dataGridView1.DataSource = dsclientes.Tables["Cliente"];

            /*int contador = dataGridView1.Rows.Count - 1;
            label11.Text = contador.ToString();*/

            /*UsuarioBLL usuarioBLL = new UsuarioBLL();
            //dsUsuarios = new DataSet();
            dsUsuarios = usuarioBLL.ObtenerUsuarios();
            dataGridView1.DataSource = dsUsuarios.Tables["Usuario"];
            int contador;
            contador = (dataGridView1.Rows.Count) - 1;
            label11.Text = Convert.ToString(contador);*/
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.CargarClientes_013AL();
            BLLBitacora_013AL bbll = new BLLBitacora_013AL();
            Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
            bbll.AgregarEvento_013AL(user.Login_013AL, "Clientes", "Cancelar Cambios", 2);
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
                        // Intentamos desencriptar el domicilio. Si lo logramos, mostramos el dato desencriptado
                        string domicilioDesencriptado = AESCrypto_013AL.Desencriptar_013AL(domicilio);
                        MessageBox.Show("Domicilio desencriptado.");
                        fila.Cells["Domicilio-013AL"].Value = domicilioDesencriptado;
                    }
                    catch (CryptographicException)
                    {
                        // Si da error, probablemente ya esté desencriptado, así que lo encriptamos
                        string domicilioEncriptadoNuevo = AESCrypto_013AL.Encriptar_013AL(domicilio);
                        MessageBox.Show("Domicilio encriptado.");
                        fila.Cells["Domicilio-013AL"].Value = domicilioEncriptadoNuevo;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error inesperado: " + ex.Message);
                    }


                    // Opción: actualizar también en el DataSet original si querés que quede persistido en memoria
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
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Limpiar ListBox de datos previos
                    listBox2.Items.Clear();

                    // Crear el objeto Usuario a partir de la fila seleccionada
                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    Cliente_013AL cliente = new Cliente_013AL
                    {
                        //IDCliente = Convert.ToInt32(row.Cells["IdCliente"].Value.ToString()),
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
                        BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                        Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bbll.AgregarEvento_013AL(user.Login_013AL, "Clientes", "Serialización", 3);
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
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                //openFileDialog1.Filter = $"{cmbFormato.SelectedItem} Files|*.{cmbFormato.SelectedItem.ToString().ToLower()}";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string formato = "XML";
                    Cliente_013AL cliente = null;

                    listBox1.Items.Clear();


                    cliente = DeserializarXML_013AL(openFileDialog1.FileName);


                    if (cliente != null)
                    {
                        MostrarDatosDeserializados_013AL(cliente);
                        BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                        Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                        bbll.AgregarEvento_013AL(user.Login_013AL, "Clientes", "Deserializar", 3);
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
            //listBox2.Items.Add($"IdCliente: {cliente.IDCliente}");
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
