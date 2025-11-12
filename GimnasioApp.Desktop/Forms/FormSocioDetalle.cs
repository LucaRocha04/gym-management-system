using GimnasioApp.Managers;
using GimnasioApp.Models;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormSocioDetalle : Form
    {
        private readonly SocioManager _socioManager = new();
        private readonly PlanManager _planManager = new();
        private readonly Socio? _socioActual;

        public FormSocioDetalle(Socio? socio = null)
        {
            _socioActual = socio;
            InitializeComponent();
        }

        private async void FormSocioDetalle_Load(object sender, EventArgs e)
        {
            await CargarPlanes();
            
            if (_socioActual != null)
            {
                // Modo edición
                this.Text = "Editar Socio";
                txtNombre.Text = _socioActual.Nombre;
                txtApellido.Text = _socioActual.Apellido;
                txtDNI.Text = _socioActual.DNI;
                txtTelefono.Text = _socioActual.Telefono;
                txtMail.Text = _socioActual.Mail;
                txtDireccion.Text = _socioActual.Direccion;
                dtpFechaIngreso.Value = _socioActual.FechaIngreso ?? DateTime.Now;
                cboEstado.SelectedItem = _socioActual.Estado;
                
                if (_socioActual.PlanId.HasValue)
                {
                    cboPlan.SelectedValue = _socioActual.PlanId.Value;
                }
            }
            else
            {
                // Modo agregar
                this.Text = "Agregar Socio";
                dtpFechaIngreso.Value = DateTime.Now;
                cboEstado.SelectedItem = "Activo";
            }
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
                
                cboPlan.DisplayMember = "NombrePlan";
                cboPlan.ValueMember = "Id";
                cboPlan.DataSource = planes;
                cboPlan.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar planes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                var socio = new Socio
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    DNI = txtDNI.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Mail = txtMail.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim(),
                    FechaIngreso = dtpFechaIngreso.Value,
                    Estado = cboEstado.SelectedItem?.ToString() ?? "Activo",
                    PlanId = cboPlan.SelectedValue as int?
                };

                if (_socioActual != null)
                {
                    // Actualizar
                    socio.Id = _socioActual.Id;
                    await _socioManager.UpdateAsync(socio);
                    MessageBox.Show("Socio actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Agregar
                    var id = await _socioManager.AddAsync(socio);
                    MessageBox.Show($"Socio registrado con ID: {id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El apellido es requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MessageBox.Show("El DNI es requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDNI.Focus();
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
