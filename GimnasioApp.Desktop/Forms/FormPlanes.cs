using GimnasioApp.Managers;
using GimnasioApp.Models;
using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormPlanes : Form
    {
        private readonly PlanManager _planManager = new();
        private readonly SocioManager _socioManager = new();
        private DataGridView dgvPlanes;
        private Button btnAgregar, btnEditar, btnEliminar, btnRefrescar;
        private Label lblTotal;
        private Panel panelBottom;

        public FormPlanes()
        {
            InitializeComponent();
            UITheme.Apply(this);
        }

        private async void FormPlanes_Load(object? sender, EventArgs e)
        {
            await CargarPlanes();
        }

        private async Task CargarPlanes()
        {
            try
            {
                var planes = (await _planManager.GetAllAsync())
                    .GroupBy(p => (p.NombrePlan ?? string.Empty).Trim().ToUpperInvariant())
                    .Select(g => g.OrderByDescending(x => x.Id).First())
                    .OrderBy(p => p.NombrePlan)
                    .ToList();

                dgvPlanes.DataSource = null;
                dgvPlanes.DataSource = planes;

                if (dgvPlanes.Columns.Count > 0)
                {
                    dgvPlanes.Columns["Id"]!.HeaderText = "ID";
                    dgvPlanes.Columns["Id"]!.Width = 50;
                    dgvPlanes.Columns["NombrePlan"]!.HeaderText = "Nombre del Plan";
                    dgvPlanes.Columns["DuracionDias"]!.HeaderText = "Duración (días)";
                    dgvPlanes.Columns["DuracionDias"]!.Width = 120;
                    dgvPlanes.Columns["Precio"]!.HeaderText = "Precio";
                    dgvPlanes.Columns["Precio"]!.DefaultCellStyle.Format = "C2";
                    dgvPlanes.Columns["Precio"]!.Width = 100;
                    dgvPlanes.Columns["Descripcion"]!.HeaderText = "Descripción";
                }

                lblTotal.Text = $"Total planes: {planes.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar planes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object? sender, EventArgs e)
        {
            using var form = new FormPlanDetalle(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _ = CargarPlanes();
            }
        }

        private void btnEditar_Click(object? sender, EventArgs e)
        {
            if (dgvPlanes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un plan para editar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var plan = (Plan)dgvPlanes.SelectedRows[0].DataBoundItem;
            using var form = new FormPlanDetalle(plan);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _ = CargarPlanes();
            }
        }

        private async void btnEliminar_Click(object? sender, EventArgs e)
        {
            if (dgvPlanes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un plan para eliminar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var plan = (Plan)dgvPlanes.SelectedRows[0].DataBoundItem;
            
            // Verificar si hay socios usando este plan
            var socios = await _socioManager.GetAllAsync();
            if (socios.Any(s => s.PlanId == plan.Id))
            {
                MessageBox.Show("No se puede eliminar el plan porque hay socios que lo están usando.", 
                    "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Desea eliminar el plan '{plan.NombrePlan}'?",
                "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _planManager.DeleteAsync(plan.Id);
                    MessageBox.Show("Plan eliminado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarPlanes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnRefrescar_Click(object? sender, EventArgs e)
        {
            await CargarPlanes();
        }

        private void InitializeComponent()
        {
            this.dgvPlanes = new DataGridView();
            this.btnAgregar = new Button();
            this.btnEditar = new Button();
            this.btnEliminar = new Button();
            this.btnRefrescar = new Button();
            this.lblTotal = new Label();
            this.panelBottom = new Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlanes)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // dgvPlanes
            this.dgvPlanes.AllowUserToAddRows = false;
            this.dgvPlanes.AllowUserToDeleteRows = false;
            this.dgvPlanes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPlanes.BackgroundColor = Color.White;
            this.dgvPlanes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlanes.Dock = DockStyle.Fill;
            this.dgvPlanes.Location = new Point(0, 0);
            this.dgvPlanes.MultiSelect = false;
            this.dgvPlanes.Name = "dgvPlanes";
            this.dgvPlanes.ReadOnly = true;
            this.dgvPlanes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlanes.Size = new Size(800, 390);

            // panelBottom
            this.panelBottom.BackColor = Color.FromArgb(245, 245, 245);
            this.panelBottom.Controls.Add(this.btnAgregar);
            this.panelBottom.Controls.Add(this.btnEditar);
            this.panelBottom.Controls.Add(this.btnEliminar);
            this.panelBottom.Controls.Add(this.btnRefrescar);
            this.panelBottom.Controls.Add(this.lblTotal);
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Location = new Point(0, 390);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new Size(800, 60);

            // btnAgregar
            this.btnAgregar.BackColor = Color.FromArgb(76, 175, 80);
            this.btnAgregar.FlatStyle = FlatStyle.Standard;
            this.btnAgregar.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            this.btnAgregar.ForeColor = Color.White;
            this.btnAgregar.Location = new Point(15, 14);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new Size(100, 43);
            this.btnAgregar.Text = "Nuevo";
            this.btnAgregar.UseVisualStyleBackColor = false;
            this.btnAgregar.Click += btnAgregar_Click;

            // btnEditar
            this.btnEditar.BackColor = Color.FromArgb(33, 150, 243);
            this.btnEditar.FlatStyle = FlatStyle.Standard;
            this.btnEditar.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            this.btnEditar.ForeColor = Color.White;
            this.btnEditar.Location = new Point(125, 14);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new Size(100, 43);
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = false;
            this.btnEditar.Click += btnEditar_Click;

            // btnEliminar
            this.btnEliminar.BackColor = Color.FromArgb(244, 67, 54);
            this.btnEliminar.FlatStyle = FlatStyle.Standard;
            this.btnEliminar.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            this.btnEliminar.ForeColor = Color.White;
            this.btnEliminar.Location = new Point(235, 14);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new Size(100, 43);
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += btnEliminar_Click;

            // btnRefrescar
            this.btnRefrescar.BackColor = Color.FromArgb(158, 158, 158);
            this.btnRefrescar.FlatStyle = FlatStyle.Standard;
            this.btnRefrescar.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            this.btnRefrescar.ForeColor = Color.White;
            this.btnRefrescar.Location = new Point(345, 14);
            this.btnRefrescar.Name = "btnRefrescar";
            this.btnRefrescar.Size = new Size(110, 43);
            this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.UseVisualStyleBackColor = false;
            this.btnRefrescar.Click += btnRefrescar_Click;

            // lblTotal
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblTotal.ForeColor = Color.FromArgb(63, 81, 181);
            this.lblTotal.Location = new Point(550, 20);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Text = "Total planes: 0";

            // FormPlanes
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(this.dgvPlanes);
            this.Controls.Add(this.panelBottom);
            this.Name = "FormPlanes";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Gestión de Planes";
            this.Load += FormPlanes_Load;
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlanes)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }
    }

    // FormPlanDetalle
    public class FormPlanDetalle : Form
    {
        private readonly PlanManager _planManager = new();
        private readonly Plan? _planActual;
        private TextBox txtNombre, txtDuracion, txtPrecio, txtDescripcion;
        private Button btnGuardar, btnCancelar;

        public FormPlanDetalle(Plan? plan)
        {
            _planActual = plan;
            InitializeComponent();
            if (_planActual != null)
            {
                Text = "Editar Plan";
                txtNombre.Text = _planActual.NombrePlan;
                txtDuracion.Text = _planActual.DuracionDias.ToString();
                txtPrecio.Text = _planActual.Precio.ToString("F2");
                txtDescripcion.Text = _planActual.Descripcion;
            }
            else
            {
                Text = "Agregar Plan";
            }
        }

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del plan es requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtDuracion.Text, out int duracion) || duracion <= 0)
            {
                MessageBox.Show("La duración debe ser un número mayor a 0", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("El precio debe ser un número mayor a 0", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var plan = new Plan
                {
                    NombrePlan = txtNombre.Text.Trim(),
                    DuracionDias = duracion,
                    Precio = precio,
                    Descripcion = txtDescripcion.Text.Trim()
                };

                if (_planActual != null)
                {
                    plan.Id = _planActual.Id;
                    await _planManager.UpdateAsync(plan);
                    MessageBox.Show("Plan actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var id = await _planManager.AddAsync(plan);
                    MessageBox.Show($"Plan registrado con ID: {id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            txtNombre = new TextBox(); txtDuracion = new TextBox(); txtPrecio = new TextBox(); txtDescripcion = new TextBox();
            btnGuardar = new Button(); btnCancelar = new Button();
            var lblNombre = new Label(); var lblDuracion = new Label(); var lblPrecio = new Label(); var lblDescripcion = new Label();
            SuspendLayout();

            int y = 20;
            lblNombre.Location = new Point(20, y); lblNombre.Size = new Size(120, 20); lblNombre.Text = "Nombre del Plan:";
            txtNombre.Location = new Point(150, y); txtNombre.Size = new Size(280, 23); y += 40;

            lblDuracion.Location = new Point(20, y); lblDuracion.Size = new Size(120, 20); lblDuracion.Text = "Duración (días):";
            txtDuracion.Location = new Point(150, y); txtDuracion.Size = new Size(280, 23); y += 40;

            lblPrecio.Location = new Point(20, y); lblPrecio.Size = new Size(120, 20); lblPrecio.Text = "Precio ($):";
            txtPrecio.Location = new Point(150, y); txtPrecio.Size = new Size(280, 23); y += 40;

            lblDescripcion.Location = new Point(20, y); lblDescripcion.Size = new Size(120, 20); lblDescripcion.Text = "Descripción:";
            txtDescripcion.Location = new Point(150, y); txtDescripcion.Size = new Size(280, 60); txtDescripcion.Multiline = true; y += 80;

            btnGuardar.Location = new Point(150, y); btnGuardar.Size = new Size(90, 35); btnGuardar.Text = "Guardar";
            btnGuardar.BackColor = Color.FromArgb(76, 175, 80); btnGuardar.ForeColor = Color.White; btnGuardar.FlatStyle = FlatStyle.Standard;
            btnGuardar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnGuardar.Click += btnGuardar_Click;

            btnCancelar.Location = new Point(250, y); btnCancelar.Size = new Size(90, 35); btnCancelar.Text = "Cancelar";
            btnCancelar.BackColor = Color.FromArgb(158, 158, 158); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Standard;
            btnCancelar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblNombre, txtNombre, lblDuracion, txtDuracion, lblPrecio, txtPrecio, lblDescripcion, txtDescripcion, btnGuardar, btnCancelar });
            ClientSize = new Size(460, y + 60); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
            MinimizeBox = false; StartPosition = FormStartPosition.CenterParent; Text = "Detalle Plan";
            ResumeLayout(false);
        }
    }
}
