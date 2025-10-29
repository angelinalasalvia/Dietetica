using BE_013AL;
using BE_013AL.Composite;
using BLL;
using BLL_013AL;
using Servicios_013AL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace UI
{
    public partial class GestorUsuarios_013AL : Form, IObserver_013AL
    {
        DataSet dsUsuariosCopia;
        DataSet dsUsuariosBD;

        public GestorUsuarios_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
            CargarRolesCombo_013AL();
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
                
        EventoBLL_013AL bbll = new EventoBLL_013AL();
        Usuarios_013AL user;
        private List<string> eventosPendientes = new List<string>();

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    string dniSeleccionado = selectedRow.Cells["DNI-013AL"].Value.ToString();

                    DataTable dtUsuarios = dsUsuariosCopia.Tables["Usuario"];
                    DataRow rowToUpdate = dtUsuarios.AsEnumerable()
                        .FirstOrDefault(row => row["DNI-013AL"].ToString() == dniSeleccionado);

                    if (rowToUpdate == null)
                    {
                        MessageBox.Show("No se encontró el usuario en la tabla.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    bool yaEliminado = Convert.ToBoolean(rowToUpdate["Eliminado-013AL"]);
                    if (yaEliminado)
                    {
                        MessageBox.Show("El usuario ya está marcado como eliminado.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        try
                        {
                            eventosPendientes.Add($"Usuario ya eliminado.");
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                        return;
                    }

                    rowToUpdate["Eliminado-013AL"] = true;

                    MessageBox.Show("Presione 'Guardar' para confirmar la eliminación lógica del usuario.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    eventosPendientes.Add($"Usuario {Convert.ToString(rowToUpdate["Login-013AL"])} desbloqueado");

                    bool mostrarActivos = radioButton1.Checked;
                    if (mostrarActivos)
                    {
                        radioButton1.Checked = true;
                    }
                    else
                    {
                        radioButton2.Checked = true;
                    }

                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un usuario para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                eventosPendientes.Add(ex.Message);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            UsuarioBLL_013AL bll = new UsuarioBLL_013AL();
            DataTable dtUsuarios = dsUsuariosCopia.Tables["Usuario"];

            string dniNuevo = textBox6.Text;

            bool dniExiste = dtUsuarios.AsEnumerable().Any(row => row["DNI-013AL"].ToString() == dniNuevo);

            if (dniExiste)
            {
                MessageBox.Show("El DNI ingresado ya pertenece a otro usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                try
                {
                    eventosPendientes.Add("DNI ya pertenece a un usuario");
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return; 
            }
            if (!EsEmailValido_013AL(textBox1.Text))
            {
                MessageBox.Show("El correo electrónico debe tener el formato correcto y terminar en '.com'.", "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                try
                {
                    eventosPendientes.Add($"Formato incorrecto de mail.");
                }
                catch (Exception ex) { Console.WriteLine(ex); }
                return;
            }

            DataRow newRow = dtUsuarios.NewRow();

            try
            {
                newRow["Mail-013AL"] = textBox1.Text;
                string inputPassword = textBox2.Text;
                string hashedInputPassword = HashHelper_013AL.CalcularSHA256_013AL(inputPassword);
                newRow["Contraseña-013AL"] = hashedInputPassword;
                newRow["Nombres-013AL"] = textBox3.Text;
                newRow["Apellidos-013AL"] = textBox4.Text;
                newRow["CodRol-013AL"] = ((KeyValuePair<int, string>)cborol.SelectedItem).Key;
                newRow["DNI-013AL"] = dniNuevo;
                newRow["Login-013AL"] = textBox7.Text;

                newRow["Bloqueo-013AL"] = checkBox1.Checked;
                newRow["Activo-013AL"] = checkBox2.Checked;

                dtUsuarios.Rows.Add(newRow);

                MessageBox.Show("El usuario se agregó con éxito. Presione 'Guardar' para guardar definitivamente los cambios.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                eventosPendientes.Add($"Se agregó el usuario {Convert.ToString(newRow["Login-013AL"])}");

                bool mostrarActivos = radioButton1.Checked;
                if (mostrarActivos)
                {
                    radioButton1.Checked = true;
                }
                else
                {
                    radioButton2.Checked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                eventosPendientes.Add(ex.Message);
            }
        }


        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                UsuarioBLL_013AL usuarioBLL = new UsuarioBLL_013AL();
                dsUsuariosBD = usuarioBLL.ObtenerUsuarios_013AL();
                dsUsuariosCopia = dsUsuariosBD.Copy();

                Dictionary<int, string> roles = rbll.ListarRoles_013AL();
                AgregarNombresDeRol(dsUsuariosBD, roles);


                if (dataGridView1.Columns["Eliminado-013AL"] != null)
                {
                    dataGridView1.Columns["Eliminado-013AL"].Visible = true;
                }

                int contador = dataGridView1.Rows.Count;
                if (dataGridView1.AllowUserToAddRows)
                    contador--;
                txtNumUsers.Text = contador.ToString();

                radioButton2.Checked = true;
            }
        }


        //REVISAR
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {            
            try
            {
                UsuarioBLL_013AL bll = new UsuarioBLL_013AL();
                dsUsuariosBD = bll.ListarUsuariosActivos_013AL();
                Dictionary<int, string> roles = rbll.ListarRoles_013AL();
                AgregarNombresDeRol(dsUsuariosBD, roles);


                dataGridView1.DataSource = dsUsuariosBD.Tables["Usuario"];      
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            int contador = dataGridView1.Rows.Count;
            if (dataGridView1.AllowUserToAddRows)
                contador--;
            txtNumUsers.Text = contador.ToString();
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    string dniSeleccionado = dataGridView1.SelectedRows[0].Cells["DNI-013AL"].Value.ToString();
                    DataTable dtUsuarios = dsUsuariosCopia.Tables["Usuario"];
                    DataRow usuarioRow = dtUsuarios.AsEnumerable().FirstOrDefault(row => row["DNI-013AL"].ToString() == dniSeleccionado);
                    if (usuarioRow != null)
                    {
                        bool estaBloqueado = Convert.ToBoolean(usuarioRow["Bloqueo-013AL"]);

                        if (estaBloqueado)
                        {
                            usuarioRow["Bloqueo-013AL"] = false;

                            eventosPendientes.Add($"Usuario {Convert.ToString(usuarioRow["Login-013AL"])} desbloqueado");

                            MessageBox.Show("El usuario fue desbloqueado con éxito. Presione 'Guardar' para aplicar los cambios definitivamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bool mostrarActivos = radioButton1.Checked;
                            if (mostrarActivos)
                            {
                                radioButton1.Checked = true;
                            }
                            else
                            {
                                radioButton2.Checked = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("El usuario seleccionado no está bloqueado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el usuario en la tabla del DataSet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un usuario para desbloquear.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                eventosPendientes.Add(ex.Message);

            }
        }

        private void GestorUsuarios_Load(object sender, EventArgs e)
        {
            
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView1.AllowUserToAddRows = false;
            
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            radioButton1.Checked = true;

            CargarUsuarios_013AL();

            dataGridView1.RowPrePaint += dataGridView1_RowPrePaint;

            eventosPendientes.Clear();

        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;

            if (!dgv.Columns.Contains("Activo-013AL")) return;

            var row = dgv.Rows[e.RowIndex];
            if (row.Cells["Activo-013AL"].Value != null &&
                bool.TryParse(row.Cells["Activo-013AL"].Value.ToString(), out bool activo))
            {
                if (!activo) 
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.White; 
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void CargarUsuarios_013AL()
        {
            UsuarioBLL_013AL usuarioBLL = new UsuarioBLL_013AL();
            dsUsuariosBD = usuarioBLL.ObtenerUsuarios_013AL();
            dsUsuariosCopia = dsUsuariosBD.Copy();

            Dictionary<int, string> roles = rbll.ListarRoles_013AL();
            AgregarNombresDeRol(dsUsuariosBD, roles);


            if (dataGridView1.Columns["Eliminado-013AL"] != null)
            {
                dataGridView1.Columns["Eliminado-013AL"].Visible = true;
            }

            int contador = dataGridView1.Rows.Count;
            if (dataGridView1.AllowUserToAddRows)
                contador--;
            txtNumUsers.Text = contador.ToString();

            radioButton2.Checked = true;
        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                bool mostrarActivos = radioButton1.Checked;

                dataGridView1.EndEdit();
                dataGridView1.BindingContext[dataGridView1.DataSource].EndCurrentEdit();
                
                UsuarioBLL_013AL bll = new UsuarioBLL_013AL();
                
                bll.GuardarUsuarios_013AL("Usuario", dsUsuariosCopia);

                this.CargarUsuarios_013AL();

                user = SingletonSession_013AL.Instance.GetUsuario_013AL();

                foreach (var evento in eventosPendientes)
                {
                    bbll.AgregarEvento_013AL(user.Login_013AL, "Gestión Usuarios", evento, 3);
                }

                eventosPendientes.Clear();

                MessageBox.Show("Datos actualizados correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar usuarios: " + ex.Message);
                bbll.AgregarEvento_013AL(user.Login_013AL, "Gestión Usuarios", ex.Message, 3);

            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                    {
                        string dniSeleccionado = selectedRow.Cells["DNI-013AL"].Value.ToString();

                        DataTable dtUsuarios = dsUsuariosCopia.Tables["Usuario"];
                        DataRow fila = dtUsuarios.AsEnumerable()
                            .FirstOrDefault(row => row["DNI-013AL"].ToString() == dniSeleccionado);

                        if (fila != null)
                        {
                            fila.Delete(); 
                        }
                        var login = selectedRow.Cells["Login-013AL"].Value?.ToString();
                        eventosPendientes.Add($"Usuario {login} fue eliminado");
                    }

                    MessageBox.Show("El usuario fue marcado para eliminación. Presione 'Guardar' para aplicar los cambios definitivamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bool mostrarActivos = radioButton1.Checked;
                    if (mostrarActivos)
                    {
                        radioButton1.Checked = true;
                    }
                    else
                    {
                        radioButton2.Checked = true;
                    }

                }
                else
                {
                    MessageBox.Show("Seleccione al menos un usuario para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                eventosPendientes.Add(ex.Message);

            }
        }

        RolBLL_013AL rbll = new RolBLL_013AL();

        public void CargarRolesCombo_013AL()
        {
            var roles = rbll.ListarRoles_013AL();

            cborol.DataSource = new BindingSource(roles, null);
            cborol.DisplayMember = "Value"; 
            cborol.ValueMember = "Key";     
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    string dniSeleccionado = selectedRow.Cells["DNI-013AL"].Value.ToString();

                    DataTable dtUsuarios = dsUsuariosCopia.Tables["Usuario"];
                    DataRow rowToModify = dtUsuarios.AsEnumerable()
                        .FirstOrDefault(row => row["DNI-013AL"].ToString() == dniSeleccionado);

                    if (rowToModify == null)
                    {
                        MessageBox.Show("No se encontró el usuario en la tabla para modificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string dniNuevo = textBox6.Text;

                    if (dniNuevo != dniSeleccionado)
                    {
                        bool dniExiste = dtUsuarios.AsEnumerable()
                            .Any(row => row["DNI-013AL"].ToString() == dniNuevo);

                        if (dniExiste)
                        {
                            MessageBox.Show("El DNI ingresado ya pertenece a otro usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (!EsEmailValido_013AL(textBox1.Text))
                    {
                        MessageBox.Show("El correo electrónico debe tener el formato correcto y terminar en '.com'.", "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    rowToModify["Mail-013AL"] = textBox1.Text;
                    rowToModify["Nombres-013AL"] = textBox3.Text;
                    rowToModify["Apellidos-013AL"] = textBox4.Text;
                    int codRolSeleccionado = ((KeyValuePair<int, string>)cborol.SelectedItem).Key;
                    rowToModify["CodRol-013AL"] = codRolSeleccionado;
                    rowToModify["Login-013AL"] = textBox7.Text;
                    rowToModify["Bloqueo-013AL"] = checkBox1.Checked;
                    rowToModify["Activo-013AL"] = checkBox2.Checked;


                    eventosPendientes.Add($"Usuario {Convert.ToString(rowToModify["Login-013AL"])} modificado.");

                    MessageBox.Show("El usuario se modificó con éxito. Presione 'Guardar' para guardar definitivamente los cambios.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bool mostrarActivos = radioButton1.Checked;
                    if (mostrarActivos)
                    {
                        radioButton1.Checked = true;
                    }
                    else
                    {
                        radioButton2.Checked = true;
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un usuario para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                eventosPendientes.Add(ex.Message);

            }

        }
        private void AgregarNombresDeRol(DataSet ds, Dictionary<int, string> roles)
        {
            if (!ds.Tables["Usuario"].Columns.Contains("NombreRol-013AL"))
            {
                ds.Tables["Usuario"].Columns.Add("NombreRol-013AL", typeof(string));
            }

            foreach (DataRow row in ds.Tables["Usuario"].Rows)
            {
                int idRol = Convert.ToInt32(row["CodRol-013AL"]);
                if (roles.ContainsKey(idRol))
                {
                    row["NombreRol-013AL"] = roles[idRol];
                }
            }

            dataGridView1.DataSource = dsUsuariosBD.Tables["Usuario"];
            if (dataGridView1.Columns["CodRol-013AL"] != null)
            {
                dataGridView1.Columns["CodRol-013AL"].Visible = false;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.CargarUsuarios_013AL();
            eventosPendientes.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                    string dniSeleccionado = selectedRow.Cells["DNI-013AL"].Value.ToString();

                    DataTable dtUsuarios = dsUsuariosCopia.Tables["Usuario"];
                    DataRow rowToUpdate = dtUsuarios.AsEnumerable()
                        .FirstOrDefault(row => row["DNI-013AL"].ToString() == dniSeleccionado);

                    if (rowToUpdate == null)
                    {
                        MessageBox.Show("No se encontró el usuario en la tabla.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    bool estaActivo = Convert.ToBoolean(rowToUpdate["Activo-013AL"]);

                    if (estaActivo)
                    {
                        rowToUpdate["Activo-013AL"] = false;
                        eventosPendientes.Add($"Usuario {Convert.ToString(rowToUpdate["Login-013AL"])} desactivado.");
                        MessageBox.Show("Usuario desactivado exitosamente. Presione 'Guardar' para confirmar este cambio definitivamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        rowToUpdate["Activo-013AL"] = true;
                        eventosPendientes.Add($"Usuario {Convert.ToString(rowToUpdate["Login-013AL"])} activado.");
                        MessageBox.Show("Usuario activado exitosamente.Presione 'Guardar' para confirmar este cambio definitivamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }


                    bool mostrarActivos = radioButton1.Checked;
                    if (mostrarActivos)
                    {
                        radioButton1.Checked = true;
                    }
                    else
                    {
                        radioButton2.Checked = true;
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un usuario para activar o desactivar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                eventosPendientes.Add(ex.Message);

            }


        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];

                textBox1.Text = row.Cells["Mail-013AL"].Value?.ToString();
                textBox2.Text = ""; 
                textBox3.Text = row.Cells["Nombres-013AL"].Value?.ToString();
                textBox4.Text = row.Cells["Apellidos-013AL"].Value?.ToString();
                textBox6.Text = row.Cells["DNI-013AL"].Value?.ToString();
                textBox7.Text = row.Cells["Login-013AL"].Value?.ToString();

                checkBox1.Checked = Convert.ToBoolean(row.Cells["Bloqueo-013AL"].Value);
                checkBox2.Checked = Convert.ToBoolean(row.Cells["Activo-013AL"].Value);

                int codRol = Convert.ToInt32(row.Cells["CodRol-013AL"].Value);

                foreach (KeyValuePair<int, string> item in (BindingSource)cborol.DataSource)
                {
                    if (item.Key == codRol)
                    {
                        cborol.SelectedItem = item;
                        break;
                    }
                }
                textBox2.ReadOnly = true;
                textBox6.ReadOnly = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            cborol.Text = "";
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            dataGridView1.ClearSelection();
            textBox2.ReadOnly = false;
            textBox6.ReadOnly = false;
        }

        private bool EsEmailValido_013AL(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.(com)$");
        }

    }
}
