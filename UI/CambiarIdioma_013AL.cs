using BE;
using BLL;
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
    public partial class CambiarIdioma_013AL : Form, IObserver_013AL
    {
        public CambiarIdioma_013AL()
        {
            InitializeComponent();
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
        }

        public void ActualizarIdioma_013AL()
        {
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
           
        }
        
        IdiomaBLL_013AL bll = new IdiomaBLL_013AL();

        private void CargarIdiomas()
        {
            List<Idioma_013AL> idiomas = bll.ListarIdiomas_013AL();
            comboBox1.DataSource = idiomas;
            comboBox1.DisplayMember = "Nombre_013AL";
            comboBox1.ValueMember = "IdIdioma_013AL";
            comboBox1.SelectedIndex = -1; 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var lm = LanguageManager_013AL.ObtenerInstancia_013AL();

            if (comboBox1.SelectedValue != null)
            {
                int nuevoId = (int)comboBox1.SelectedValue;
                lm.CambiarIdioma_013AL(nuevoId);

                MessageBox.Show("Idioma cambiado correctamente.", "Idioma", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Seleccione un idioma antes de continuar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CambiarIdioma_013AL_Load(object sender, EventArgs e)
        {
            CargarIdiomas();
        }
    }
}
