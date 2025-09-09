namespace UI
{
    partial class CobrarVenta_013AL
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.txtcuil = new System.Windows.Forms.TextBox();
            this.txttotal = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTarjeta = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Cliente:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Datos de la venta:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Total:";
            // 
            // btnCobrar
            // 
            this.btnCobrar.Location = new System.Drawing.Point(16, 147);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(49, 23);
            this.btnCobrar.TabIndex = 17;
            this.btnCobrar.Text = "Cobrar";
            this.btnCobrar.UseVisualStyleBackColor = true;
            this.btnCobrar.Click += new System.EventHandler(this.btnCobrar_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Método de pago:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Efectivo",
            "Tarjeta Débito"});
            this.comboBox1.Location = new System.Drawing.Point(107, 75);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 20;
            // 
            // txtcuil
            // 
            this.txtcuil.Location = new System.Drawing.Point(60, 25);
            this.txtcuil.Name = "txtcuil";
            this.txtcuil.ReadOnly = true;
            this.txtcuil.Size = new System.Drawing.Size(100, 20);
            this.txtcuil.TabIndex = 21;
            // 
            // txttotal
            // 
            this.txttotal.Location = new System.Drawing.Point(60, 49);
            this.txttotal.Name = "txttotal";
            this.txttotal.ReadOnly = true;
            this.txttotal.Size = new System.Drawing.Size(100, 20);
            this.txttotal.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Numero Tarjeta (opcional): ";
            // 
            // txtTarjeta
            // 
            this.txtTarjeta.Location = new System.Drawing.Point(17, 121);
            this.txtTarjeta.Name = "txtTarjeta";
            this.txtTarjeta.Size = new System.Drawing.Size(121, 20);
            this.txtTarjeta.TabIndex = 24;
            // 
            // CobrarVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 187);
            this.Controls.Add(this.txtTarjeta);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txttotal);
            this.Controls.Add(this.txtcuil);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnCobrar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CobrarVenta";
            this.Text = "CobrarVenta";
            this.Load += new System.EventHandler(this.CobrarVenta_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCobrar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox txtcuil;
        private System.Windows.Forms.TextBox txttotal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTarjeta;
    }
}