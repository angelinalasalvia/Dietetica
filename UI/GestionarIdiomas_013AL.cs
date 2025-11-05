using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Servicios_013AL;

namespace UI
{
    public partial class GestionarIdiomas_013AL : Form, IObserver_013AL
    {
        private readonly IdiomaBLL_013AL idiomaBLL = new IdiomaBLL_013AL();
        private readonly TraduccionBLL_013AL traduccionBLL = new TraduccionBLL_013AL();
        private readonly LanguageManager_013AL languageManager = LanguageManager_013AL.ObtenerInstancia_013AL();

        public GestionarIdiomas_013AL()
        {
            InitializeComponent();
            languageManager.Agregar_013AL(this);
            CargarIdiomas();
            dataGridView1.CellEndEdit += dataGridViewTraducciones_CellEndEdit;
            LanguageManager_013AL.ObtenerInstancia_013AL().Agregar_013AL(this);
            ActualizarIdioma_013AL();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            LanguageManager_013AL.ObtenerInstancia_013AL().Quitar_013AL(this);
        }
        private void CargarIdiomas()
        {
            comboBox1.DataSource = idiomaBLL.ListarIdiomas_013AL();
            comboBox1.DisplayMember = "Nombre_013AL";
            comboBox1.ValueMember = "IdIdioma_013AL";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Ingrese un nombre para el nuevo idioma.");
                return;
            }

            var nuevo = new Idioma_013AL { Nombre_013AL = textBox1.Text.Trim() };
            try
            {
                idiomaBLL.CrearIdiomaConTraducciones(nuevo);
                MessageBox.Show("Idioma creado con éxito.");
                CargarIdiomas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public void ActualizarIdioma_013AL()
        {
            languageManager.CambiarIdiomaControles_013AL(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un idioma.");
                return;
            }

            var idiomaSeleccionado = (Idioma_013AL)comboBox1.SelectedItem;
            CargarTraducciones(idiomaSeleccionado.IdIdioma_013AL);
        }

        private void CargarTraducciones(int idIdioma)
        {
            var traducciones = traduccionBLL.ObtenerPorIdioma(idIdioma);

            // Crear una tabla editable
            DataTable dt = new DataTable();
            dt.Columns.Add("IdTraduccion", typeof(int));
            dt.Columns.Add("Etiqueta", typeof(string));
            dt.Columns.Add("Traduccion", typeof(string));

            foreach (var t in traducciones)
            {
                dt.Rows.Add(t.IdTraduccion_013AL, t.Etiqueta_013AL.Nombre_013AL, t.Texto_013AL);
            }

            dataGridView1.DataSource = dt;

            // Configurar columnas
            dataGridView1.Columns["IdTraduccion"].Visible = false;
            dataGridView1.Columns["Etiqueta"].ReadOnly = true;
            dataGridView1.Columns["Traduccion"].ReadOnly = false;

            // Permitir edición
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
        }


        private void dataGridViewTraducciones_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Solo procesar si la columna editada es “Traduccion”
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Traduccion")
                {
                    var fila = dataGridView1.Rows[e.RowIndex];

                    int idTraduccion = Convert.ToInt32(fila.Cells["IdTraduccion"].Value);
                    string nuevoTexto = fila.Cells["Traduccion"].Value?.ToString() ?? "";

                    traduccionBLL.ActualizarTexto(idTraduccion, nuevoTexto);

                    MessageBox.Show("Traducción actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (comboBox1.SelectedValue.Equals(languageManager.IdiomaActual_013AL))
                    {
                        languageManager.CargarIdioma_013AL();
                        languageManager.Notificar_013AL();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar traducción: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
