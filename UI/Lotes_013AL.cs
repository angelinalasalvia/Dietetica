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

            dataGridView1.Columns["CodLote-013AL"].Visible = false;

            dataGridView1.Columns["Producto"].HeaderText = "Producto";
            dataGridView1.Columns["NumeroLote-013AL"].HeaderText = "Lote";
            dataGridView1.Columns["FechaIngreso-013AL"].HeaderText = "Ingreso";
            dataGridView1.Columns["FechaVencimiento-013AL"].HeaderText = "Vencimiento";
            dataGridView1.Columns["CantidadInicial-013AL"].HeaderText = "Inicial";
            dataGridView1.Columns["CantidadDisponible-013AL"].HeaderText = "Disponible";
            dataGridView1.Columns["Estado-013AL"].HeaderText = "Estado";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int idLote = Convert.ToInt32(dataGridView1.CurrentRow.Cells["CodLote-013AL"].Value);

            SolicitudPromocionForm_013AL frm = new SolicitudPromocionForm_013AL(idLote);
            frm.ShowDialog();

            CargarLotes();
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int idLote = Convert.ToInt32(dataGridView1.CurrentRow.Cells["CodLote-013AL"].Value);

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

        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                textBox1.Text = dataGridView1.CurrentRow.Cells["Producto"].Value.ToString();
                textBox2.Text = dataGridView1.CurrentRow.Cells["NumeroLote-013AL"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["FechaIngreso-013AL"].Value);
                dateTimePicker2.Value = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["FechaVencimiento-013AL"].Value);
                textBox5.Text = dataGridView1.CurrentRow.Cells["CantidadInicial-013AL"].Value.ToString();
                textBox6.Text = dataGridView1.CurrentRow.Cells["CantidadDisponible-013AL"].Value.ToString();
                comboBox1.Text = dataGridView1.CurrentRow.Cells["Estado-013AL"].Value.ToString();

                dateTimePicker2.Enabled = true;
                comboBox1.Enabled = true;

                DateTime fechaVencimiento = dateTimePicker2.Value.Date;
                int diasRestantes = (fechaVencimiento - DateTime.Today).Days;

                button1.Enabled = diasRestantes >= 0 && diasRestantes <= 14;
            }
        }
    }
}
