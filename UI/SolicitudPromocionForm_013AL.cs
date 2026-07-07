using BE;
using BE_013AL;
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
    public partial class SolicitudPromocionForm_013AL : Form
    {
        private int _idLote;
        private int _idSolicitud;
        Usuarios_013AL user = SingletonSession_013AL.Instance.GetUsuario_013AL();
        /*public SolicitudPromocionForm_013AL(int idLote, string producto, string lote, DateTime vencimiento, int stock)
        {            
            InitializeComponent();

            _idLote = idLote;

            textBox1.Text = producto;
            textBox2.Text = lote;
            dateTimePicker1.Value = vencimiento;
            textBox4.Text = stock.ToString();
        }*/

        SolicitudPromocionBLL_013AL bll = new SolicitudPromocionBLL_013AL();

        //=========================
        // Constructor Supervisor
        //=========================

        public SolicitudPromocionForm_013AL()
        {
            InitializeComponent();

            ConfigurarModoSupervisor();

            CargarSolicitudesPendientes();
        }
        private void CargarSolicitudesPendientes()
        {
            comboBox1.DataSource = bll.ListarSolicitudesPendientes_013AL();

            comboBox1.DisplayMember = "Producto";

            comboBox1.ValueMember = "CodSolicitud-013AL";

        }

        private void AnalizarSolicitud()
        {
            DateTime fechaVencimiento = dateTimePicker1.Value;

            int stock = Convert.ToInt32(textBox4.Text);

            Analizar(fechaVencimiento, stock);
        }

        private void Analizar(DateTime fechaVencimiento, int stock)
        {
            bool promocionActiva = bll.ExistePromocionActiva_013AL(_idLote);

            bool tuvoPromocion =
                bll.TuvoPromocionAnterior_013AL(_idLote);

            int puntaje = 0;

            int diasRestantes =
                (fechaVencimiento.Date - DateTime.Today).Days;

            string recomendacion;

            richTextBox1.Clear();

            richTextBox1.AppendText("ANÁLISIS DEL SISTEMA\n\n");

            if (diasRestantes > 10)
            {
                puntaje += 40;
                richTextBox1.AppendText($"✔ Restan {diasRestantes} días para el vencimiento.\n");
            }
            else if (diasRestantes >= 5)
            {
                puntaje += 20;
                richTextBox1.AppendText($"⚠ Restan {diasRestantes} días para el vencimiento.\n");
            }
            else
            {
                richTextBox1.AppendText($"✖ Restan solamente {diasRestantes} días para el vencimiento.\n");
            }

            if (stock > 30)
            {
                puntaje += 30;
                richTextBox1.AppendText($"✔ Stock disponible: {stock} unidades.\n");
            }
            else if (stock >= 10)
            {
                puntaje += 15;
                richTextBox1.AppendText($"⚠ Stock disponible: {stock} unidades.\n");
            }
            else
            {
                richTextBox1.AppendText($"✖ Stock bajo: {stock} unidades.\n");
            }

            if (promocionActiva)
            {
                puntaje -= 30;
                richTextBox1.AppendText("✖ Existe una promoción activa.\n");
            }
            else
            {
                puntaje += 20;
                richTextBox1.AppendText("✔ No existen promociones activas.\n");
            }

            if (tuvoPromocion)
            {
                puntaje -= 10;
                richTextBox1.AppendText("⚠ El lote ya tuvo promociones anteriormente.\n");
            }
            else
            {
                richTextBox1.AppendText("✔ El lote no tuvo promociones anteriores.\n");
            }

            if (puntaje >= 80)
                recomendacion = "APROBAR";
            else if (puntaje >= 50)
                recomendacion = "REVISAR";
            else
                recomendacion = "RECHAZAR";

            richTextBox1.AppendText($"\nÍndice de viabilidad: {puntaje}/100\n");
            richTextBox1.AppendText($"Recomendación: {recomendacion}");
        }
        //=========================
        // Constructor Almacenista
        //=========================

        public SolicitudPromocionForm_013AL(
            int idLote,
            string producto,
            string lote,
            DateTime vencimiento,
            int stock)
        {
            InitializeComponent();

            ConfigurarModoAlmacenista();

            _idLote = idLote;

            textBox1.Text = producto;
            textBox2.Text = lote;
            dateTimePicker1.Value = vencimiento;
            textBox4.Text = stock.ToString();
        }
        private void ConfigurarModoAlmacenista()
        {
            comboBox1.Visible = false;
            textBox3.ReadOnly = false;
            button2.Visible = false;
            button3.Visible = false;
            richTextBox1.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            comboBox2.Visible = false;
            textBox5.Visible = false;
            dateTimePicker2.Visible = false;
        }

        private void ConfigurarModoSupervisor()
        {
            comboBox1.Visible = true;
            textBox3.ReadOnly = true;
            button1.Visible = false;
            button2.Visible = true;
            button3.Visible = true;
            richTextBox1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bll.ExisteSolicitudPendiente_013AL(_idLote))
            {
                MessageBox.Show(
                    "Ya existe una solicitud pendiente para este lote.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            bll.AgregarSolicitudPromocion_013AL(new BE.SolicitudPromocion_013AL
            {
                CodLote_013AL = _idLote,
                FechaSolicitud_013AL = DateTime.Now,
                Estado_013AL = "Pendiente",
                Observaciones_013AL = textBox3.Text,
                UsuarioSolicitante_013AL = user.Login_013AL
            });
            MessageBox.Show("Solicitud enviada correctamente.");

            this.DialogResult = DialogResult.OK;

            Close();
        }
        private void CargarSolicitud(int idSolicitud)
        {
            _idSolicitud = idSolicitud;

            DataTable dt = bll.TraerSolicitudPorId_013AL(idSolicitud);

            if (dt.Rows.Count == 0)
                return;

            DataRow fila = dt.Rows[0];

            _idLote = Convert.ToInt32(fila["CodLote-013AL"]);

            textBox1.Text = fila["Producto"].ToString();

            textBox2.Text = fila["NumeroLote-013AL"].ToString();

            dateTimePicker1.Value =
                Convert.ToDateTime(fila["FechaVencimiento-013AL"]);

            textBox4.Text =
                fila["CantidadDisponible-013AL"].ToString();

            textBox3.Text =
                fila["Observaciones-013AL"].ToString();


            AnalizarSolicitud();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue == null)
                return;

            int idSolicitud;

            if (!int.TryParse(comboBox1.SelectedValue.ToString(), out idSolicitud))
                return;

            CargarSolicitud(idSolicitud);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // La fecha de fin no puede ser anterior a hoy
            if (dateTimePicker2.Value.Date < DateTime.Today)
            {
                MessageBox.Show(
                    "La fecha de fin de la promoción no puede ser anterior a la fecha actual.",
                    "Fecha inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // La promoción no puede finalizar después del vencimiento del lote
            if (dateTimePicker2.Value.Date > dateTimePicker1.Value.Date)
            {
                MessageBox.Show(
                    "La fecha de fin de la promoción no puede superar la fecha de vencimiento del lote.",
                    "Fecha inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Validación del tipo
            if (string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                MessageBox.Show(
                    "Seleccione un tipo de promoción.",
                    "Datos incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Validación del valor
            int? valor = null;

            if (!string.IsNullOrWhiteSpace(textBox5.Text))
            {
                if (!int.TryParse(textBox5.Text, out int valorIngresado) || valorIngresado <= 0)
                {
                    MessageBox.Show(
                        "Ingrese un valor numérico mayor a cero para la promoción.",
                        "Datos inválidos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                valor = valorIngresado;
            }

            SolicitudPromocion_013AL solicitud = new SolicitudPromocion_013AL();

            solicitud.UsuarioSupervisor_013AL = user.Login_013AL;
            solicitud.FechaResolucion_013AL = DateTime.Now;

            Promocion_013AL promo = new Promocion_013AL();

            promo.CodSolicitud_013AL = _idSolicitud;
            promo.Tipo_013AL = comboBox2.Text;
            promo.Valor_013AL = valor;
            promo.FechaInicio_013AL = DateTime.Today;
            promo.FechaFin_013AL = dateTimePicker2.Value;
            promo.CodLote_013AL = _idLote;

            bll.AprobarSolicitud_013AL(promo, solicitud);

            MessageBox.Show(
                "La solicitud fue aprobada correctamente.",
                "Solicitud aprobada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bll.RechazarSolicitud_013AL(_idSolicitud);

            MessageBox.Show("La solicitud fue rechazada.");

            Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.Text == "2 X 1" || comboBox2.Text == "3 X 2" || comboBox2.Text == "Segunda unidad al 50%")
            {
                textBox5.Clear();
                textBox5.ReadOnly = true;
            }
            else
            {
                textBox5.ReadOnly = false;
            }
        }
    }
}
