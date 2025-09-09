namespace UI
{
    partial class GenerarFactura_013AL
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnFactura = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnBuscarCliente = new System.Windows.Forms.Button();
            this.btnProductos = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 377);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total:";
            // 
            // btnCobrar
            // 
            this.btnCobrar.Location = new System.Drawing.Point(12, 399);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(49, 23);
            this.btnCobrar.TabIndex = 2;
            this.btnCobrar.Text = "Cobrar";
            this.btnCobrar.UseVisualStyleBackColor = true;
            this.btnCobrar.Click += new System.EventHandler(this.btnCobrar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Datos de la venta:";
            // 
            // btnFactura
            // 
            this.btnFactura.Location = new System.Drawing.Point(67, 399);
            this.btnFactura.Name = "btnFactura";
            this.btnFactura.Size = new System.Drawing.Size(93, 23);
            this.btnFactura.TabIndex = 5;
            this.btnFactura.Text = "Imprimir Factura";
            this.btnFactura.UseVisualStyleBackColor = true;
            this.btnFactura.Click += new System.EventHandler(this.btnFactura_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Productos:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 242);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "CUIL Cliente:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 258);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(130, 20);
            this.textBox1.TabIndex = 10;
            this.textBox1.Text = "23924";
            
            // 
            // btnBuscarCliente
            // 
            this.btnBuscarCliente.Location = new System.Drawing.Point(145, 258);
            this.btnBuscarCliente.Name = "btnBuscarCliente";
            this.btnBuscarCliente.Size = new System.Drawing.Size(95, 23);
            this.btnBuscarCliente.TabIndex = 13;
            this.btnBuscarCliente.Text = "Buscar Cliente";
            this.btnBuscarCliente.UseVisualStyleBackColor = true;
            this.btnBuscarCliente.Click += new System.EventHandler(this.btnBuscarCliente_Click);
            // 
            // btnProductos
            // 
            this.btnProductos.Location = new System.Drawing.Point(9, 34);
            this.btnProductos.Name = "btnProductos";
            this.btnProductos.Size = new System.Drawing.Size(130, 23);
            this.btnProductos.TabIndex = 14;
            this.btnProductos.Text = "Seleccionar Productos";
            this.btnProductos.UseVisualStyleBackColor = true;
            this.btnProductos.Click += new System.EventHandler(this.btnProductos_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(9, 86);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(267, 150);
            this.dataGridView1.TabIndex = 17;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(70, 60);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "ActualizarTabla";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(52, 373);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(100, 20);
            this.txtTotal.TabIndex = 19;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(168, 60);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 23);
            this.button2.TabIndex = 20;
            this.button2.Text = "EliminarProducto";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(9, 287);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(267, 80);
            this.dataGridView2.TabIndex = 21;
            // 
            // GenerarFactura_013AL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 431);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnProductos);
            this.Controls.Add(this.btnBuscarCliente);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnFactura);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCobrar);
            this.Controls.Add(this.label1);
            this.Name = "GenerarFactura_013AL";
            this.Text = "GenerarFactura";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCobrar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnFactura;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnBuscarCliente;
        private System.Windows.Forms.Button btnProductos;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataGridView2;
    }
}