using GimnasioApp.Managers;
using GimnasioApp.Models;
using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
	public partial class FormEntradaRapida : Form
	{
		private readonly SocioManager _socioManager = new();
		private readonly AsistenciaManager _asistenciaManager = new();
		private TextBox txtDni;
		private Button btnRegistrar, btnCancelar;
		private Label lblEstado;

		public FormEntradaRapida()
		{
			InitializeComponent();
			// UITheme.Apply(this); // Comentado para evitar que sobrescriba los colores
		}

		private async void btnRegistrar_Click(object? sender, EventArgs e)
		{
			string dni = txtDni.Text.Trim();
			if (string.IsNullOrWhiteSpace(dni))
			{
				lblEstado.Text = "Ingrese un DNI"; lblEstado.ForeColor = Color.Red; return;
			}

			try
			{
				var socios = await _socioManager.GetAllAsync();
				var socio = socios.FirstOrDefault(s => string.Equals(s.DNI, dni, StringComparison.OrdinalIgnoreCase));
				if (socio == null)
				{
					lblEstado.Text = "❌ Socio no encontrado"; lblEstado.ForeColor = Color.Red; return;
				}
				if (!string.Equals(socio.Estado, "Activo", StringComparison.OrdinalIgnoreCase))
				{
					lblEstado.Text = "⚠️ Socio no está activo"; lblEstado.ForeColor = Color.Orange; return;
				}

				var asistencia = new Asistencia
				{
					SocioId = socio.Id,
					Fecha = DateTime.Today,
					HoraEntrada = TimeSpan.Parse(DateTime.Now.ToString("HH:mm")),
					Observaciones = "Entrada rápida"
				};
				await _asistenciaManager.AddAsistenciaAsync(asistencia);
				lblEstado.Text = $"✓ Entrada registrada para {socio.Nombre} {socio.Apellido}";
				lblEstado.ForeColor = Color.Green;
			}
			catch (Exception ex)
			{
				lblEstado.Text = $"Error: {ex.Message}"; lblEstado.ForeColor = Color.Red;
			}
		}

		private void InitializeComponent()
		{
			txtDni = new TextBox(); btnRegistrar = new Button(); btnCancelar = new Button(); lblEstado = new Label();
			var lbl = new Label();
			SuspendLayout();

			lbl.Location = new Point(20, 20); lbl.Size = new Size(120, 20); lbl.Text = "DNI del socio:";
			txtDni.Location = new Point(150, 18); txtDni.Size = new Size(220, 23);

			btnRegistrar.Location = new Point(150, 55); btnRegistrar.Size = new Size(150, 40); btnRegistrar.Text = "Registrar";
			btnRegistrar.BackColor = Color.FromArgb(255, 87, 34); btnRegistrar.ForeColor = Color.White; btnRegistrar.FlatStyle = FlatStyle.Standard;
			btnRegistrar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			btnRegistrar.Click += btnRegistrar_Click;

			btnCancelar.Location = new Point(310, 55); btnCancelar.Size = new Size(100, 40); btnCancelar.Text = "Cerrar";
			btnCancelar.BackColor = Color.FromArgb(158, 158, 158); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Standard;
			btnCancelar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			btnCancelar.Click += (s, e) => Close();

			lblEstado.Location = new Point(150, 100); lblEstado.Size = new Size(320, 20); lblEstado.Text = ""; lblEstado.ForeColor = Color.Gray;

			Controls.AddRange(new Control[] { lbl, txtDni, btnRegistrar, btnCancelar, lblEstado });
			ClientSize = new Size(460, 150); FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false; MinimizeBox = false; StartPosition = FormStartPosition.CenterParent; Text = "Entrada Rápida";
			ResumeLayout(false);
		}
	}
}
