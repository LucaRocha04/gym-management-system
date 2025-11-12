using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
    public partial class SplashForm : Form
    {
        private System.Windows.Forms.Timer splashTimer;
        private Label lblAppName;
        private Label lblVersion;
        private ProgressBar progressBar;
        private Label lblStatus;

        public SplashForm()
        {
            InitializeComponent();
            UITheme.Apply(this);
        }

        private void InitializeComponent()
        {
            this.lblAppName = new Label();
            this.lblVersion = new Label();
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();
            this.splashTimer = new System.Windows.Forms.Timer();
            this.SuspendLayout();

            // SplashForm
            this.BackColor = Color.FromArgb(52, 73, 94);
            this.ClientSize = new Size(500, 300);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "GimnasioApp";

            // lblAppName
            this.lblAppName.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            this.lblAppName.ForeColor = Color.White;
            this.lblAppName.Location = new Point(50, 80);
            this.lblAppName.Size = new Size(400, 50);
            this.lblAppName.Text = "GimnasioApp";
            this.lblAppName.TextAlign = ContentAlignment.MiddleCenter;

            // lblVersion
            this.lblVersion.Font = new Font("Segoe UI", 10F);
            this.lblVersion.ForeColor = Color.FromArgb(189, 195, 199);
            this.lblVersion.Location = new Point(50, 130);
            this.lblVersion.Size = new Size(400, 25);
            this.lblVersion.Text = "Sistema de Gestión de Gimnasio v1.0";
            this.lblVersion.TextAlign = ContentAlignment.MiddleCenter;

            // progressBar
            this.progressBar.Location = new Point(75, 200);
            this.progressBar.Size = new Size(350, 20);
            this.progressBar.Style = ProgressBarStyle.Marquee;
            this.progressBar.MarqueeAnimationSpeed = 50;

            // lblStatus
            this.lblStatus.Font = new Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = Color.FromArgb(149, 165, 166);
            this.lblStatus.Location = new Point(75, 230);
            this.lblStatus.Size = new Size(350, 20);
            this.lblStatus.Text = "Inicializando...";
            this.lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            // Timer
            this.splashTimer.Interval = 3000; // 3 segundos
            this.splashTimer.Tick += SplashTimer_Tick;

            this.Controls.Add(this.lblAppName);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.ResumeLayout(false);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.lblStatus.Text = "Conectando a la base de datos...";
            this.splashTimer.Start();
        }

        private void SplashTimer_Tick(object? sender, EventArgs e)
        {
            this.splashTimer.Stop();
            this.Hide();
            
            // Mostrar el formulario de login
            var loginForm = new FormLogin();
            loginForm.Show();
            
            // Cerrar el splash cuando se cierre el login
            loginForm.FormClosed += (s, args) => this.Close();
        }
    }
}