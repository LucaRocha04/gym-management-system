using GimnasioApp.Managers;

namespace GimnasioApp.Desktop.Forms
{
	public partial class FormPlanesConsulta : Form
	{
		private readonly PlanManager _planManager = new();
		private DataGridView dgv;
		private Button btnRefrescar;
		private Panel panelTop, panelBottom;
		private Label lblTitulo;

		public FormPlanesConsulta()
		{
			InitializeComponent();
		}

		private async void FormPlanesConsulta_Load(object? sender, EventArgs e)
		{
			await Cargar();
		}

		private async Task Cargar()
		{
			try
			{
				var planes = await _planManager.GetAllAsync();
				var dedup = planes
					.GroupBy(p => p.NombrePlan.Trim())
					.Select(g => g.OrderByDescending(x => x.Id).First())
					.Select(p => new { p.NombrePlan, p.DuracionDias, p.Precio, p.Descripcion })
					.OrderBy(p => p.NombrePlan)
					.ToList();

				dgv.DataSource = dedup;
				if (dgv.Columns.Count > 0)
				{
					dgv.Columns["Precio"]!.DefaultCellStyle.Format = "C2";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void btnRefrescar_Click(object? sender, EventArgs e)
		{
			await Cargar();
		}

		private void InitializeComponent()
		{
			dgv = new DataGridView(); btnRefrescar = new Button(); panelTop = new Panel(); panelBottom = new Panel(); lblTitulo = new Label();
			((System.ComponentModel.ISupportInitialize)(dgv)).BeginInit(); panelTop.SuspendLayout(); panelBottom.SuspendLayout(); SuspendLayout();

			panelTop.BackColor = Color.FromArgb(63,81,181); panelTop.Dock = DockStyle.Top; panelTop.Size = new Size(800,60);
			lblTitulo.ForeColor = Color.White; lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold); lblTitulo.Location = new Point(15,15); lblTitulo.Size = new Size(400,30); lblTitulo.Text = "Planes Disponibles";
			panelTop.Controls.Add(lblTitulo);

			dgv.AllowUserToAddRows = false; dgv.AllowUserToDeleteRows = false; dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; dgv.BackgroundColor = Color.White; dgv.Dock = DockStyle.Fill; dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

			panelBottom.BackColor = Color.FromArgb(245,245,245); panelBottom.Dock = DockStyle.Bottom; panelBottom.Size = new Size(800,60);
			btnRefrescar.Location = new Point(15,14); btnRefrescar.Size = new Size(120,34); btnRefrescar.Text = "🔄 Refrescar"; btnRefrescar.BackColor = Color.FromArgb(158,158,158); btnRefrescar.ForeColor = Color.White; btnRefrescar.FlatStyle = FlatStyle.Flat; btnRefrescar.Click += btnRefrescar_Click;
			panelBottom.Controls.Add(btnRefrescar);

			ClientSize = new Size(800, 500); Controls.Add(dgv); Controls.Add(panelTop); Controls.Add(panelBottom); StartPosition = FormStartPosition.CenterParent; Text = "Planes Disponibles"; Load += FormPlanesConsulta_Load;

			((System.ComponentModel.ISupportInitialize)(dgv)).EndInit(); panelTop.ResumeLayout(false); panelBottom.ResumeLayout(false); ResumeLayout(false);
		}
	}
}
