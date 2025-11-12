using GimnasioApp.Managers;
using GimnasioApp.Models;
using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
	public partial class FormReportes : Form
	{
		private readonly ReporteManager _reporteManager = new();
		private readonly SocioManager _socioManager = new();
		private readonly PagoManager _pagoManager = new();
		private DataGridView dgvReporte;
		private Button btnMorosos, btnActivos, btnIngresosMes, btnSociosPlan, btnRefrescar;
		private Label lblEstadistica1, lblEstadistica2, lblEstadistica3;
		private Panel panelTop, panelStats, panelBottom;
		private DateTimePicker dtpMes;

		public FormReportes()
		{
			InitializeComponent();
			UITheme.Apply(this);
		}

		private async void FormReportes_Load(object? sender, EventArgs e)
		{
			await CargarEstadisticas();
		}

		private async Task CargarEstadisticas()
		{
			try
			{
				var socios = await _socioManager.GetAllAsync();
				var activos = socios.Count(s => s.Estado == "Activo");
				var inactivos = socios.Count(s => s.Estado == "Inactivo");

				var now = DateTime.Now;
				var pagosMes = await _pagoManager.GetPagosByMonthAsync(now.Year, now.Month);
				decimal ingresosMes = pagosMes.Sum(p => p.Monto);

				lblEstadistica1.Text = $"👥 Total Socios: {socios.Count}";
				lblEstadistica2.Text = $"✅ Activos: {activos} | ❌ Inactivos: {inactivos}";
				lblEstadistica3.Text = $"💰 Ingresos del mes: {ingresosMes:C2}";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error al cargar estadísticas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void btnMorosos_Click(object? sender, EventArgs e)
		{
			try
			{
				var morosos = await _reporteManager.GetSociosConCuotaVencidaAsync();
				var data = morosos.Select(s => new
				{
					DNI = s.DNI,
					Nombre = s.Nombre,
					Apellido = s.Apellido,
					Telefono = s.Telefono,
					Email = s.Mail,
					FechaIngreso = s.FechaIngreso,
					Estado = s.Estado
				}).ToList();

				dgvReporte.DataSource = data;
				if (dgvReporte.Columns.Count > 0)
				{
					dgvReporte.Columns["DNI"]!.Width = 100;
					dgvReporte.Columns["FechaIngreso"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
				}

				MessageBox.Show($"Total de morosos: {data.Count}", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void btnActivos_Click(object? sender, EventArgs e)
		{
			try
			{
				var socios = await _socioManager.GetAllAsync();
				var activos = socios.Where(s => s.Estado == "Activo").Select(s => new
				{
					DNI = s.DNI,
					Nombre = s.Nombre,
					Apellido = s.Apellido,
					Telefono = s.Telefono,
					Email = s.Mail,
					PlanId = s.PlanId,
					FechaIngreso = s.FechaIngreso
				}).ToList();

				dgvReporte.DataSource = activos;
				if (dgvReporte.Columns.Count > 0)
				{
					dgvReporte.Columns["DNI"]!.Width = 100;
					dgvReporte.Columns["FechaIngreso"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
				}

				MessageBox.Show($"Total de activos: {activos.Count}", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void btnIngresosMes_Click(object? sender, EventArgs e)
		{
			try
			{
				int mes = dtpMes.Value.Month;
				int anio = dtpMes.Value.Year;
				var pagos = await _pagoManager.GetPagosByMonthAsync(anio, mes);

				var data = new List<dynamic>();
				foreach (var p in pagos)
				{
					var socio = await _socioManager.GetByIdAsync(p.SocioId);
					data.Add(new
					{
						Fecha = p.FechaPago,
						Socio = socio != null ? $"{socio.Nombre} {socio.Apellido}" : "Desconocido",
						Monto = p.Monto,
						MetodoPago = p.Metodo,
						Observaciones = p.Observaciones ?? string.Empty
					});
				}

				dgvReporte.DataSource = data;
				if (dgvReporte.Columns.Count > 0)
				{
					dgvReporte.Columns["Fecha"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
					dgvReporte.Columns["Monto"]!.DefaultCellStyle.Format = "C2";
					dgvReporte.Columns["Fecha"]!.Width = 100;
					dgvReporte.Columns["Monto"]!.Width = 100;
				}

				decimal total = pagos.Sum(p => p.Monto);
				MessageBox.Show($"Total de ingresos en {dtpMes.Value:MMMM yyyy}: {total:C2}\nCantidad de pagos: {pagos.Count}", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void btnSociosPlan_Click(object? sender, EventArgs e)
		{
			try
			{
				var socios = await _socioManager.GetAllAsync();
				var planManager = new PlanManager();
				var planes = await planManager.GetAllAsync();

				var agrupado = socios.GroupBy(s => s.PlanId).Select(g => new
				{
					PlanId = g.Key,
					Plan = planes.FirstOrDefault(p => p.Id == g.Key)?.NombrePlan ?? "Sin plan",
					CantidadSocios = g.Count(),
					Activos = g.Count(s => s.Estado == "Activo"),
					Inactivos = g.Count(s => s.Estado == "Inactivo")
				}).OrderByDescending(x => x.CantidadSocios).ToList();

				dgvReporte.DataSource = agrupado;

				MessageBox.Show($"Planes con socios registrados: {agrupado.Count}", "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void btnRefrescar_Click(object? sender, EventArgs e)
		{
			await CargarEstadisticas();
			dgvReporte.DataSource = null;
		}

		private void InitializeComponent()
		{
			this.dgvReporte = new DataGridView();
			this.btnMorosos = new Button();
			this.btnActivos = new Button();
			this.btnIngresosMes = new Button();
			this.btnSociosPlan = new Button();
			this.btnRefrescar = new Button();
			this.lblEstadistica1 = new Label();
			this.lblEstadistica2 = new Label();
			this.lblEstadistica3 = new Label();
			this.panelTop = new Panel();
			this.panelStats = new Panel();
			this.panelBottom = new Panel();
			this.dtpMes = new DateTimePicker();
			var lblMes = new Label();
			((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).BeginInit();
			this.panelTop.SuspendLayout();
			this.panelStats.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();

			// panelStats
			this.panelStats.BackColor = Color.FromArgb(63, 81, 181);
			this.panelStats.Controls.Add(this.lblEstadistica1);
			this.panelStats.Controls.Add(this.lblEstadistica2);
			this.panelStats.Controls.Add(this.lblEstadistica3);
			this.panelStats.Dock = DockStyle.Top;
			this.panelStats.Size = new Size(900, 80);

			this.lblEstadistica1.ForeColor = Color.White; this.lblEstadistica1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
			this.lblEstadistica1.Location = new Point(20, 15); this.lblEstadistica1.Size = new Size(250, 20); this.lblEstadistica1.Text = "👥 Total Socios: 0";

			this.lblEstadistica2.ForeColor = Color.White; this.lblEstadistica2.Font = new Font("Segoe UI", 10F);
			this.lblEstadistica2.Location = new Point(20, 40); this.lblEstadistica2.Size = new Size(350, 20); this.lblEstadistica2.Text = "✅ Activos: 0 | ❌ Inactivos: 0";

			this.lblEstadistica3.ForeColor = Color.FromArgb(255, 235, 59); this.lblEstadistica3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
			this.lblEstadistica3.Location = new Point(500, 25); this.lblEstadistica3.Size = new Size(350, 20); this.lblEstadistica3.Text = "💰 Ingresos del mes: $0.00";

			// panelTop
			this.panelTop.BackColor = Color.FromArgb(245, 245, 245);
			this.panelTop.Controls.Add(lblMes);
			this.panelTop.Controls.Add(this.dtpMes);
			this.panelTop.Controls.Add(this.btnIngresosMes);
			this.panelTop.Dock = DockStyle.Top;
			this.panelTop.Size = new Size(900, 60);

			lblMes.Location = new Point(15, 20); lblMes.Size = new Size(100, 20); lblMes.Text = "Mes/Año:";
			this.dtpMes.Location = new Point(120, 17); this.dtpMes.Size = new Size(200, 23); this.dtpMes.Format = DateTimePickerFormat.Custom;
			this.dtpMes.CustomFormat = "MMMM yyyy"; this.dtpMes.ShowUpDown = true;

			this.btnIngresosMes.Location = new Point(330, 8); this.btnIngresosMes.Size = new Size(120, 50); this.btnIngresosMes.Text = "Ver";
			this.btnIngresosMes.BackColor = Color.FromArgb(76, 175, 80); this.btnIngresosMes.ForeColor = Color.White; this.btnIngresosMes.FlatStyle = FlatStyle.Standard;
			this.btnIngresosMes.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
			this.btnIngresosMes.Click += btnIngresosMes_Click;

			// dgvReporte
			this.dgvReporte.AllowUserToAddRows = false;
			this.dgvReporte.AllowUserToDeleteRows = false;
			this.dgvReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvReporte.BackgroundColor = Color.White;
			this.dgvReporte.Dock = DockStyle.Fill;
			this.dgvReporte.ReadOnly = true;
			this.dgvReporte.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

			// panelBottom
			this.panelBottom.BackColor = Color.FromArgb(245, 245, 245);
			this.panelBottom.Controls.Add(this.btnMorosos);
			this.panelBottom.Controls.Add(this.btnActivos);
			this.panelBottom.Controls.Add(this.btnSociosPlan);
			this.panelBottom.Controls.Add(this.btnRefrescar);
			this.panelBottom.Dock = DockStyle.Bottom;
			this.panelBottom.Size = new Size(900, 70); // Aumentado de 60 a 70

			this.btnMorosos.Location = new Point(15, 14); this.btnMorosos.Size = new Size(130, 43); this.btnMorosos.Text = "Morosos";
			this.btnMorosos.BackColor = Color.FromArgb(244, 67, 54); this.btnMorosos.ForeColor = Color.White; this.btnMorosos.FlatStyle = FlatStyle.Standard;
			this.btnMorosos.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnMorosos.Click += btnMorosos_Click;

			this.btnActivos.Location = new Point(155, 14); this.btnActivos.Size = new Size(100, 43); this.btnActivos.Text = "Activos";
			this.btnActivos.BackColor = Color.FromArgb(33, 150, 243); this.btnActivos.ForeColor = Color.White; this.btnActivos.FlatStyle = FlatStyle.Standard;
			this.btnActivos.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnActivos.Click += btnActivos_Click;

			this.btnSociosPlan.Location = new Point(265, 14); this.btnSociosPlan.Size = new Size(100, 43); this.btnSociosPlan.Text = "Planes";
			this.btnSociosPlan.BackColor = Color.FromArgb(103, 58, 181); this.btnSociosPlan.ForeColor = Color.White; this.btnSociosPlan.FlatStyle = FlatStyle.Standard;
			this.btnSociosPlan.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnSociosPlan.Click += btnSociosPlan_Click;

			this.btnRefrescar.Location = new Point(375, 14); this.btnRefrescar.Size = new Size(115, 43); this.btnRefrescar.Text = "Refrescar";
			this.btnRefrescar.BackColor = Color.FromArgb(158, 158, 158); this.btnRefrescar.ForeColor = Color.White; this.btnRefrescar.FlatStyle = FlatStyle.Standard;
			this.btnRefrescar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnRefrescar.Click += btnRefrescar_Click;

			// FormReportes
			this.ClientSize = new Size(900, 600);
			this.Controls.Add(this.dgvReporte);
			this.Controls.Add(this.panelTop);
			this.Controls.Add(this.panelStats);
			this.Controls.Add(this.panelBottom);
			this.Name = "FormReportes";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Reportes y Estadísticas";
			this.Load += FormReportes_Load;
			((System.ComponentModel.ISupportInitialize)(this.dgvReporte)).EndInit();
			this.panelTop.ResumeLayout(false);
			this.panelStats.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);
		}
	}
}
