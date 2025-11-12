using GimnasioApp.Models;
using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormMain : Form
    {
        private readonly Usuario _currentUser;

        public FormMain(Usuario usuario)
        {
            _currentUser = usuario;
            InitializeComponent();
            UITheme.Apply(this);
            ConfigureMenuByRole();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = $"Bienvenido/a: {_currentUser.NombreUsuario} ({_currentUser.Rol})";
        }

        private void ConfigureMenuByRole()
        {
            // Ocultar todos los menús inicialmente
            sociosToolStripMenuItem.Visible = false;
            planesToolStripMenuItem.Visible = false;
            pagosToolStripMenuItem.Visible = false;
            asistenciasToolStripMenuItem.Visible = false;
            reportesToolStripMenuItem.Visible = false;
            usuariosToolStripMenuItem.Visible = false;
            clasesToolStripMenuItem.Visible = false;
            accionesRapidasToolStripMenuItem.Visible = false;
            consultasToolStripMenuItem.Visible = false;

            switch (_currentUser.Rol)
            {
                case "Administrador":
                    sociosToolStripMenuItem.Visible = true;
                    planesToolStripMenuItem.Visible = true;
                    pagosToolStripMenuItem.Visible = true;
                    asistenciasToolStripMenuItem.Visible = true;
                    reportesToolStripMenuItem.Visible = true;
                    usuariosToolStripMenuItem.Visible = true;
                    clasesToolStripMenuItem.Visible = true; // Admin ve todo
                    break;

                case "Recepcionista":
                    sociosToolStripMenuItem.Visible = true;
                    pagosToolStripMenuItem.Visible = true;
                    asistenciasToolStripMenuItem.Visible = true;
                    accionesRapidasToolStripMenuItem.Visible = true;
                    clasesToolStripMenuItem.Visible = true;
                    gestionarMisClasesToolStripMenuItem.Visible = false; // solo inscripción
                    inscribirEnClasesToolStripMenuItem.Visible = true;
                    break;

                case "Profesor":
                    consultasToolStripMenuItem.Visible = true;
                    clasesToolStripMenuItem.Visible = true;
                    gestionarMisClasesToolStripMenuItem.Visible = true;
                    inscribirEnClasesToolStripMenuItem.Visible = false;
                    break;
            }
        }

        // Menú Socios
        private void gestionarSociosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormSocios();
            form.ShowDialog();
        }

        // Menú Planes
        private void gestionarPlanesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormPlanes();
            form.ShowDialog();
        }

        // Menú Pagos
        private void registrarPagoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormPagos();
            form.ShowDialog();
        }

        // Menú Asistencias
        private void registrarAsistenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormAsistencias();
            form.ShowDialog();
        }

        // Menú Reportes
        private void verReportesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormReportes();
            form.ShowDialog();
        }

        // Menú Usuarios
        private void gestionarUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormUsuarios();
            form.ShowDialog();
        }

        // Acciones Rápidas (Recepcionista)
        private void registrarEntradaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormEntradaRapida();
            form.ShowDialog();
        }

        private void registrarSalidaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormSalidaRapida();
            form.ShowDialog();
        }

        // Consultas (Profesor)
        private void verSociosActivosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormSociosActivos();
            form.ShowDialog();
        }

        private void verPlanesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormPlanesConsulta();
            form.ShowDialog();
        }

        private void gestionarMisClasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormClasesProfesor(_currentUser);
            form.ShowDialog();
        }

        private void inscribirEnClasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormInscripcionesClases();
            form.ShowDialog();
        }

        // Salir
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
