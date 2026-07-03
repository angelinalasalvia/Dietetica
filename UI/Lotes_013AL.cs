using BLL;
using Newtonsoft.Json.Linq;
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
    public partial class Lotes_013AL : Form
    {
        LoteBLL_013AL bll = new LoteBLL_013AL();
        public Lotes_013AL()
        {
            InitializeComponent();
            CargarLotes();
        }
        private void CargarLotes()
        {
            dataGridView1.DataSource = bll.ListarLotes_013AL();

            dataGridView1.Columns["IdLote"].Visible = false;

            dataGridView1.Columns["Producto"].HeaderText = "Producto";
            dataGridView1.Columns["NumeroLote"].HeaderText = "Lote";
            dataGridView1.Columns["FechaIngreso"].HeaderText = "Ingreso";
            dataGridView1.Columns["FechaVencimiento"].HeaderText = "Vencimiento";
            dataGridView1.Columns["CantidadInicial"].HeaderText = "Inicial";
            dataGridView1.Columns["CantidadDisponible"].HeaderText = "Disponible";
            dataGridView1.Columns["Estado"].HeaderText = "Estado";
            dataGridView1.Columns["Promocion"].HeaderText = "Promoción";
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                textBox1.Text = dataGridView1.CurrentRow.Cells["Producto"].Value.ToString();
                textBox2.Text = dataGridView1.CurrentRow.Cells["NumeroLote"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["FechaIngreso"].Value);
                dateTimePicker2.Value = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["FechaVencimiento"].Value);
                textBox5.Text = dataGridView1.CurrentRow.Cells["CantidadInicial"].Value.ToString();
                textBox6.Text = dataGridView1.CurrentRow.Cells["CantidadDisponible"].Value.ToString();
                comboBox1.Text = dataGridView1.CurrentRow.Cells["Estado"].Value.ToString();

                dateTimePicker2.Enabled = true;
                comboBox1.Enabled = true;

                button1.Enabled =
                    comboBox1.Text == "Próximo a vencer";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int idLote = Convert.ToInt32(dataGridView1.CurrentRow.Cells["IdLote"].Value);

            Promociones_013AL frm = new Promociones_013AL(idLote);
            frm.ShowDialog();

            CargarLotes();
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int idLote = Convert.ToInt32(dataGridView1.CurrentRow.Cells["IdLote"].Value);

            bll.ModificarLote_013AL(
                idLote,
                dateTimePicker2.Value,
                comboBox1.Text);

            MessageBox.Show("Lote modificado correctamente.");

            CargarLotes();

            dateTimePicker2.Enabled = false;
            comboBox1.Enabled = false;
        }

        private void Lotes_013AL_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }
    }
}
