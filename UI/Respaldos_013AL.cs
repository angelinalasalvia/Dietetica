using BE_013AL;
using BLL_013AL;
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
    public partial class Respaldos_013AL : Form, IObserver_013AL
    {
        public Respaldos_013AL()
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
        private BackupBLL_013AL backuprestorebll = new BackupBLL_013AL();
        EventoBLL_013AL bbll = new EventoBLL_013AL();
        Usuarios_013AL user;

        private void button3_Click(object sender, EventArgs e)
        {
           using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;  
                folderDialog.Description = "Selecciona una carpeta para guardar el backup";  
                folderDialog.SelectedPath = @"C:\"; 

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = folderDialog.SelectedPath;  
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQL Backup Files (*.bak)|*.bak";




                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                   textBox2.Text = openFileDialog.FileName;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                try
                {
                    backuprestorebll.RealizarBackup_013AL(textBox1.Text);
                    MessageBox.Show("Backup realizado con éxito.");
                    textBox1.Text = "";
                    string usuarioLog = SingletonSession_013AL.Instance.IsLoggedIn_013AL()
                    ? SingletonSession_013AL.Instance.GetUsuario_013AL().Login_013AL
                    : "Administrador";
                    bbll.AgregarEvento_013AL(usuarioLog, "Respaldos", "Backup", 4);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al realizar el backup: {ex.Message}");
                    string usuarioLog = SingletonSession_013AL.Instance.IsLoggedIn_013AL()
                    ? SingletonSession_013AL.Instance.GetUsuario_013AL().Login_013AL
                    : "Administrador";
                    bbll.AgregarEvento_013AL(usuarioLog, "Respaldos", ex.Message, 4);
                }
            }
            else
            {
                MessageBox.Show("Seleccione una ubicación para el backup.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                try
                {
                    backuprestorebll.RealizarRestore_013AL(textBox2.Text);
                    MessageBox.Show("Restauración realizada con éxito.");
                    textBox2.Text = "";
                    string usuarioLog = SingletonSession_013AL.Instance.IsLoggedIn_013AL()
                    ? SingletonSession_013AL.Instance.GetUsuario_013AL().Login_013AL
                    : "Administrador";
                    bbll.AgregarEvento_013AL(usuarioLog, "Respaldos", "Restore", 4);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al restaurar la base de datos: {ex.Message}");
                    string usuarioLog = SingletonSession_013AL.Instance.IsLoggedIn_013AL()
                    ? SingletonSession_013AL.Instance.GetUsuario_013AL().Login_013AL
                    : "Administrador";
                    bbll.AgregarEvento_013AL(usuarioLog, "Respaldos", ex.Message, 4);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un archivo de restore.");
            }
        }

        private void Respaldos_Load(object sender, EventArgs e)
        {

        }
    }
}
