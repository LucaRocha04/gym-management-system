using System;
using System.Windows.Forms;
using GimnasioApp.Services;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormConfiguracionEmail : Form
    {
        private EmailConfig _config;
        
        public FormConfiguracionEmail()
        {
            InitializeComponent();
            _config = EmailConfig.CargarConfiguracion();
            CargarConfiguracion();
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new Label();
            this.lblInstrucciones = new Label();
            this.lblApiKey = new Label();
            this.txtApiKey = new TextBox();
            this.lblEmailFrom = new Label();
            this.txtEmailFrom = new TextBox();
            this.lblNombre = new Label();
            this.txtNombre = new TextBox();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();
            this.btnObtenerApiKey = new Button();
            this.lblEstado = new Label();
            this.SuspendLayout();

            // FormConfiguracionEmail
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Configuración de Email - Brevo";

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Size = new System.Drawing.Size(300, 30);
            this.lblTitulo.Text = "Configuración de Email";

            // lblInstrucciones
            this.lblInstrucciones.Location = new System.Drawing.Point(20, 60);
            this.lblInstrucciones.Size = new System.Drawing.Size(460, 60);
            this.lblInstrucciones.Text = "Para enviar emails automáticos necesitas una API Key de Brevo (gratuita).\n" +
                                      "1. Crea una cuenta en brevo.com\n" +
                                      "2. Ve a 'API Keys' en tu dashboard\n" +
                                      "3. Crea una nueva API Key y pégala aquí";
            this.lblInstrucciones.ForeColor = System.Drawing.Color.FromArgb(127, 140, 141);

            // lblApiKey
            this.lblApiKey.AutoSize = true;
            this.lblApiKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblApiKey.Location = new System.Drawing.Point(20, 140);
            this.lblApiKey.Text = "API Key de Brevo:";

            // txtApiKey
            this.txtApiKey.Location = new System.Drawing.Point(20, 165);
            this.txtApiKey.Size = new System.Drawing.Size(360, 23);
            this.txtApiKey.UseSystemPasswordChar = true;

            // btnObtenerApiKey
            this.btnObtenerApiKey.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnObtenerApiKey.FlatStyle = FlatStyle.Flat;
            this.btnObtenerApiKey.ForeColor = System.Drawing.Color.White;
            this.btnObtenerApiKey.Location = new System.Drawing.Point(390, 165);
            this.btnObtenerApiKey.Size = new System.Drawing.Size(90, 23);
            this.btnObtenerApiKey.Text = "Obtener";
            this.btnObtenerApiKey.Click += BtnObtenerApiKey_Click;

            // lblEmailFrom
            this.lblEmailFrom.AutoSize = true;
            this.lblEmailFrom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEmailFrom.Location = new System.Drawing.Point(20, 210);
            this.lblEmailFrom.Text = "Email del remitente:";

            // txtEmailFrom
            this.txtEmailFrom.Location = new System.Drawing.Point(20, 235);
            this.txtEmailFrom.Size = new System.Drawing.Size(220, 23);

            // lblNombre
            this.lblNombre.AutoSize = true;
            this.lblNombre.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNombre.Location = new System.Drawing.Point(260, 210);
            this.lblNombre.Text = "Nombre del remitente:";

            // txtNombre
            this.txtNombre.Location = new System.Drawing.Point(260, 235);
            this.txtNombre.Size = new System.Drawing.Size(220, 23);

            // lblEstado
            this.lblEstado.Location = new System.Drawing.Point(20, 280);
            this.lblEstado.Size = new System.Drawing.Size(460, 40);
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);

            // btnGuardar
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(295, 340);
            this.btnGuardar.Size = new System.Drawing.Size(90, 30);
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += BtnGuardar_Click;

            // btnCancelar
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this.btnCancelar.FlatStyle = FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(390, 340);
            this.btnCancelar.Size = new System.Drawing.Size(90, 30);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += BtnCancelar_Click;

            // btnDiagnostico
            this.btnDiagnostico = new Button();
            this.btnDiagnostico.BackColor = System.Drawing.Color.FromArgb(230, 126, 34);
            this.btnDiagnostico.FlatStyle = FlatStyle.Flat;
            this.btnDiagnostico.ForeColor = System.Drawing.Color.White;
            this.btnDiagnostico.Location = new System.Drawing.Point(190, 340);
            this.btnDiagnostico.Size = new System.Drawing.Size(100, 30);
            this.btnDiagnostico.Text = "🔍 Probar";
            this.btnDiagnostico.Click += BtnDiagnostico_Click;

            // btnLimpiarDatos
            this.btnLimpiarDatos = new Button();
            this.btnLimpiarDatos.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnLimpiarDatos.FlatStyle = FlatStyle.Flat;
            this.btnLimpiarDatos.ForeColor = System.Drawing.Color.White;
            this.btnLimpiarDatos.Location = new System.Drawing.Point(20, 340);
            this.btnLimpiarDatos.Size = new System.Drawing.Size(160, 30);
            this.btnLimpiarDatos.Text = "🗑️ Limpiar Datos";
            this.btnLimpiarDatos.Click += BtnLimpiarDatos_Click;

            // Agregar controles
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblInstrucciones);
            this.Controls.Add(this.lblApiKey);
            this.Controls.Add(this.txtApiKey);
            this.Controls.Add(this.btnObtenerApiKey);
            this.Controls.Add(this.lblEmailFrom);
            this.Controls.Add(this.txtEmailFrom);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnDiagnostico);
            this.Controls.Add(this.btnLimpiarDatos);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblTitulo;
        private Label lblInstrucciones;
        private Label lblApiKey;
        private TextBox txtApiKey;
        private Label lblEmailFrom;
        private TextBox txtEmailFrom;
        private Label lblNombre;
        private TextBox txtNombre;
        private Button btnGuardar;
        private Button btnCancelar;
        private Button btnObtenerApiKey;
        private Button btnDiagnostico;
        private Button btnLimpiarDatos;
        private Label lblEstado;

        private void CargarConfiguracion()
        {
            txtApiKey.Text = _config.BrevoApiKey;
            txtEmailFrom.Text = _config.EmailFrom;
            txtNombre.Text = _config.EmailFromName;

            if (_config.EstaConfigurado())
            {
                lblEstado.Text = "✅ Email configurado correctamente";
                lblEstado.ForeColor = System.Drawing.Color.FromArgb(39, 174, 96);
            }
            else
            {
                lblEstado.Text = "⚠️ Email no configurado. Complete los datos para activar las notificaciones.";
                lblEstado.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            }
        }

        private void BtnObtenerApiKey_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://app.brevo.com/settings/keys/api",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el navegador: {ex.Message}", "Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtApiKey.Text))
                {
                    MessageBox.Show("La API Key es requerida", "Error", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmailFrom.Text))
                {
                    MessageBox.Show("El email del remitente es requerido", "Error", 
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _config.BrevoApiKey = txtApiKey.Text.Trim();
                _config.EmailFrom = txtEmailFrom.Text.Trim();
                _config.EmailFromName = txtNombre.Text.Trim();

                _config.GuardarConfiguracion();

                MessageBox.Show("Configuración guardada exitosamente", "Éxito", 
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error guardando configuración: {ex.Message}", "Error", 
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnDiagnostico_Click(object sender, EventArgs e)
        {
            var formPrueba = new FormPruebaEmail();
            formPrueba.ShowDialog();
        }

        private async void BtnLimpiarDatos_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show(
                "⚠️ ADVERTENCIA: Esta acción eliminará TODOS los datos de:\n\n" +
                "• Socios\n" +
                "• Pagos\n" +
                "• Asistencias\n" +
                "• Clases e Inscripciones\n\n" +
                "Se mantendrán:\n" +
                "• Usuarios\n" +
                "• Planes\n" +
                "• Configuración de Email\n\n" +
                "¿Está seguro de continuar?",
                "Limpiar Base de Datos",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    btnLimpiarDatos.Enabled = false;
                    btnLimpiarDatos.Text = "Limpiando...";

                    await GimnasioApp.Tools.DatabaseCleaner.LimpiarDatosPresentacionAsync();

                    MessageBox.Show(
                        "✅ Base de datos limpiada exitosamente.\n\nTodos los datos de socios, pagos y asistencias han sido eliminados.\n\nLa aplicación está lista para la presentación.",
                        "Limpieza Completada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"❌ Error al limpiar la base de datos:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    btnLimpiarDatos.Enabled = true;
                    btnLimpiarDatos.Text = "🗑️ Limpiar Datos";
                }
            }
        }
    }
}