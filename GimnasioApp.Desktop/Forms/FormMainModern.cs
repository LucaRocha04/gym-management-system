using GimnasioApp.Models;
using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormMainModern : Form
    {
        private readonly Usuario _currentUser;
        private Panel mainPanel;
        private Label lblWelcome;
        private FlowLayoutPanel tilesContainer;
        private Button btnCerrarSesion;

        public FormMainModern(Usuario usuario)
        {
            _currentUser = usuario;
            InitializeComponent();
            UITheme.Apply(this);
            CreateDashboard();
            ConfigureMenuByRole();
        }

        private void InitializeComponent()
        {
            this.Text = "Sistema de Gestión - Gimnasio";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 252);
            this.WindowState = FormWindowState.Maximized;

            // Panel principal
            this.mainPanel = new Panel();
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.BackColor = Color.FromArgb(248, 249, 252);
            this.mainPanel.Padding = new Padding(30);

            // Label de bienvenida
            this.lblWelcome = new Label();
            this.lblWelcome.Text = $"Bienvenido/a: {_currentUser.NombreUsuario} ({_currentUser.Rol})";
            this.lblWelcome.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblWelcome.ForeColor = Color.FromArgb(44, 62, 80);
            this.lblWelcome.Location = new Point(30, 30);
            this.lblWelcome.Size = new Size(800, 40);

            // Botón Cerrar Sesión (Mejorado estéticamente)
            this.btnCerrarSesion = new Button();
            this.btnCerrarSesion.Text = "🚪 Cerrar Sesión";
            this.btnCerrarSesion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.btnCerrarSesion.BackColor = Color.FromArgb(220, 53, 69); // Rojo Bootstrap
            this.btnCerrarSesion.ForeColor = Color.White;
            this.btnCerrarSesion.FlatStyle = FlatStyle.Flat;
            this.btnCerrarSesion.FlatAppearance.BorderSize = 0;
            this.btnCerrarSesion.FlatAppearance.BorderColor = Color.FromArgb(200, 35, 51);
            this.btnCerrarSesion.Size = new Size(170, 42);
            this.btnCerrarSesion.Location = new Point(this.Width - 200, 29);
            this.btnCerrarSesion.Cursor = Cursors.Hand;
            this.btnCerrarSesion.Click += BtnCerrarSesion_Click;

            // Efectos hover para el botón
            this.btnCerrarSesion.MouseEnter += (s, e) => {
                this.btnCerrarSesion.BackColor = Color.FromArgb(200, 35, 51); // Rojo más oscuro al hover
                this.btnCerrarSesion.FlatAppearance.BorderSize = 1;
            };
            
            this.btnCerrarSesion.MouseLeave += (s, e) => {
                this.btnCerrarSesion.BackColor = Color.FromArgb(220, 53, 69); // Volver al color original
                this.btnCerrarSesion.FlatAppearance.BorderSize = 0;
            };

            this.btnCerrarSesion.MouseDown += (s, e) => {
                this.btnCerrarSesion.BackColor = Color.FromArgb(176, 27, 41); // Aún más oscuro al presionar
            };

            this.btnCerrarSesion.MouseUp += (s, e) => {
                this.btnCerrarSesion.BackColor = Color.FromArgb(200, 35, 51); // Volver al hover
            };
            
            // Reposicionar cuando la ventana cambie de tamaño
            this.Resize += (s, e) => {
                this.btnCerrarSesion.Location = new Point(this.Width - 200, 30);
            };

            // Contenedor de tiles (ajustado para tiles más grandes)
            this.tilesContainer = new FlowLayoutPanel();
            this.tilesContainer.Location = new Point(30, 80);
            this.tilesContainer.Size = new Size(1200, 700); // Más espacio
            this.tilesContainer.FlowDirection = FlowDirection.LeftToRight;
            this.tilesContainer.WrapContents = true;
            this.tilesContainer.Padding = new Padding(10);
            this.tilesContainer.AutoScroll = true;

            this.mainPanel.Controls.Add(this.lblWelcome);
            this.mainPanel.Controls.Add(this.btnCerrarSesion);
            this.mainPanel.Controls.Add(this.tilesContainer);
            this.Controls.Add(this.mainPanel);
        }

        private void CreateDashboard()
        {
            // Crear tiles según el rol del usuario
            var tiles = GetTilesByRole();
            
            foreach (var tile in tiles)
            {
                var tileButton = CreateTileButton(tile.Title, tile.Description, tile.Icon, tile.Color, tile.Action);
                tilesContainer.Controls.Add(tileButton);
            }
        }

        private Panel CreateTileButton(string title, string description, string icon, Color color, Action clickAction)
        {
            var tile = new Panel();
            tile.Size = new Size(300, 180); // Tamaño aumentado
            tile.BackColor = Color.White;
            tile.Margin = new Padding(15);
            tile.Cursor = Cursors.Hand;
            
            // Efecto de sombra simulado con un borde
            tile.Paint += (s, e) => {
                var rect = new Rectangle(0, 0, tile.Width - 1, tile.Height - 1);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(230, 230, 230), 1), rect);
            };

            // Panel de color superior (más alto)
            var colorBar = new Panel();
            colorBar.BackColor = color;
            colorBar.Dock = DockStyle.Top;
            colorBar.Height = 8; // Más alto

            // Icono/Emoji (más grande)
            var lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 32F); // Más grande
            lblIcon.Location = new Point(20, 30);
            lblIcon.Size = new Size(70, 60); // Más espacio
            lblIcon.ForeColor = color;

            // Título (con más espacio y fuente más grande)
            var lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold); // Fuente más grande
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(100, 35);
            lblTitle.Size = new Size(185, 35); // Más espacio en altura
            lblTitle.AutoEllipsis = true; // Para mostrar ... si es muy largo

            // Descripción (con más espacio)
            var lblDescription = new Label();
            lblDescription.Text = description;
            lblDescription.Font = new Font("Segoe UI", 10F); // Fuente ligeramente más grande
            lblDescription.ForeColor = Color.FromArgb(127, 140, 141);
            lblDescription.Location = new Point(20, 105);
            lblDescription.Size = new Size(260, 60); // Mucho más espacio
            lblDescription.AutoEllipsis = true; // Para mostrar ... si es muy largo

            // Eventos de hover
            tile.MouseEnter += (s, e) => {
                tile.BackColor = Color.FromArgb(250, 250, 250);
            };
            
            tile.MouseLeave += (s, e) => {
                tile.BackColor = Color.White;
            };

            // Evento click
            tile.Click += (s, e) => clickAction?.Invoke();
            lblIcon.Click += (s, e) => clickAction?.Invoke();
            lblTitle.Click += (s, e) => clickAction?.Invoke();
            lblDescription.Click += (s, e) => clickAction?.Invoke();

            tile.Controls.Add(colorBar);
            tile.Controls.Add(lblIcon);
            tile.Controls.Add(lblTitle);
            tile.Controls.Add(lblDescription);

            return tile;
        }

        private List<TileInfo> GetTilesByRole()
        {
            var tiles = new List<TileInfo>();

            // ========== ADMINISTRADOR - ACCESO COMPLETO ==========
            if (_currentUser.Rol == "Administrador")
            {
                // Gestión de socios
                tiles.Add(new TileInfo
                {
                    Title = "Socios",
                    Description = "Gestión completa de socios",
                    Icon = "👥",
                    Color = Color.FromArgb(52, 152, 219),
                    Action = () => new FormSocios().ShowDialog()
                });

                // Gestión de planes
                tiles.Add(new TileInfo
                {
                    Title = "Planes",
                    Description = "Crear y editar planes",
                    Icon = "📋",
                    Color = Color.FromArgb(155, 89, 182),
                    Action = () => new FormPlanes().ShowDialog()
                });

                // Gestión de pagos
                tiles.Add(new TileInfo
                {
                    Title = "Pagos",
                    Description = "Registro y control de pagos",
                    Icon = "💳",
                    Color = Color.FromArgb(46, 204, 113),
                    Action = () => new FormPagos().ShowDialog()
                });

                // Control de asistencias
                tiles.Add(new TileInfo
                {
                    Title = "Asistencias",
                    Description = "Control de asistencia",
                    Icon = "📅",
                    Color = Color.FromArgb(241, 196, 15),
                    Action = () => new FormAsistencias().ShowDialog()
                });

                // Reportes completos
                tiles.Add(new TileInfo
                {
                    Title = "Reportes",
                    Description = "Estadísticas e informes",
                    Icon = "📊",
                    Color = Color.FromArgb(230, 126, 34),
                    Action = () => new FormReportes().ShowDialog()
                });

                // Gestión de usuarios (solo admin)
                tiles.Add(new TileInfo
                {
                    Title = "Usuarios",
                    Description = "Administrar usuarios",
                    Icon = "�",
                    Color = Color.FromArgb(231, 76, 60),
                    Action = () => new FormUsuarios().ShowDialog()
                });

                // Clases como profesor
                tiles.Add(new TileInfo
                {
                    Title = "Clases",
                    Description = "Gestionar clases del gym",
                    Icon = "🏃",
                    Color = Color.FromArgb(26, 188, 156),
                    Action = () => new FormClasesProfesor(_currentUser).ShowDialog()
                });

                // Inscripciones
                tiles.Add(new TileInfo
                {
                    Title = "Inscripciones",
                    Description = "Inscribir en clases",
                    Icon = "📝",
                    Color = Color.FromArgb(52, 73, 94),
                    Action = () => new FormInscripcionesClases().ShowDialog()
                });

                // Configuración de Email
                tiles.Add(new TileInfo
                {
                    Title = "Config Email",
                    Description = "Configurar notificaciones",
                    Icon = "📧",
                    Color = Color.FromArgb(142, 68, 173),
                    Action = () => new FormConfiguracionEmail().ShowDialog()
                });
            }

            // ========== RECEPCIONISTA - OPERACIONES DIARIAS ==========
            else if (_currentUser.Rol == "Recepcionista")
            {
                // Gestión básica de socios (consulta y registro básico)
                tiles.Add(new TileInfo
                {
                    Title = "Socios",
                    Description = "Consultar y registrar socios",
                    Icon = "👥",
                    Color = Color.FromArgb(52, 152, 219),
                    Action = () => new FormSocios().ShowDialog()
                });

                // Registro de pagos
                tiles.Add(new TileInfo
                {
                    Title = "Pagos",
                    Description = "Registrar pagos de socios",
                    Icon = "💳",
                    Color = Color.FromArgb(46, 204, 113),
                    Action = () => new FormPagos().ShowDialog()
                });

                // Control de asistencias
                tiles.Add(new TileInfo
                {
                    Title = "Asistencias",
                    Description = "Control de asistencia",
                    Icon = "📅",
                    Color = Color.FromArgb(241, 196, 15),
                    Action = () => new FormAsistencias().ShowDialog()
                });

                // Inscripciones
                tiles.Add(new TileInfo
                {
                    Title = "Inscripciones",
                    Description = "Inscribir en clases",
                    Icon = "📝",
                    Color = Color.FromArgb(52, 73, 94),
                    Action = () => new FormInscripcionesClases().ShowDialog()
                });

                // Ver planes (consulta)
                tiles.Add(new TileInfo
                {
                    Title = "Ver Planes",
                    Description = "Consultar planes disponibles",
                    Icon = "❓",
                    Color = Color.FromArgb(155, 89, 182),
                    Action = () => new FormPlanesConsulta().ShowDialog()
                });
            }

            // ========== PROFESOR - SOLO CONSULTAS ==========
            else if (_currentUser.Rol == "Profesor")
            {
                // Solo sus clases asignadas
                tiles.Add(new TileInfo
                {
                    Title = "Mis Clases",
                    Description = "Gestionar mis clases",
                    Icon = "🏃",
                    Color = Color.FromArgb(26, 188, 156),
                    Action = () => new FormClasesProfesor(_currentUser).ShowDialog()
                });

                // Herramientas del profesor (IMC y rutinas)
                tiles.Add(new TileInfo
                {
                    Title = "Herramientas Fitness",
                    Description = "IMC y guías de entrenamiento",
                    Icon = "🧮",
                    Color = Color.FromArgb(142, 68, 173),
                    Action = () => new FormHerramientasProfesor(_currentUser).ShowDialog()
                });

                // Ver socios activos (solo lectura)
                tiles.Add(new TileInfo
                {
                    Title = "Ver Socios",
                    Description = "Consultar socios activos",
                    Icon = "👥",
                    Color = Color.FromArgb(52, 152, 219),
                    Action = () => new FormSociosActivos().ShowDialog()
                });

                // Ver planes (solo lectura)
                tiles.Add(new TileInfo
                {
                    Title = "Ver Planes",
                    Description = "Consultar planes disponibles",
                    Icon = "�",
                    Color = Color.FromArgb(155, 89, 182),
                    Action = () => new FormPlanesConsulta().ShowDialog()
                });
            }

            // ========== ACCESO RÁPIDO PARA TODOS LOS ROLES ==========
            if (_currentUser.Rol == "Administrador" || _currentUser.Rol == "Recepcionista")
            {
                tiles.Add(new TileInfo
                {
                    Title = "Entrada",
                    Description = "Registrar entrada rápida",
                    Icon = "🚪",
                    Color = Color.FromArgb(22, 160, 133),
                    Action = () => new FormEntradaRapida().ShowDialog()
                });

                tiles.Add(new TileInfo
                {
                    Title = "Salida Rápida", 
                    Description = "Registrar salida rápida",
                    Icon = "🚪",
                    Color = Color.FromArgb(192, 57, 43),
                    Action = () => new FormSalidaRapida().ShowDialog()
                });
            }

            return tiles;
        }

        private void ConfigureMenuByRole()
        {
            // Mantener el menú superior para funciones adicionales si es necesario
        }

        private void BtnCerrarSesion_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "¿Está seguro que desea cerrar la sesión actual?",
                "Cerrar Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Reiniciar la aplicación para volver al login
                Application.Restart();
                Environment.Exit(0);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Application.Exit();
            base.OnFormClosed(e);
        }
    }

    public class TileInfo
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Icon { get; set; } = "";
        public Color Color { get; set; }
        public Action? Action { get; set; }
    }
}