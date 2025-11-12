using GimnasioApp.Managers;
using GimnasioApp.Models;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormAsistencias : Form
    {
        private readonly AsistenciaManager _asistenciaManager = new();
        private readonly SocioManager _socioManager = new();
        private DataGridView dgvAsistencias;
        private Button btnEntrada, btnSalida, btnHoy, btnPorSocio, btnRefrescar;
        private Label lblTotal;
        private Panel panelTop, panelBottom;
        private TextBox txtBuscar;
        private DateTimePicker dtpFecha;

        public FormAsistencias()
        {
            InitializeComponent();
        }

        private async void FormAsistencias_Load(object? sender, EventArgs e)
        {
            await CargarAsistenciasHoy();
        }

        private async Task CargarAsistenciasHoy()
        {
            try
            {
                var asistencias = await _asistenciaManager.GetAsistenciasByDateAsync(DateTime.Today);
                await MostrarAsistencias(asistencias, "Asistencias de Hoy");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task MostrarAsistencias(List<Asistencia> asistencias, string titulo)
        {
            var asistenciasConSocio = new List<dynamic>();

            foreach (var a in asistencias)
            {
                var socio = await _socioManager.GetByIdAsync(a.SocioId);
                asistenciasConSocio.Add(new
                {
                    ID = a.IdAsistencia,
                    Socio = socio != null ? $"{socio.Nombre} {socio.Apellido}" : "Desconocido",
                    DNI = socio?.DNI ?? "",
                    Fecha = a.Fecha,
                    Entrada = a.HoraEntrada,
                    Salida = a.HoraSalida?.ToString(@"hh\:mm") ?? "En gimnasio",
                    Observaciones = a.Observaciones ?? ""
                });
            }

            dgvAsistencias.DataSource = null;
            dgvAsistencias.DataSource = asistenciasConSocio;

            if (dgvAsistencias.Columns.Count > 0)
            {
                dgvAsistencias.Columns["ID"]!.Width = 50;
                dgvAsistencias.Columns["Fecha"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvAsistencias.Columns["Fecha"]!.Width = 100;
                dgvAsistencias.Columns["Entrada"]!.Width = 80;
                dgvAsistencias.Columns["Salida"]!.Width = 100;
            }

            lblTotal.Text = $"{titulo} - Total: {asistenciasConSocio.Count}";
        }

        private void btnEntrada_Click(object? sender, EventArgs e)
        {
            using var form = new FormRegistroAsistencia(true);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _ = CargarAsistenciasHoy();
            }
        }

        private void btnSalida_Click(object? sender, EventArgs e)
        {
            using var form = new FormRegistroAsistencia(false);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _ = CargarAsistenciasHoy();
            }
        }

        private async void btnHoy_Click(object? sender, EventArgs e)
        {
            await CargarAsistenciasHoy();
        }

        private async void btnPorSocio_Click(object? sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text.Trim();
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                MessageBox.Show("Ingrese DNI o nombre del socio", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var socios = await _socioManager.GetAllAsync();
                var socio = socios.FirstOrDefault(s =>
                    s.DNI.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    s.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    s.Apellido.Contains(busqueda, StringComparison.OrdinalIgnoreCase));

                if (socio == null)
                {
                    MessageBox.Show("Socio no encontrado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var asistencias = await _asistenciaManager.GetAsistenciasBySocioAsync(socio.Id);
                await MostrarAsistencias(asistencias, $"Asistencias de {socio.Nombre} {socio.Apellido}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRefrescar_Click(object? sender, EventArgs e)
        {
            await CargarAsistenciasHoy();
        }

        private void InitializeComponent()
        {
            this.dgvAsistencias = new DataGridView();
            this.btnEntrada = new Button();
            this.btnSalida = new Button();
            this.btnHoy = new Button();
            this.btnPorSocio = new Button();
            this.btnRefrescar = new Button();
            this.lblTotal = new Label();
            this.panelTop = new Panel();
            this.panelBottom = new Panel();
            this.txtBuscar = new TextBox();
            this.dtpFecha = new DateTimePicker();
            var lblBuscar = new Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsistencias)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // panelTop
            this.panelTop.BackColor = Color.FromArgb(245, 245, 245);
            this.panelTop.Controls.Add(lblBuscar);
            this.panelTop.Controls.Add(this.txtBuscar);
            this.panelTop.Controls.Add(this.btnPorSocio);
            this.panelTop.Controls.Add(this.btnHoy);
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Size = new Size(900, 60);

            lblBuscar.Location = new Point(15, 18); lblBuscar.Size = new Size(100, 20); lblBuscar.Text = "Buscar socio:";
            this.txtBuscar.Location = new Point(120, 15); this.txtBuscar.Size = new Size(250, 23);
            this.btnPorSocio.Location = new Point(380, 13); this.btnPorSocio.Size = new Size(140, 32); this.btnPorSocio.Text = "🔍 Ver Historial";
            this.btnPorSocio.BackColor = Color.FromArgb(33, 150, 243); this.btnPorSocio.ForeColor = Color.White; this.btnPorSocio.FlatStyle = FlatStyle.Flat;
            this.btnPorSocio.Click += btnPorSocio_Click;

            this.btnHoy.Location = new Point(530, 13); this.btnHoy.Size = new Size(120, 32); this.btnHoy.Text = "📅 Hoy";
            this.btnHoy.BackColor = Color.FromArgb(103, 58, 181); this.btnHoy.ForeColor = Color.White; this.btnHoy.FlatStyle = FlatStyle.Flat;
            this.btnHoy.Click += btnHoy_Click;

            // dgvAsistencias
            this.dgvAsistencias.AllowUserToAddRows = false;
            this.dgvAsistencias.AllowUserToDeleteRows = false;
            this.dgvAsistencias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAsistencias.BackgroundColor = Color.White;
            this.dgvAsistencias.Dock = DockStyle.Fill;
            this.dgvAsistencias.ReadOnly = true;
            this.dgvAsistencias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // panelBottom
            this.panelBottom.BackColor = Color.FromArgb(245, 245, 245);
            this.panelBottom.Controls.Add(this.btnEntrada);
            this.panelBottom.Controls.Add(this.btnSalida);
            this.panelBottom.Controls.Add(this.btnRefrescar);
            this.panelBottom.Controls.Add(this.lblTotal);
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Size = new Size(900, 60);

            this.btnEntrada.Location = new Point(15, 14); this.btnEntrada.Size = new Size(150, 34); this.btnEntrada.Text = "🟢 Registrar Entrada";
            this.btnEntrada.BackColor = Color.FromArgb(76, 175, 80); this.btnEntrada.ForeColor = Color.White; this.btnEntrada.FlatStyle = FlatStyle.Flat;
            this.btnEntrada.Click += btnEntrada_Click;

            this.btnSalida.Location = new Point(175, 14); this.btnSalida.Size = new Size(150, 34); this.btnSalida.Text = "🔴 Registrar Salida";
            this.btnSalida.BackColor = Color.FromArgb(255, 87, 34); this.btnSalida.ForeColor = Color.White; this.btnSalida.FlatStyle = FlatStyle.Flat;
            this.btnSalida.Click += btnSalida_Click;

            this.btnRefrescar.Location = new Point(335, 14); this.btnRefrescar.Size = new Size(110, 34); this.btnRefrescar.Text = "🔄 Refrescar";
            this.btnRefrescar.BackColor = Color.FromArgb(158, 158, 158); this.btnRefrescar.ForeColor = Color.White; this.btnRefrescar.FlatStyle = FlatStyle.Flat;
            this.btnRefrescar.Click += btnRefrescar_Click;

            this.lblTotal.Location = new Point(550, 20); this.lblTotal.Size = new Size(300, 20); this.lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblTotal.ForeColor = Color.FromArgb(63, 81, 181); this.lblTotal.Text = "Total: 0";

            // FormAsistencias
            this.ClientSize = new Size(900, 500);
            this.Controls.Add(this.dgvAsistencias);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Name = "FormAsistencias";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Gestión de Asistencias";
            this.Load += FormAsistencias_Load;
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsistencias)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }

    public class FormRegistroAsistencia : Form
    {
        private readonly AsistenciaManager _asistenciaManager = new();
        private readonly SocioManager _socioManager = new();
        private readonly bool _esEntrada;
        private TextBox txtBuscarSocio, txtObservaciones;
        private Button btnBuscar, btnGuardar, btnCancelar;
        private Label lblSocioInfo, lblTitulo;
        private Socio? _socioSeleccionado;

        public FormRegistroAsistencia(bool esEntrada)
        {
            _esEntrada = esEntrada;
            InitializeComponent();
        }

        private async void btnBuscar_Click(object? sender, EventArgs e)
        {
            string busqueda = txtBuscarSocio.Text.Trim();
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                MessageBox.Show("Ingrese DNI o nombre", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var socios = await _socioManager.GetAllAsync();
                _socioSeleccionado = socios.FirstOrDefault(s =>
                    s.DNI.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    s.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    s.Apellido.Contains(busqueda, StringComparison.OrdinalIgnoreCase));

                if (_socioSeleccionado == null)
                {
                    MessageBox.Show("Socio no encontrado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblSocioInfo.Text = "❌ Socio no encontrado";
                    lblSocioInfo.ForeColor = Color.Red;
                }
                else
                {
                    lblSocioInfo.Text = $"✓ {_socioSeleccionado.Nombre} {_socioSeleccionado.Apellido} - DNI: {_socioSeleccionado.DNI} - {_socioSeleccionado.Estado}";
                    lblSocioInfo.ForeColor = _socioSeleccionado.Estado == "Activo" ? Color.Green : Color.Orange;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (_socioSeleccionado == null)
            {
                MessageBox.Show("Debe buscar y seleccionar un socio", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_esEntrada)
                {
                    var asistencia = new Asistencia
                    {
                        SocioId = _socioSeleccionado.Id,
                        Fecha = DateTime.Today,
                        HoraEntrada = TimeSpan.Parse(DateTime.Now.ToString("HH:mm")),
                        Observaciones = txtObservaciones.Text.Trim()
                    };

                    var id = await _asistenciaManager.AddAsistenciaAsync(asistencia);
                    MessageBox.Show($"Entrada registrada con ID: {id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var asistencias = await _asistenciaManager.GetAsistenciasByDateAsync(DateTime.Today);
                    var asistenciaHoy = asistencias.FirstOrDefault(a => a.SocioId == _socioSeleccionado.Id && !a.HoraSalida.HasValue);

                    if (asistenciaHoy == null)
                    {
                        MessageBox.Show("No se encontró registro de entrada para hoy", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    asistenciaHoy.HoraSalida = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                    await _asistenciaManager.UpdateAsistenciaAsync(asistenciaHoy);
                    MessageBox.Show("Salida registrada correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            txtBuscarSocio = new TextBox(); txtObservaciones = new TextBox();
            btnBuscar = new Button(); btnGuardar = new Button(); btnCancelar = new Button();
            lblSocioInfo = new Label(); lblTitulo = new Label();
            var lblBuscar = new Label(); var lblObs = new Label();
            SuspendLayout();

            int y = 20;
            lblTitulo.Location = new Point(20, y); lblTitulo.Size = new Size(400, 30); lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitulo.Text = _esEntrada ? "🟢 Registrar Entrada" : "🔴 Registrar Salida"; y += 40;

            lblBuscar.Location = new Point(20, y); lblBuscar.Size = new Size(120, 20); lblBuscar.Text = "Buscar socio:";
            txtBuscarSocio.Location = new Point(150, y); txtBuscarSocio.Size = new Size(200, 23);
            btnBuscar.Location = new Point(360, y - 2); btnBuscar.Size = new Size(80, 27); btnBuscar.Text = "🔍";
            btnBuscar.BackColor = Color.FromArgb(33, 150, 243); btnBuscar.ForeColor = Color.White; btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Click += btnBuscar_Click; y += 35;

            lblSocioInfo.Location = new Point(150, y); lblSocioInfo.Size = new Size(300, 20); lblSocioInfo.Text = "Busque un socio...";
            lblSocioInfo.ForeColor = Color.Gray; y += 35;

            lblObs.Location = new Point(20, y); lblObs.Size = new Size(120, 20); lblObs.Text = "Observaciones:";
            txtObservaciones.Location = new Point(150, y); txtObservaciones.Size = new Size(290, 50); txtObservaciones.Multiline = true; y += 65;

            btnGuardar.Location = new Point(150, y); btnGuardar.Size = new Size(120, 35); btnGuardar.Text = "💾 Guardar";
            btnGuardar.BackColor = _esEntrada ? Color.FromArgb(76, 175, 80) : Color.FromArgb(255, 87, 34);
            btnGuardar.ForeColor = Color.White; btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Click += btnGuardar_Click;

            btnCancelar.Location = new Point(280, y); btnCancelar.Size = new Size(120, 35); btnCancelar.Text = "❌ Cancelar";
            btnCancelar.BackColor = Color.FromArgb(158, 158, 158); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblTitulo, lblBuscar, txtBuscarSocio, btnBuscar, lblSocioInfo, lblObs, txtObservaciones, btnGuardar, btnCancelar });
            ClientSize = new Size(470, y + 60); FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false; StartPosition = FormStartPosition.CenterParent;
            Text = _esEntrada ? "Registrar Entrada" : "Registrar Salida";
            ResumeLayout(false);
        }
    }
}
