using BE_013AL;
using BLL_013AL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Servicios;
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
    public partial class BitácoraEventos_013AL : Form, IObserver_013AL
    {
        public BitácoraEventos_013AL()
        {
            InitializeComponent();
            dateTimePicker1.ValueChanged += new EventHandler(dateTimePicker1_ValueChanged);
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
        BLLBitacora_013AL bll = new BLLBitacora_013AL();
        private void ListarEventos_013AL()
        {
            dataGridView1.DataSource = bll.ListarEventos_013AL();
        }

        private void BitácoraEventos_Load(object sender, EventArgs e)
        {
            this.ListarEventos_013AL();
            
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dateTimePicker2.Value = dateTimePicker1.Value.AddDays(30);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = bll.ConsultasEventos_013AL(
            textBox3.Text, 
            string.IsNullOrEmpty(dateTimePicker1.Text) ? (DateTime?)null : Convert.ToDateTime(dateTimePicker1.Text), 
            string.IsNullOrEmpty(dateTimePicker2.Text) ? (DateTime?)null : Convert.ToDateTime(dateTimePicker2.Text), 
            comboBox2.Text, 
            comboBox1.Text, 
            string.IsNullOrEmpty(textBox8.Text) ? (int?)null : Convert.ToInt32(textBox8.Text) 
            );

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ListarEventos_013AL();
            textBox3.Text = "";
            comboBox2.Text = "";
            comboBox1.Text = "";
            textBox8.Text = "";
            dateTimePicker1.Text = "";
            dateTimePicker2.Text = "";
            
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string login = dataGridView1.SelectedRows[0].Cells["Login-013AL"].Value.ToString();

                DataTable usuarioData = bll.ObtenerNombreApellidoPorLogin_013AL(login);

                if (usuarioData.Rows.Count > 0)
                {
                    textBox1.Text = usuarioData.Rows[0]["Nombres-013AL"].ToString();
                    textBox2.Text = usuarioData.Rows[0]["Apellidos-013AL"].ToString();
                }
                else
                {
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<Eventos> listaEventos = new List<Eventos>();


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Login-013AL"].Value != null)
                {
                    Eventos evento = new Eventos()
                    {
                        Login = row.Cells["Login-013AL"].Value.ToString(),
                        Fecha = Convert.ToDateTime(row.Cells["Fecha-013AL"].Value).Date,
                        Hora = TimeSpan.Parse(row.Cells["Hora-013AL"].Value.ToString()),
                        Modulo = row.Cells["Modulo-013AL"].Value.ToString(),
                        Evento = row.Cells["Evento-013AL"].Value.ToString(),
                        Criticidad = Convert.ToInt32(row.Cells["Criticidad-013AL"].Value.ToString())
                    };
                    listaEventos.Add(evento);
                }
            }
            try
            {
                GenerarEventosPDF_013AL(listaEventos);
                string resultado;
                BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                resultado = bbll.AgregarEvento_013AL(user.Login_013AL, "Bitácora", "Generación de PDF sobre bitácora", 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el PDF: " + ex.Message);
                string resultado;
                BLLBitacora_013AL bbll = new BLLBitacora_013AL();
                Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
                resultado = bbll.AgregarEvento_013AL(user.Login_013AL, "Bitácora", "Error al generar PDF sobre bitácora", 1);
            }
        }

        private void GenerarEventosPDF_013AL(List<Eventos> evento)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF|*.pdf";
            saveFileDialog.Title = "Guardar Consulta";
            saveFileDialog.FileName = "ConsultaEventos" + ".pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    pdfDoc.Add(new Paragraph(" Dietetica Eat Healthy"));
                    
                    pdfDoc.Add(new Paragraph(" Consulta Bitácora de Eventos ")); 

                    
                    pdfDoc.Add(new Paragraph(" ")); 


                    PdfPTable table = new PdfPTable(6);
                    table.WidthPercentage = 80;
                    table.SetWidths(new float[] { 1, 1, 1, 1, 1, 1 });
                    
                    table.AddCell("Login");
                    table.AddCell("Fecha");
                    table.AddCell("Hora");
                    table.AddCell("Modulo");
                    table.AddCell("Evento");
                    table.AddCell("Criticidad");

                    foreach (var item in evento)
                    {
                        table.AddCell(item.Login.ToString());
                        table.AddCell(item.Fecha.ToString()); 
                        table.AddCell(item.Hora.ToString());
                        table.AddCell(item.Modulo.ToString());
                        table.AddCell(item.Evento.ToString());
                        table.AddCell(item.Criticidad.ToString());
                    }

                    pdfDoc.Add(table);

                    pdfDoc.Close();
                }

                MessageBox.Show("Pdf generado correctamente");
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value.AddDays(30);
        }
    }
}
