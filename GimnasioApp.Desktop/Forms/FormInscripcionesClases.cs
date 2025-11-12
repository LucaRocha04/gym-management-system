using GimnasioApp.Managers;
using GimnasioApp.Models;

namespace GimnasioApp.Desktop.Forms
{
    public class FormInscripcionesClases : Form
    {
        private readonly ClaseManager _claseManager = new();
        private readonly InscripcionClaseManager _inscManager = new();
        private readonly SocioManager _socioManager = new();

        private DataGridView dgvClases;
        private DataGridView dgvInscritos;
        private TextBox txtBuscarClase; private Button btnRefrescar;
        private TextBox txtBuscarSocio; private Button btnBuscarSocio; private ListBox lstSocios; private Button btnInscribir; private Button btnQuitar;
        private Label lblCupo;

        public FormInscripcionesClases()
        {
            InitializeComponent();
        }

        private async void Form_Load(object? sender, EventArgs e)
        {
            await CargarClases();
        }

        private async Task CargarClases()
        {
            var clases = await _claseManager.GetProximasAsync(DateTime.Today);
            if (!string.IsNullOrWhiteSpace(txtBuscarClase.Text))
            {
                var f = txtBuscarClase.Text.Trim().ToUpperInvariant();
                clases = clases.Where(c => c.Nombre.ToUpperInvariant().Contains(f) || c.Descripcion.ToUpperInvariant().Contains(f)).ToList();
            }
            dgvClases.DataSource = null; dgvClases.DataSource = clases;
            if (dgvClases.Columns.Count > 0)
            {
                dgvClases.Columns[nameof(Clase.Id)]!.Width = 50;
                dgvClases.Columns[nameof(Clase.Nombre)]!.HeaderText = "Clase";
                dgvClases.Columns[nameof(Clase.Descripcion)]!.HeaderText = "Descripción";
                dgvClases.Columns[nameof(Clase.Fecha)]!.DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvClases.Columns[nameof(Clase.ProfesorId)]!.Visible = false;
                dgvClases.Columns[nameof(Clase.Estado)]!.Visible = false;
            }
            await RefrescarInscritos();
        }

        private async Task RefrescarInscritos()
        {
            if (dgvClases.SelectedRows.Count == 0)
            {
                dgvInscritos.DataSource = null; lblCupo.Text = "Cupo: -"; return;
            }
            var clase = (Clase)dgvClases.SelectedRows[0].DataBoundItem;
            var ins = await _inscManager.GetInscritosAsync(clase.Id);
            dgvInscritos.DataSource = ins.Select(x => new { SocioId = x.SocioId, Nombre = x.NombreSocio }).ToList();
            dgvInscritos.Columns["SocioId"].Width = 70;
            var count = ins.Count;
            var ocupados = await _claseManager.GetInscritosCountAsync(clase.Id);
            lblCupo.Text = $"Cupo: {ocupados}/{clase.Cupo}";
        }

        private async Task BuscarSocios()
        {
            var q = txtBuscarSocio.Text.Trim();
            var socios = await _socioManager.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(q))
                socios = socios.Where(s => ($"{s.Nombre} {s.Apellido}").Contains(q, StringComparison.OrdinalIgnoreCase) || s.DNI.Contains(q)).ToList();
            lstSocios.Items.Clear();
            foreach (var s in socios)
            {
                lstSocios.Items.Add(new SocioItem(s));
            }
        }

        private async Task InscribirSeleccion()
        {
            if (dgvClases.SelectedRows.Count == 0) { MessageBox.Show("Seleccione una clase"); return; }
            if (lstSocios.SelectedItem is not SocioItem si) { MessageBox.Show("Seleccione un socio"); return; }
            var clase = (Clase)dgvClases.SelectedRows[0].DataBoundItem;
            try
            {
                await _inscManager.InscribirAsync(clase.Id, si.Socio.Id);
                await RefrescarInscritos();
                MessageBox.Show($"{si.Socio.Nombre} inscripto en {clase.Nombre}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo inscribir: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task QuitarSeleccion()
        {
            if (dgvClases.SelectedRows.Count == 0 || dgvInscritos.SelectedRows.Count == 0) { MessageBox.Show("Seleccione clase e inscripto"); return; }
            var clase = (Clase)dgvClases.SelectedRows[0].DataBoundItem;
            var socioId = (int)dgvInscritos.SelectedRows[0].Cells["SocioId"].Value;
            try
            {
                await _inscManager.DesinscribirAsync(clase.Id, socioId);
                await RefrescarInscritos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo quitar: {ex.Message}");
            }
        }

        private void InitializeComponent()
        {
            Text = "Inscripción a Clases"; StartPosition = FormStartPosition.CenterParent; ClientSize = new Size(1000, 560);

            // Izquierda: clases + filtros
            var left = new Panel { Dock = DockStyle.Left, Width = 600 };
            var panelTop = new Panel { Dock = DockStyle.Top, Height = 50 };
            var lblBuscarClase = new Label { Text = "Buscar clase:", Location = new Point(10, 15), AutoSize = true };
            txtBuscarClase = new TextBox { Location = new Point(100, 12), Width = 300 };
            btnRefrescar = new Button { Text = "🔄", Location = new Point(410, 10), Width = 40, Height = 30 };
            btnRefrescar.Click += async (s,e)=> await CargarClases();
            panelTop.Controls.AddRange(new Control[]{ lblBuscarClase, txtBuscarClase, btnRefrescar });
            txtBuscarClase.TextChanged += async (s,e)=> await CargarClases();

            dgvClases = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvClases.SelectionChanged += async (s,e)=> await RefrescarInscritos();
            left.Controls.Add(dgvClases); left.Controls.Add(panelTop);

            // Derecha: búsqueda de socio + lista de inscriptos
            var right = new Panel { Dock = DockStyle.Fill };
            var socioTop = new Panel { Dock = DockStyle.Top, Height = 80 };
            var lblSocio = new Label { Text = "Buscar socio (nombre o DNI):", Location = new Point(10, 12), AutoSize = true };
            txtBuscarSocio = new TextBox { Location = new Point(10, 35), Width = 250 };
            btnBuscarSocio = new Button { Text = "Buscar", Location = new Point(270, 33), Width = 100 };
            btnBuscarSocio.Click += async (s,e)=> await BuscarSocios();
            socioTop.Controls.AddRange(new Control[]{ lblSocio, txtBuscarSocio, btnBuscarSocio });

            lstSocios = new ListBox { Dock = DockStyle.Top, Height = 150 };
            btnInscribir = new Button { Text = "➕ Inscribir", Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(76,175,80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnInscribir.Click += async (s,e)=> await InscribirSeleccion();

            var panelInscritos = new Panel { Dock = DockStyle.Fill };
            var lblIns = new Label { Text = "Inscriptos:", Dock = DockStyle.Top, Height = 24 };
            dgvInscritos = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnQuitar = new Button { Text = "🗑️ Quitar", Dock = DockStyle.Bottom, Height = 36, BackColor = Color.FromArgb(244,67,54), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnQuitar.Click += async (s,e)=> await QuitarSeleccion();
            lblCupo = new Label { Text = "Cupo:", Dock = DockStyle.Bottom, Height = 24, TextAlign = ContentAlignment.MiddleRight };
            panelInscritos.Controls.AddRange(new Control[]{ dgvInscritos, lblIns, lblCupo, btnQuitar });

            right.Controls.Add(panelInscritos);
            right.Controls.Add(btnInscribir);
            right.Controls.Add(lstSocios);
            right.Controls.Add(socioTop);

            Controls.Add(right); Controls.Add(left);
            Load += Form_Load;
        }

        private class SocioItem
        {
            public Socio Socio { get; }
            public SocioItem(Socio s) { Socio = s; }
            public override string ToString() => $"{Socio.DNI} - {Socio.Nombre} {Socio.Apellido}";
        }
    }
}
