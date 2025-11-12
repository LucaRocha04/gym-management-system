using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GimnasioApp.Services;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormPruebaEmail : Form
    {
        private EmailServiceBrevo _emailService;
        
        public FormPruebaEmail()
        {
            InitializeComponent();
            _emailService = new EmailServiceBrevo();
            VerificarConfiguracion();
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new Label();
            this.lblEstado = new Label();
            this.lblDestino = new Label();
            this.txtDestino = new TextBox();
            this.btnPrueba = new Button();
            this.btnVerificar = new Button();
            this.txtResultado = new TextBox();
            this.SuspendLayout();

            // Form
            this.Text = "Prueba de Email - Diagnóstico";
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 20);
            this.lblTitulo.Text = "Diagnóstico del Sistema de Email";

            // lblEstado
            this.lblEstado.Location = new System.Drawing.Point(20, 60);
            this.lblEstado.Size = new System.Drawing.Size(560, 40);
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 10F);

            // lblDestino
            this.lblDestino.AutoSize = true;
            this.lblDestino.Location = new System.Drawing.Point(20, 120);
            this.lblDestino.Text = "Email de prueba:";
            this.lblDestino.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // txtDestino
            this.txtDestino.Location = new System.Drawing.Point(20, 145);
            this.txtDestino.Size = new System.Drawing.Size(300, 23);
            this.txtDestino.PlaceholderText = "tu-email@ejemplo.com";

            // btnPrueba
            this.btnPrueba.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnPrueba.FlatStyle = FlatStyle.Flat;
            this.btnPrueba.ForeColor = System.Drawing.Color.White;
            this.btnPrueba.Location = new System.Drawing.Point(340, 145);
            this.btnPrueba.Size = new System.Drawing.Size(120, 30);
            this.btnPrueba.Text = "Enviar Prueba";
            this.btnPrueba.Click += BtnPrueba_Click;

            // btnVerificar
            this.btnVerificar.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnVerificar.FlatStyle = FlatStyle.Flat;
            this.btnVerificar.ForeColor = System.Drawing.Color.White;
            this.btnVerificar.Location = new System.Drawing.Point(470, 145);
            this.btnVerificar.Size = new System.Drawing.Size(100, 30);
            this.btnVerificar.Text = "Verificar";
            this.btnVerificar.Click += BtnVerificar_Click;

            // txtResultado
            this.txtResultado.Location = new System.Drawing.Point(20, 190);
            this.txtResultado.Size = new System.Drawing.Size(550, 250);
            this.txtResultado.Multiline = true;
            this.txtResultado.ScrollBars = ScrollBars.Vertical;
            this.txtResultado.ReadOnly = true;
            this.txtResultado.Font = new System.Drawing.Font("Consolas", 9F);

            // Agregar controles
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.lblDestino);
            this.Controls.Add(this.txtDestino);
            this.Controls.Add(this.btnPrueba);
            this.Controls.Add(this.btnVerificar);
            this.Controls.Add(this.txtResultado);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblTitulo;
        private Label lblEstado;
        private Label lblDestino;
        private TextBox txtDestino;
        private Button btnPrueba;
        private Button btnVerificar;
        private TextBox txtResultado;

        private void VerificarConfiguracion()
        {
            if (_emailService.EstaConfigurado())
            {
                lblEstado.Text = "✅ Email configurado correctamente";
                lblEstado.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblEstado.Text = "❌ Email NO configurado. Verifica la API Key y configuración.";
                lblEstado.ForeColor = System.Drawing.Color.Red;
            }
        }

        private async void BtnPrueba_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDestino.Text))
            {
                MessageBox.Show("Ingresa un email de destino", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnPrueba.Enabled = false;
            btnPrueba.Text = "Enviando...";
            txtResultado.Clear();
            txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Iniciando envío de email de prueba...\n");
            txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Destinatario: {txtDestino.Text}\n");

            try
            {
                var resultado = await _emailService.EnviarEmailAsync(
                    txtDestino.Text,
                    "🧪 Email de Prueba - Gimnasio LR",
                    GenerarEmailPrueba(),
                    true
                );

                if (resultado)
                {
                    txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] ✅ Email enviado exitosamente\n");
                    txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Revisa tu bandeja de entrada (y spam)\n");
                }
                else
                {
                    txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] ❌ Error al enviar email\n");
                    txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Revisa la cola de emails offline\n");
                }
            }
            catch (Exception ex)
            {
                txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] ❌ Excepción: {ex.Message}\n");
            }
            finally
            {
                btnPrueba.Enabled = true;
                btnPrueba.Text = "Enviar Prueba";
            }
        }

        private async void BtnVerificar_Click(object sender, EventArgs e)
        {
            txtResultado.Clear();
            txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] === DIAGNÓSTICO COMPLETO ===\n");
            
            // Verificar configuración
            var config = EmailConfig.CargarConfiguracion();
            txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] API Key configurada: {(!string.IsNullOrEmpty(config.BrevoApiKey) ? "✅ SÍ" : "❌ NO")}\n");
            txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Email From: {config.EmailFrom}\n");
            txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Nombre From: {config.EmailFromName}\n");
            txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Configuración válida: {(config.EstaConfigurado() ? "✅ SÍ" : "❌ NO")}\n");
            
            // Verificar cola de emails
            try
            {
                var queuePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "email_queue");
                if (Directory.Exists(queuePath))
                {
                    var files = Directory.GetFiles(queuePath, "*.json");
                    txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Emails en cola: {files.Length}\n");
                    
                    if (files.Length > 0)
                    {
                        txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Intentando procesar cola...\n");
                        var procesados = await _emailService.ProcesarColaAsync();
                        txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Emails procesados: {procesados}\n");
                    }
                }
                else
                {
                    txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Directorio de cola no existe\n");
                }
            }
            catch (Exception ex)
            {
                txtResultado.AppendText($"[{DateTime.Now:HH:mm:ss}] Error verificando cola: {ex.Message}\n");
            }
        }

        private string GenerarEmailPrueba()
        {
            return @"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px;'>
        <h1 style='color: #2c3e50; text-align: center;'>🧪 Email de Prueba</h1>
        <p>¡Hola!</p>
        <p>Este es un email de prueba del sistema de notificaciones del <strong>Gimnasio LR</strong>.</p>
        <p>Si recibes este mensaje, significa que:</p>
        <ul>
            <li>✅ La configuración de Brevo es correcta</li>
            <li>✅ La API Key está funcionando</li>
            <li>✅ El sistema de emails está operativo</li>
        </ul>
        <p><strong>Fecha y hora:</strong> " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + @"</p>
        <hr style='margin: 20px 0;'>
        <p style='text-align: center; color: #7f8c8d;'>
            <strong>Gimnasio LR</strong><br>
            Av. Libertador 1234, San Miguel de Tucumán<br>
            📞 3813860020
        </p>
    </div>
</body>
</html>";
        }
    }
}