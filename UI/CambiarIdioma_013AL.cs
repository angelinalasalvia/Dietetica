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
            //LanguageManager.ObtenerInstancia().Notificar();
            LanguageManager_013AL.ObtenerInstancia_013AL().CambiarIdiomaControles_013AL(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
           /* base.OnFormClosing(e);
            LanguageManager.ObtenerInstancia().Quitar(this);*/
        }
        

        private void button3_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text == "Español")
            {
                SingletonSession_013AL.Instance.IdiomaActual_013AL = "es";
            }
            if (comboBox1.Text == "Inglés")
            {
                SingletonSession_013AL.Instance.IdiomaActual_013AL = "en";
            }
        }
    }
}
