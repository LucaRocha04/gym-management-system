using GimnasioApp.Managers;
using GimnasioApp.Models;
using System.Linq;

namespace GimnasioApp.Desktop.Forms
{
    public class FormClasesProfesor : Form
    {
        private readonly Usuario _profesor;
        private readonly ClaseManager _claseManager = new();
    private DataGridView dgv = null!;
    private Button btnNueva = null!, btnCancelar = null!, btnEliminar = null!, btnRefrescar = null!;
    private Label lblInfo = null!;
    private ComboBox cboFiltro = null!;

        public FormClasesProfesor(Usuario profesor)
        {
            _profesor = profesor;
            InitializeComponent();
        }

        private async void FormClasesProfesor_Load(object? sender, EventArgs e)
        {
            await Cargar();
        }

        private async Task Cargar()
        {
            var todas = await _claseManager.GetPorProfesorAsync(_profesor.Id);
            var filtro = cboFiltro?.SelectedItem?.ToString() ?? "Todas";

            IEnumerable<Clase> query = todas;
            var hoy = DateTime.Today;
            switch (filtro)
            {
                case "Futuras":
                    query = query.Where(c => c.Fecha.Date >= hoy);
                    break;
                case "Pasadas":
                    query = query.Where(c => c.Fecha.Date < hoy);
                    break;
                case "Activas":
                    query = query.Where(c => string.Equals(c.Estado, "Activa", StringComparison.OrdinalIgnoreCase));
                    break;
                case "Canceladas":
                    query = query.Where(c => string.Equals(c.Estado, "Cancelada", StringComparison.OrdinalIgnoreCase));
                    break;
                default:
                    break; // Todas
            }

            var clases = query
                .OrderByDescending(c => c.Fecha)
                .ThenByDescending(c => c.HoraInicio)
                .ThenByDescending(c => c.Id)
                .ToList();
            dgv.DataSource = null;
            dgv.DataSource = clases;
            if (dgv.Columns.Count > 0)
            {
                dgv.Columns[nameof(Clase.Id)]!.Width = 50;
                dgv.Columns[nameof(Clase.Nombre)]!.HeaderText = "Nombre";
                dgv.Columns[nameof(Clase.Descripcion)]!.HeaderText = "Descripción";
                dgv.Columns[nameof(Clase.Fecha)]!.DefaultCellStyle.Format = "dd/MM/yyyy";
                dgv.Columns[nameof(Clase.HoraInicio)]!.HeaderText = "Inicio";
                dgv.Columns[nameof(Clase.HoraFin)]!.HeaderText = "Fin";
                dgv.Columns[nameof(Clase.Cupo)]!.HeaderText = "Cupo";
                dgv.Columns[nameof(Clase.ProfesorId)]!.Visible = false;
                dgv.Columns[nameof(Clase.Estado)]!.HeaderText = "Estado";
            }
            lblInfo.Text = $"Mis clases: {clases.Count}";
        }

        private void InitializeComponent()
        {
            dgv = new DataGridView();
            btnNueva = new Button();
            btnCancelar = new Button();
            btnEliminar = new Button();
            btnRefrescar = new Button();
            lblInfo = new Label();
            cboFiltro = new ComboBox();
            SuspendLayout();

            // dgv
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true; dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // top panel (filtro)
            var top = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.FromArgb(250,250,250) };
            var lblFiltro = new Label { Text = "Filtro:", AutoSize = true, Location = new Point(12, 12) };
            cboFiltro.DropDownStyle = ComboBoxStyle.DropDownList; cboFiltro.Location = new Point(60, 8); cboFiltro.Width = 180;
            cboFiltro.Items.AddRange(new object[]{ "Todas", "Futuras", "Pasadas", "Activas", "Canceladas" });
            cboFiltro.SelectedIndex = 1; // por defecto Futuras
            cboFiltro.SelectedIndexChanged += async (s,e) => await Cargar();
            top.Controls.Add(lblFiltro); top.Controls.Add(cboFiltro);

            // panel bottom
            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(245,245,245) };
            btnNueva.Text = "➕ Nueva"; btnNueva.BackColor = Color.FromArgb(76,175,80); btnNueva.ForeColor = Color.White; btnNueva.FlatStyle = FlatStyle.Flat;
            btnNueva.Size = new Size(110,34); btnNueva.Location = new Point(15, 13);
            btnNueva.Click += async (s,e) =>
            {
                using var dlg = new FormClaseDetalle(_profesor);
                if (dlg.ShowDialog() == DialogResult.OK) await Cargar();
            };

            btnCancelar.Text = "🛑 Cancelar"; btnCancelar.BackColor = Color.FromArgb(244,67,54); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Size = new Size(110,34); btnCancelar.Location = new Point(135, 13);
            btnCancelar.Click += async (s,e) =>
            {
                if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Seleccione una clase", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                var clase = (Clase)dgv.SelectedRows[0].DataBoundItem;
                if (clase.Estado == "Cancelada") { MessageBox.Show("La clase ya está cancelada."); return; }
                if (MessageBox.Show($"Cancelar la clase '{clase.Nombre}' del {clase.Fecha:dd/MM/yyyy}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try { await _claseManager.CancelarAsync(clase.Id); await Cargar(); }
                    catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
                }
            };

            // Eliminar
            btnEliminar.Text = "🗑 Eliminar"; btnEliminar.BackColor = Color.FromArgb(121,85,72); btnEliminar.ForeColor = Color.White; btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.Size = new Size(110,34); btnEliminar.Location = new Point(255, 13);
            btnEliminar.Click += async (s,e) =>
            {
                if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Seleccione una clase", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                var clase = (Clase)dgv.SelectedRows[0].DataBoundItem;

                // Regla: sólo eliminar si es pasada o está cancelada. Para futuras activas, pedir cancelar primero.
                if (clase.Fecha.Date >= DateTime.Today && string.Equals(clase.Estado, "Activa", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("No se puede eliminar una clase futura activa. Cancélela primero.", "Acción no permitida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (MessageBox.Show($"Eliminar PERMANENTEMENTE la clase '{clase.Nombre}' del {clase.Fecha:dd/MM/yyyy}?\nSe eliminarán también sus inscripciones.",
                    "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        var ok = await _claseManager.EliminarAsync(clase.Id, _profesor.Id);
                        if (!ok) MessageBox.Show("No se pudo eliminar la clase. Verifique que le pertenezca.");
                        await Cargar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            };

            btnRefrescar.Text = "🔄 Refrescar"; btnRefrescar.BackColor = Color.FromArgb(158,158,158); btnRefrescar.ForeColor = Color.White; btnRefrescar.FlatStyle = FlatStyle.Flat;
            btnRefrescar.Size = new Size(110,34); btnRefrescar.Location = new Point(375, 13);
            btnRefrescar.Click += async (s,e) => await Cargar();

            lblInfo.AutoSize = true; lblInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            // Se reposiciona dinámicamente al redimensionar para que no quede tapado
            void ReposicionarLabelInfo()
            {
                lblInfo.Location = new Point(bottom.Width - lblInfo.PreferredWidth - 15, 18);
            }
            bottom.Resize += (s, e) => ReposicionarLabelInfo();
            // Posición inicial
            ReposicionarLabelInfo();

            bottom.Controls.AddRange(new Control[]{ btnNueva, btnCancelar, btnEliminar, btnRefrescar, lblInfo });

            // Orden correcto para Dock: agregar primero el Fill y luego Top/Bottom,
            // así el Fill ocupa el espacio restante y no queda solapado.
            Controls.Add(dgv);
            Controls.Add(bottom);
            Controls.Add(top);
            Text = "Mis Clases"; StartPosition = FormStartPosition.CenterParent; ClientSize = new Size(900, 500);
            Load += FormClasesProfesor_Load;
            ResumeLayout(false);
        }
    }

    public class FormClaseDetalle : Form
    {
        private readonly Usuario _profesor;
        private readonly ClaseManager _claseManager = new();
    private TextBox txtNombre = null!, txtDescripcion = null!, txtCupo = null!;
    private DateTimePicker dtFecha = null!; private DateTimePicker dtInicio = null!; private DateTimePicker dtFin = null!;
    private Button btnGuardar = null!, btnCancelar = null!;

        public FormClaseDetalle(Usuario profesor)
        {
            _profesor = profesor;
            InitializeComponent();
        }

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre requerido"); return; }
            if (!int.TryParse(txtCupo.Text, out var cupo) || cupo <= 0) { MessageBox.Show("Cupo inválido"); return; }

            var clase = new Clase
            {
                Nombre = txtNombre.Text.Trim(),
                Descripcion = txtDescripcion.Text.Trim(),
                Fecha = dtFecha.Value.Date,
                HoraInicio = dtInicio.Value.TimeOfDay,
                HoraFin = dtFin.Value.TimeOfDay,
                Cupo = cupo,
                ProfesorId = _profesor.Id,
                Estado = "Activa"
            };
            try
            {
                var id = await _claseManager.CrearAsync(clase);
                MessageBox.Show($"Clase creada con ID {id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void InitializeComponent()
        {
            txtNombre = new TextBox(); txtDescripcion = new TextBox(); txtCupo = new TextBox();
            dtFecha = new DateTimePicker(); dtInicio = new DateTimePicker(); dtFin = new DateTimePicker();
            btnGuardar = new Button(); btnCancelar = new Button();
            var lblNombre = new Label(); var lblDesc = new Label(); var lblFecha = new Label(); var lblIni = new Label(); var lblFin = new Label(); var lblCupo = new Label();
            SuspendLayout();

            int y = 20;
            lblNombre.Text = "Nombre:"; lblNombre.Location = new Point(20, y); lblNombre.Size = new Size(100, 24);
            txtNombre.Location = new Point(130, y); txtNombre.Size = new Size(300, 24); y += 40;

            lblDesc.Text = "Descripción:"; lblDesc.Location = new Point(20, y); lblDesc.Size = new Size(100, 24);
            txtDescripcion.Location = new Point(130, y); txtDescripcion.Size = new Size(300, 60); txtDescripcion.Multiline = true; y += 80;

            lblFecha.Text = "Fecha:"; lblFecha.Location = new Point(20, y);
            dtFecha.Location = new Point(130, y); dtFecha.Format = DateTimePickerFormat.Short; y += 40;

            lblIni.Text = "Inicio:"; lblIni.Location = new Point(20, y);
            dtInicio.Location = new Point(130, y); dtInicio.Format = DateTimePickerFormat.Time; dtInicio.ShowUpDown = true; y += 40;

            lblFin.Text = "Fin:"; lblFin.Location = new Point(20, y);
            dtFin.Location = new Point(130, y); dtFin.Format = DateTimePickerFormat.Time; dtFin.ShowUpDown = true; y += 40;

            lblCupo.Text = "Cupo:"; lblCupo.Location = new Point(20, y);
            txtCupo.Location = new Point(130, y); txtCupo.Size = new Size(100, 24); y += 50;

            btnGuardar.Text = "💾 Guardar"; btnGuardar.BackColor = Color.FromArgb(76,175,80); btnGuardar.ForeColor = Color.White; btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Location = new Point(130, y); btnGuardar.Size = new Size(120, 34); btnGuardar.Click += btnGuardar_Click;

            btnCancelar.Text = "❌ Cancelar"; btnCancelar.BackColor = Color.FromArgb(158,158,158); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Location = new Point(260, y); btnCancelar.Size = new Size(120, 34); btnCancelar.Click += (s,e)=>{ DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[]{ lblNombre, txtNombre, lblDesc, txtDescripcion, lblFecha, dtFecha, lblIni, dtInicio, lblFin, dtFin, lblCupo, txtCupo, btnGuardar, btnCancelar });
            Text = "Nueva Clase"; StartPosition = FormStartPosition.CenterParent; ClientSize = new Size(480, y + 70);
            ResumeLayout(false);
        }
    }
}
