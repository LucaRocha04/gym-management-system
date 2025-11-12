namespace GimnasioApp.Desktop.Forms
{
    partial class FormSocioDetalle
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtNombre, txtApellido, txtDNI, txtTelefono, txtMail, txtDireccion;
        private ComboBox cboPlan, cboEstado;
        private DateTimePicker dtpFechaIngreso;
        private Button btnGuardar, btnCancelar;
        private Label lblNombre, lblApellido, lblDNI, lblTelefono, lblMail, lblDireccion, lblPlan, lblEstado, lblFechaIngreso;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtNombre = new TextBox();
            this.txtApellido = new TextBox();
            this.txtDNI = new TextBox();
            this.txtTelefono = new TextBox();
            this.txtMail = new TextBox();
            this.txtDireccion = new TextBox();
            this.cboPlan = new ComboBox();
            this.cboEstado = new ComboBox();
            this.dtpFechaIngreso = new DateTimePicker();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();
            this.lblNombre = new Label(); this.lblApellido = new Label(); this.lblDNI = new Label();
            this.lblTelefono = new Label(); this.lblMail = new Label(); this.lblDireccion = new Label();
            this.lblPlan = new Label(); this.lblEstado = new Label(); this.lblFechaIngreso = new Label();
            this.SuspendLayout();

            int y = 20;
            // Nombre
            this.lblNombre.Location = new Point(20, y); this.lblNombre.Size = new Size(100, 20); this.lblNombre.Text = "Nombre:";
            this.txtNombre.Location = new Point(130, y); this.txtNombre.Size = new Size(250, 23);
            y += 35;
            // Apellido
            this.lblApellido.Location = new Point(20, y); this.lblApellido.Size = new Size(100, 20); this.lblApellido.Text = "Apellido:";
            this.txtApellido.Location = new Point(130, y); this.txtApellido.Size = new Size(250, 23);
            y += 35;
            // DNI
            this.lblDNI.Location = new Point(20, y); this.lblDNI.Size = new Size(100, 20); this.lblDNI.Text = "DNI:";
            this.txtDNI.Location = new Point(130, y); this.txtDNI.Size = new Size(250, 23);
            y += 35;
            // Teléfono
            this.lblTelefono.Location = new Point(20, y); this.lblTelefono.Size = new Size(100, 20); this.lblTelefono.Text = "Teléfono:";
            this.txtTelefono.Location = new Point(130, y); this.txtTelefono.Size = new Size(250, 23);
            y += 35;
            // Mail
            this.lblMail.Location = new Point(20, y); this.lblMail.Size = new Size(100, 20); this.lblMail.Text = "Email:";
            this.txtMail.Location = new Point(130, y); this.txtMail.Size = new Size(250, 23);
            y += 35;
            // Dirección
            this.lblDireccion.Location = new Point(20, y); this.lblDireccion.Size = new Size(100, 20); this.lblDireccion.Text = "Dirección:";
            this.txtDireccion.Location = new Point(130, y); this.txtDireccion.Size = new Size(250, 23);
            y += 35;
            // Plan
            this.lblPlan.Location = new Point(20, y); this.lblPlan.Size = new Size(100, 20); this.lblPlan.Text = "Plan:";
            this.cboPlan.Location = new Point(130, y); this.cboPlan.Size = new Size(250, 23); this.cboPlan.DropDownStyle = ComboBoxStyle.DropDownList;
            y += 35;
            // Estado
            this.lblEstado.Location = new Point(20, y); this.lblEstado.Size = new Size(100, 20); this.lblEstado.Text = "Estado:";
            this.cboEstado.Location = new Point(130, y); this.cboEstado.Size = new Size(250, 23); this.cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboEstado.Items.AddRange(new object[] { "Activo", "Inactivo" });
            y += 35;
            // Fecha Ingreso
            this.lblFechaIngreso.Location = new Point(20, y); this.lblFechaIngreso.Size = new Size(100, 20); this.lblFechaIngreso.Text = "Fecha Ingreso:";
            this.dtpFechaIngreso.Location = new Point(130, y); this.dtpFechaIngreso.Size = new Size(250, 23);
            y += 40;
            // Botones
            this.btnGuardar.Location = new Point(130, y); this.btnGuardar.Size = new Size(100, 30); this.btnGuardar.Text = "Guardar";
            this.btnGuardar.BackColor = Color.FromArgb(0, 150, 136); this.btnGuardar.ForeColor = Color.White; this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.Click += new EventHandler(this.btnGuardar_Click);
            this.btnCancelar.Location = new Point(240, y); this.btnCancelar.Size = new Size(100, 30); this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new EventHandler(this.btnCancelar_Click);

            this.Controls.AddRange(new Control[] {
                lblNombre, txtNombre, lblApellido, txtApellido, lblDNI, txtDNI,
                lblTelefono, txtTelefono, lblMail, txtMail, lblDireccion, txtDireccion,
                lblPlan, cboPlan, lblEstado, cboEstado, lblFechaIngreso, dtpFechaIngreso,
                btnGuardar, btnCancelar
            });

            this.ClientSize = new Size(420, y + 60);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Detalle Socio";
            this.Load += new EventHandler(this.FormSocioDetalle_Load);
            this.ResumeLayout(false);
        }
    }
}
