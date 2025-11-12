namespace GimnasioApp.Desktop.Forms
{
    partial class FormSocios
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridView dgvSocios;
        private Button btnAgregar;
        private Button btnVer;
        private Button btnEditar;
        private Button btnEliminar;
        private Button btnBuscar;
        private Button btnMorosos;
        private Button btnRefrescar;
        private TextBox txtBuscar;
        private Label lblBuscar;
        private Label lblTotal;
        private Panel panelTop;
        private Panel panelBottom;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvSocios = new DataGridView();
            this.btnAgregar = new Button();
            this.btnVer = new Button();
            this.btnEditar = new Button();
            this.btnEliminar = new Button();
            this.btnBuscar = new Button();
            this.btnMorosos = new Button();
            this.btnRefrescar = new Button();
            this.txtBuscar = new TextBox();
            this.lblBuscar = new Label();
            this.lblTotal = new Label();
            this.panelTop = new Panel();
            this.panelBottom = new Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSocios)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            
            // panelTop
            this.panelTop.Controls.Add(this.lblBuscar);
            this.panelTop.Controls.Add(this.txtBuscar);
            this.panelTop.Controls.Add(this.btnBuscar);
            this.panelTop.Controls.Add(this.btnRefrescar);
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Location = new Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new Size(900, 60);
            this.panelTop.TabIndex = 0;
            
            // lblBuscar
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.Location = new Point(12, 18);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new Size(80, 15);
            this.lblBuscar.Text = "Buscar socio:";
            
            // txtBuscar
            this.txtBuscar.Location = new Point(150, 18);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new Size(220, 23);
            this.txtBuscar.TabIndex = 1;
            this.txtBuscar.KeyPress += new KeyPressEventHandler(this.txtBuscar_KeyPress);
            
            // btnBuscar
            this.btnBuscar.BackColor = Color.FromArgb(155, 89, 182);
            this.btnBuscar.FlatStyle = FlatStyle.Standard;
            this.btnBuscar.ForeColor = Color.White;
            this.btnBuscar.Location = new Point(400, 15);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new Size(90, 35);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.btnBuscar.Click += new EventHandler(this.btnBuscar_Click);
            
            // btnRefrescar
            this.btnRefrescar.BackColor = Color.FromArgb(52, 152, 219);
            this.btnRefrescar.FlatStyle = FlatStyle.Standard;
            this.btnRefrescar.ForeColor = Color.White;
            this.btnRefrescar.Location = new Point(500, 15);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new Size(90, 35);
            this.btnRefrescar.TabIndex = 3;
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.UseVisualStyleBackColor = true;
            this.btnRefrescar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point);
            this.btnRefrescar.Click += new EventHandler(this.btnRefrescar_Click);
            
            // dgvSocios
            this.dgvSocios.AllowUserToAddRows = false;
            this.dgvSocios.AllowUserToDeleteRows = false;
            this.dgvSocios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSocios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSocios.Dock = DockStyle.Fill;
            this.dgvSocios.Location = new Point(0, 60);
            this.dgvSocios.MultiSelect = false;
            this.dgvSocios.Name = "dgvSocios";
            this.dgvSocios.ReadOnly = true;
            this.dgvSocios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSocios.Size = new Size(900, 390);
            this.dgvSocios.TabIndex = 4;
            
            // panelBottom
            this.panelBottom.Controls.Add(this.btnAgregar);
            this.panelBottom.Controls.Add(this.btnVer);
            this.panelBottom.Controls.Add(this.btnEditar);
            this.panelBottom.Controls.Add(this.btnEliminar);
            this.panelBottom.Controls.Add(this.btnMorosos);
            this.panelBottom.Controls.Add(this.lblTotal);
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Location = new Point(0, 450);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new Size(900, 60); // Aumentado de 50 a 60
            this.panelBottom.TabIndex = 5;
            
            // btnAgregar
            this.btnAgregar.BackColor = Color.FromArgb(0, 150, 136);
            this.btnAgregar.FlatStyle = FlatStyle.Flat;
            this.btnAgregar.ForeColor = Color.White;
            this.btnAgregar.Location = new Point(10, 14);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new Size(100, 32);
            this.btnAgregar.TabIndex = 6;
            this.btnAgregar.Text = "Nuevo";
            this.btnAgregar.UseVisualStyleBackColor = false;
            this.btnAgregar.Click += new EventHandler(this.btnAgregar_Click);
            
            // btnEditar
            this.btnEditar.BackColor = Color.FromArgb(33, 150, 243);
            this.btnEditar.FlatStyle = FlatStyle.Flat;
            this.btnEditar.ForeColor = Color.White;
            // btnVer
            this.btnVer.BackColor = Color.FromArgb(0, 123, 255);
            this.btnVer.FlatStyle = FlatStyle.Flat;
            this.btnVer.ForeColor = Color.White;
            this.btnVer.Location = new Point(120, 14);
            this.btnVer.Name = "btnVer";
            this.btnVer.Size = new Size(100, 32);
            this.btnVer.TabIndex = 6;
            this.btnVer.Text = "👁️ Ver";
            this.btnVer.UseVisualStyleBackColor = false;
            this.btnVer.Click += new EventHandler(this.btnVer_Click);
            
            this.btnEditar.Location = new Point(230, 14);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new Size(100, 32);
            this.btnEditar.TabIndex = 7;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = false;
            this.btnEditar.Click += new EventHandler(this.btnEditar_Click);
            
            // btnEliminar
            this.btnEliminar.BackColor = Color.FromArgb(244, 67, 54);
            this.btnEliminar.FlatStyle = FlatStyle.Flat;
            this.btnEliminar.ForeColor = Color.White;
            this.btnEliminar.Location = new Point(340, 14);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new Size(100, 32);
            this.btnEliminar.TabIndex = 8;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new EventHandler(this.btnEliminar_Click);
            
            // btnMorosos
            this.btnMorosos.BackColor = Color.FromArgb(255, 152, 0);
            this.btnMorosos.FlatStyle = FlatStyle.Flat;
            this.btnMorosos.ForeColor = Color.White;
            this.btnMorosos.Location = new Point(450, 14);
            this.btnMorosos.Name = "btnMorosos";
            this.btnMorosos.Size = new Size(100, 32);
            this.btnMorosos.TabIndex = 9;
            this.btnMorosos.Text = "Morosos";
            this.btnMorosos.UseVisualStyleBackColor = false;
            this.btnMorosos.Click += new EventHandler(this.btnMorosos_Click);
            
            // lblTotal
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblTotal.Location = new Point(650, 16);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new Size(120, 19);
            this.lblTotal.Text = "Total socios: 0";
            
            // FormSocios
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(900, 500);
            this.Controls.Add(this.dgvSocios);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Name = "FormSocios";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Gestión de Socios";
            this.Load += new EventHandler(this.FormSocios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSocios)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
