using GimnasioApp.Models;
using GimnasioApp.Desktop.Theme;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

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
            this.WindowState = FormWindowState.Maximized;
            
            // Configurar imagen de fondo
            SetBackgroundImage();

            // Panel principal
            this.mainPanel = new Panel();
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.BackColor = Color.Transparent; // Completamente transparente para mostrar fondo
            this.mainPanel.Padding = new Padding(30);

            // Label de bienvenida
            this.lblWelcome = new Label();
            this.lblWelcome.Text = $"Bienvenido/a: {_currentUser.NombreUsuario} ({_currentUser.Rol})";
            this.lblWelcome.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblWelcome.ForeColor = Color.FromArgb(44, 62, 80);
            this.lblWelcome.Location = new Point(30, 30);
            this.lblWelcome.Size = new Size(800, 40);

            // Crear un campo placeholder para el botón cerrar sesión (ya no se usa aquí)
            this.btnCerrarSesion = new Button(); // Solo para evitar errores, no se agrega al form

            // Contenedor de tiles (para layout específico del admin y profesor)
            if (_currentUser.Rol == "Administrador")
            {
                // Para admin: layout específico de 3x3 + 1 centrada
                this.tilesContainer = new FlowLayoutPanel();
                this.tilesContainer.Dock = DockStyle.None;
                this.tilesContainer.Anchor = AnchorStyles.None;
                this.tilesContainer.Location = new Point(50, 80);
                this.tilesContainer.Size = new Size(1050, 720); // Aumentado para mostrar la 4ta fila completa
                this.tilesContainer.FlowDirection = FlowDirection.LeftToRight;
                this.tilesContainer.WrapContents = true;
                this.tilesContainer.Padding = new Padding(0);
                this.tilesContainer.AutoScroll = false; // Sin scroll para admin
            }
            else if (_currentUser.Rol == "Profesor")
            {
                // Para profesor: layout específico de 2 + 2 + 1 centrada
                this.tilesContainer = new FlowLayoutPanel();
                this.tilesContainer.Dock = DockStyle.None;
                this.tilesContainer.Anchor = AnchorStyles.None;
                this.tilesContainer.Location = new Point(200, 40); // Posición inicial más arriba
                this.tilesContainer.Size = new Size(800, 650); // Aumentado altura para mostrar la tarjeta completa sin cortes
                this.tilesContainer.FlowDirection = FlowDirection.LeftToRight;
                this.tilesContainer.WrapContents = true;
                this.tilesContainer.Padding = new Padding(20);
                this.tilesContainer.AutoScroll = false; // Sin scroll para profesor
            }
            else
            {
                // Para otros roles: layout centrado normal
                this.tilesContainer = new FlowLayoutPanel();
                this.tilesContainer.Dock = DockStyle.None;
                this.tilesContainer.Anchor = AnchorStyles.None;
                this.tilesContainer.Location = new Point(50, 80);
                this.tilesContainer.Size = new Size(1100, 700);
                this.tilesContainer.FlowDirection = FlowDirection.LeftToRight;
                this.tilesContainer.WrapContents = true;
                this.tilesContainer.Padding = new Padding(20);
                this.tilesContainer.AutoScroll = true;
            }
            
            // Centrar el contenedor en el formulario
            this.Resize += (s, e) => {
                var formWidth = this.ClientSize.Width;
                var containerWidth = this.tilesContainer.Width;
                var formHeight = this.ClientSize.Height;
                var containerHeight = this.tilesContainer.Height;
                
                // Centrar horizontal y verticalmente (subiendo más)
                this.tilesContainer.Location = new Point(
                    (formWidth - containerWidth) / 2, 
                    (formHeight - containerHeight) / 2 - 50  // -50 para subir más las tarjetas
                );
            };

            this.mainPanel.Controls.Add(this.lblWelcome);
            this.mainPanel.Controls.Add(this.tilesContainer);
            this.Controls.Add(this.mainPanel);
        }

        private void CreateDashboard()
        {
            // Crear tiles según el rol del usuario
            var tiles = GetTilesByRole();
            
            if (_currentUser.Rol == "Administrador")
            {
                // Layout específico para Administrador: 3x3 + 1 centrada
                CreateAdminLayout(tiles);
            }
            else if (_currentUser.Rol == "Profesor")
            {
                // Layout específico para Profesor: 2 tarjetas arriba + 1 centrada abajo
                CreateProfesorLayout(tiles);
            }
            else
            {
                // Layout normal para otros roles
                foreach (var tile in tiles)
                {
                    var tileButton = CreateTileButton(tile.Title, tile.Description, tile.Icon, tile.Color, tile.Action);
                    tilesContainer.Controls.Add(tileButton);
                }
            }
        }

        private void CreateProfesorLayout(List<TileInfo> tiles)
        {
            // Las primeras 2 tarjetas en la primera fila
            for (int i = 0; i < Math.Min(2, tiles.Count - 1); i++) // -1 para dejar la última para el final
            {
                var tile = tiles[i];
                var tileButton = CreateTileButton(tile.Title, tile.Description, tile.Icon, tile.Color, tile.Action);
                
                // Ajustar márgenes para layout de profesor
                tileButton.Margin = new Padding(25, 15, 25, 15);
                tilesContainer.Controls.Add(tileButton);
            }
            
            // Las tarjetas restantes (excepto la última) en filas normales
            for (int i = 2; i < tiles.Count - 1; i++)
            {
                var tile = tiles[i];
                var tileButton = CreateTileButton(tile.Title, tile.Description, tile.Icon, tile.Color, tile.Action);
                tileButton.Margin = new Padding(25, 15, 25, 15);
                tilesContainer.Controls.Add(tileButton);
            }
            
            // La tarjeta de cerrar (última) centrada en una nueva fila
            if (tiles.Count > 0)
            {
                var cerrarTile = tiles[tiles.Count - 1]; // Última tarjeta (Cerrar)
                var cerrarButton = CreateTileButton(cerrarTile.Title, cerrarTile.Description, cerrarTile.Icon, cerrarTile.Color, cerrarTile.Action);
                
                // Crear un panel contenedor para centrar la última tarjeta
                var cerrarContainer = new Panel();
                cerrarContainer.Size = new Size(800, 210); // Altura aumentada para mostrar la tarjeta completa
                cerrarContainer.BackColor = Color.Transparent;
                
                // Centrar la tarjeta de cerrar dentro del contenedor
                cerrarButton.Location = new Point((cerrarContainer.Width - cerrarButton.Width) / 2, 20);
                cerrarButton.Margin = new Padding(0);
                cerrarContainer.Controls.Add(cerrarButton);
                
                // Forzar que el contenedor ocupe toda una fila
                tilesContainer.SetFlowBreak(cerrarContainer, true);
                tilesContainer.Controls.Add(cerrarContainer);
            }
        }

        private void CreateAdminLayout(List<TileInfo> tiles)
        {
            // Las primeras 9 tarjetas en 3 filas de 3
            for (int i = 0; i < Math.Min(9, tiles.Count - 1); i++) // -1 para dejar la última para el final
            {
                var tile = tiles[i];
                var tileButton = CreateTileButton(tile.Title, tile.Description, tile.Icon, tile.Color, tile.Action);
                
                // Ajustar márgenes para layout de administrador
                tileButton.Margin = new Padding(15, 10, 15, 10);
                tilesContainer.Controls.Add(tileButton);
            }
            
            // La tarjeta de cerrar (última) centrada en una nueva fila
            if (tiles.Count > 0)
            {
                var cerrarTile = tiles[tiles.Count - 1]; // Última tarjeta (Cerrar)
                var cerrarButton = CreateTileButton(cerrarTile.Title, cerrarTile.Description, cerrarTile.Icon, cerrarTile.Color, cerrarTile.Action);
                
                // Crear un panel contenedor para centrar la última tarjeta
                var cerrarContainer = new Panel();
                cerrarContainer.Size = new Size(1050, 180); // Altura suficiente para mostrar la tarjeta completa
                cerrarContainer.BackColor = Color.Transparent;
                
                // Centrar la tarjeta de cerrar dentro del contenedor
                cerrarButton.Location = new Point((cerrarContainer.Width - cerrarButton.Width) / 2, 15);
                cerrarButton.Margin = new Padding(0);
                cerrarContainer.Controls.Add(cerrarButton);
                
                // Forzar que el contenedor ocupe toda una fila
                tilesContainer.SetFlowBreak(cerrarContainer, true);
                tilesContainer.Controls.Add(cerrarContainer);
            }
        }

        private Panel CreateTileButton(string title, string description, string icon, Color color, Action? clickAction)
        {
            var tile = new Panel();
            
            // Tamaño ajustado para el layout de administrador
            if (_currentUser.Rol == "Administrador")
            {
                tile.Size = new Size(320, 150); // Ligeramente más compacto para caber 3 filas
                tile.Margin = new Padding(15, 10, 15, 10);
            }
            else if (_currentUser.Rol == "Profesor")
            {
                tile.Size = new Size(320, 180); // Tamaño normal para profesor
                tile.Margin = new Padding(25, 15, 25, 15);
            }
            else
            {
                tile.Size = new Size(320, 180); // Tamaño normal para otros roles
                tile.Margin = new Padding(25, 15, 25, 15);
            }
            
            tile.BackColor = Color.White;
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
                    Icon = "📋",
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
                    Icon = "📋",
                    Color = Color.FromArgb(155, 89, 182),
                    Action = () => new FormPlanesConsulta().ShowDialog()
                });
            }

            // ========== TARJETA CERRAR SESIÓN PARA TODOS LOS ROLES ==========
            tiles.Add(new TileInfo
            {
                Title = "Cerrar",
                Description = "Salir del sistema",
                Icon = "🚪",
                Color = Color.FromArgb(220, 53, 69), // Rojo Bootstrap
                Action = () => CerrarSesion()
            });

            return tiles;
        }

        private void ConfigureMenuByRole()
        {
            // Mantener el menú superior para funciones adicionales si es necesario
        }

        private void CerrarSesion()
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

        private void SetBackgroundImage()
        {
            try
            {
                // Limpiar imagen de fondo anterior si existe
                if (this.BackgroundImage != null)
                {
                    this.BackgroundImage.Dispose();
                    this.BackgroundImage = null;
                }

                // Intentar varias rutas posibles para la imagen
                string[] possiblePaths = {
                    Path.Combine(Application.StartupPath, "Resources", "gym-background.jpg"),
                    Path.Combine(Application.StartupPath, "Resources", "gym-background.png"),
                    Path.Combine(Application.StartupPath, "gym-background.jpg"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources", "gym-background.jpg"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources", "gym-background.png"),
                    Path.Combine(Directory.GetCurrentDirectory(), "gym-background.jpg")
                };
                
                string? foundImagePath = null;
                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundImagePath = path;
                        break;
                    }
                }
                
                if (foundImagePath != null)
                {
                    // Cargar y configurar la imagen de fondo
                    using (var originalImage = Image.FromFile(foundImagePath))
                    {
                        // Crear una copia optimizada para evitar bloqueo de archivo
                        var backgroundImage = new Bitmap(originalImage, Math.Min(800, originalImage.Width), Math.Min(600, originalImage.Height));
                        this.BackgroundImage = backgroundImage;
                        this.BackgroundImageLayout = ImageLayout.Stretch;
                        // Asegurar que el fondo se vea removiendo colores de fondo
                        this.BackColor = Color.Transparent;
                    }
                }
                else
                {
                    // Usar solo color sólido para evitar problemas de memoria
                    CreateSimpleBackground();
                }
            }
            catch (OutOfMemoryException)
            {
                // Error específico de memoria - usar fondo simple
                CreateSimpleBackground();
            }
            catch (Exception ex)
            {
                // En caso de cualquier error, usar fondo simple
                CreateSimpleBackground();
            }
        }
        
        private void CreateGradientBackground()
        {
            try
            {
                // Crear un gradiente elegante usando Paint event (más eficiente)
                this.BackgroundImage = null;
                this.BackColor = Color.FromArgb(22, 25, 28);
                
                // Remover eventos anteriores para evitar duplicados
                this.Paint -= FormMainModern_Paint;
                this.Paint += FormMainModern_Paint;
            }
            catch
            {
                // Si falla, usar fondo simple
                CreateSimpleBackground();
            }
        }

        private void CreateSimpleBackground()
        {
            // Fondo ultra simple - solo color sólido oscuro de gimnasio
            this.BackgroundImage = null;
            this.BackColor = Color.FromArgb(22, 25, 28);  // Color sólido oscuro de gimnasio
            
            // Remover el evento Paint para que no interfiera
            this.Paint -= FormMainModern_Paint;
        }
        
        private void FormMainModern_Paint(object? sender, PaintEventArgs e)
        {
            try
            {
                // Crear un gradiente elegante pero liviano
                using (var brush = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(32, 36, 40),  // Más claro arriba
                    Color.FromArgb(18, 20, 22),  // Más oscuro abajo
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
                
                // Agregar líneas sutiles de textura de gimnasio
                using (var pen = new Pen(Color.FromArgb(8, 255, 165, 0), 1)) // Naranja muy tenue
                {
                    // Líneas diagonales muy sutiles cada 100 pixels
                    for (int i = -this.Height; i < this.Width + this.Height; i += 100)
                    {
                        e.Graphics.DrawLine(pen, i, 0, i - this.Height, this.Height);
                    }
                }
            }
            catch
            {
                // Si algo falla, usar color sólido
                using (var brush = new SolidBrush(Color.FromArgb(22, 25, 28)))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
        }

        private void BtnCerrarSesion_Click(object? sender, EventArgs e)
        {
            // Método legacy - ahora redirige al nuevo método
            CerrarSesion();
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