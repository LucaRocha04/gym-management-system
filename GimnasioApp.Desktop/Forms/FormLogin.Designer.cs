namespace GimnasioApp.Desktop.Forms
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblUsuario;
        private Label lblPassword;
        private Label lblTitulo;

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
            this.txtUsuario = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.lblUsuario = new Label();
            this.lblPassword = new Label();
            this.lblTitulo = new Label();
            this.SuspendLayout();
            
            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitulo.Location = new Point(80, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new Size(240, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Gimnasio - Login";
            
            // lblUsuario
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new Point(50, 90);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new Size(120, 15);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuario o Email:";
            
            // txtUsuario
            this.txtUsuario.Location = new Point(50, 110);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new Size(300, 23);
            this.txtUsuario.TabIndex = 2;
            this.txtUsuario.KeyPress += new KeyPressEventHandler(this.txtUsuario_KeyPress);
            
            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new Point(50, 150);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(70, 15);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Contraseña:";
            
            // txtPassword
            this.txtPassword.Location = new Point(50, 170);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new Size(300, 23);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.KeyPress += new KeyPressEventHandler(this.txtPassword_KeyPress);
            
            // btnLogin
            this.btnLogin.BackColor = Color.FromArgb(0, 120, 215);
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.Location = new Point(125, 220);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new Size(150, 35);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Ingresar";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            
            // FormLogin
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(400, 300);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Gimnasio - Login";
            this.Load += new EventHandler(this.FormLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
