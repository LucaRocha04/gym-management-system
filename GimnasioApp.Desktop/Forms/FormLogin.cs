using GimnasioApp.Managers;
using GimnasioApp.Models;
using GimnasioApp.Connection;
using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormLogin : Form
    {
        private readonly UsuarioManager _usuarioManager = new();
        private int _intentos = 0;
        private const int MAX_INTENTOS = 3;

        public FormLogin()
        {
            InitializeComponent();
            UITheme.Apply(this);
        }

        private async void FormLogin_Load(object sender, EventArgs e)
        {
            // Probar conexión a la base de datos
            if (!await DatabaseConnection.TestConnectionAsync())
            {
                MessageBox.Show("Error al conectar con la base de datos.\nVerifique la configuración.", 
                    "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string userOrMail = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(userOrMail) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Ingrese usuario y contraseña", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var (ok, usuario) = await _usuarioManager.ValidateLoginAsync(userOrMail, password);
                
                if (ok && usuario != null)
                {
                    // Login exitoso
                    this.Hide();
                    var formMain = new FormMainModern(usuario);
                    formMain.FormClosed += (s, args) => Application.Exit();
                    formMain.Show();
                }
                else
                {
                    _intentos++;
                    int restantes = MAX_INTENTOS - _intentos;
                    
                    if (restantes > 0)
                    {
                        MessageBox.Show($"Credenciales inválidas.\nIntentos restantes: {restantes}", 
                            "Error de Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPassword.Clear();
                        txtUsuario.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Superó el número máximo de intentos.\nLa aplicación se cerrará.", 
                            "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnLogin.PerformClick();
            }
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                txtPassword.Focus();
            }
        }
    }
}
