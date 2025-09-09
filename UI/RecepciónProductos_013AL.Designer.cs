namespace UI
{
    partial class RecepciónProductos_013AL
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
            this.dataGridViewProductos = new System.Windows.Forms.DataGridView();
            this.comboBoxOrdenesCompra = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProductos)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewProductos
            // 
            this.dataGridViewProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewProductos.Location = new System.Drawing.Point(12, 35);
            this.dataGridViewProductos.Name = "dataGridViewProductos";
            this.dataGridViewProductos.Size = new System.Drawing.Size(496, 150);
            this.dataGridViewProductos.TabIndex = 0;
            // 
            // comboBoxOrdenesCompra
            // 
            this.comboBoxOrdenesCompra.FormattingEnabled = true;
            this.comboBoxOrdenesCompra.Location = new System.Drawing.Point(12, 8);
            this.comboBoxOrdenesCompra.Name = "comboBoxOrdenesCompra";
            this.comboBoxOrdenesCompra.Size = new System.Drawing.Size(121, 21);
            this.comboBoxOrdenesCompra.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(139, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Seleccionar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RecepciónProductos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 231);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBoxOrdenesCompra);
            this.Controls.Add(this.dataGridViewProductos);
            this.Name = "RecepciónProductos";
            this.Text = "RecepciónProductos";
            this.Load += new System.EventHandler(this.RecepciónProductos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProductos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewProductos;
        private System.Windows.Forms.ComboBox comboBoxOrdenesCompra;
        private System.Windows.Forms.Button button1;
    }
}