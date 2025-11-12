using GimnasioApp.Managers;
using GimnasioApp.Models;
using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormSocios : Form
    {
        private readonly SocioManager _socioManager = new();
        private readonly PlanManager _planManager = new();

        public FormSocios()
        {
            InitializeComponent();
            UITheme.Apply(this);
            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            // Configurar doble clic para ver detalles
            dgvSocios.CellDoubleClick += DgvSocios_CellDoubleClick;
        }

        private void DgvSocios_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                MostrarDetalleSocio();
            }
        }

        private async void FormSocios_Load(object sender, EventArgs e)
        {
            await CargarSocios();
        }

        private async Task CargarSocios()
        {
            try
            {
                var sociosConMembresia = await _socioManager.GetAllWithMembresiaAsync();
                dgvSocios.DataSource = null;
                dgvSocios.DataSource = sociosConMembresia;
                
                // Configurar columnas - Vista simplificada con datos esenciales
                if (dgvSocios.Columns.Count > 0)
                {
                    // Mostrar solo las columnas esenciales
                    dgvSocios.Columns["Id"]!.HeaderText = "ID";
                    dgvSocios.Columns["Id"]!.Width = 60;
                    dgvSocios.Columns["Id"]!.DisplayIndex = 0;
                    
                    dgvSocios.Columns["Nombre"]!.HeaderText = "Nombre";
                    dgvSocios.Columns["Nombre"]!.Width = 120;
                    dgvSocios.Columns["Nombre"]!.DisplayIndex = 1;
                    
                    dgvSocios.Columns["Apellido"]!.HeaderText = "Apellido";
                    dgvSocios.Columns["Apellido"]!.Width = 120;
                    dgvSocios.Columns["Apellido"]!.DisplayIndex = 2;
                    
                    dgvSocios.Columns["DNI"]!.HeaderText = "DNI";
                    dgvSocios.Columns["DNI"]!.Width = 100;
                    dgvSocios.Columns["DNI"]!.DisplayIndex = 3;
                    
                    dgvSocios.Columns["FechaIngreso"]!.HeaderText = "Fecha Ingreso";
                    dgvSocios.Columns["FechaIngreso"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvSocios.Columns["FechaIngreso"]!.Width = 120;
                    dgvSocios.Columns["FechaIngreso"]!.DisplayIndex = 4;
                    
                    dgvSocios.Columns["VencimientoInfo"]!.HeaderText = "Vencimiento";
                    dgvSocios.Columns["VencimientoInfo"]!.Width = 180;
                    dgvSocios.Columns["VencimientoInfo"]!.DisplayIndex = 5;
                    
                    // Ocultar todas las demás columnas para vista simplificada
                    dgvSocios.Columns["Telefono"]!.Visible = false;
                    dgvSocios.Columns["Mail"]!.Visible = false;
                    dgvSocios.Columns["Estado"]!.Visible = false;
                    dgvSocios.Columns["NombrePlan"]!.Visible = false;
                    dgvSocios.Columns["EstadoMembresia"]!.Visible = false;
                    dgvSocios.Columns["PlanId"]!.Visible = false;
                    dgvSocios.Columns["Direccion"]!.Visible = false;
                    dgvSocios.Columns["DiasRestantes"]!.Visible = false;
                    dgvSocios.Columns["FechaVencimiento"]!.Visible = false;
                    
                    // Aplicar colores según estado de membresía
                    foreach (DataGridViewRow row in dgvSocios.Rows)
                    {
                        if (row.DataBoundItem is SocioConMembresia socio)
                        {
                            switch (socio.EstadoMembresia)
                            {
                                case "Vencida":
                                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238); // Rojo claro
                                    row.DefaultCellStyle.ForeColor = Color.FromArgb(220, 53, 69);   // Rojo
                                    break;
                                case "Por Vencer":
                                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205); // Amarillo claro
                                    row.DefaultCellStyle.ForeColor = Color.FromArgb(255, 193, 7);   // Amarillo oscuro
                                    break;
                                case "Activa":
                                    row.DefaultCellStyle.BackColor = Color.FromArgb(212, 237, 218); // Verde claro
                                    row.DefaultCellStyle.ForeColor = Color.FromArgb(25, 135, 84);   // Verde
                                    break;
                                case "Sin Plan":
                                    row.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250); // Gris claro
                                    row.DefaultCellStyle.ForeColor = Color.FromArgb(108, 117, 125);  // Gris
                                    break;
                            }
                        }
                    }
                }
                
                lblTotal.Text = $"Total socios: {sociosConMembresia.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar socios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarDetalleSocio()
        {
            if (dgvSocios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un socio para ver sus detalles", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var socioConMembresia = (SocioConMembresia)dgvSocios.SelectedRows[0].DataBoundItem;
            
            // Mostrar detalles completos en un MessageBox personalizado
            string detalles = $@"INFORMACIÓN COMPLETA DEL SOCIO

📋 DATOS PERSONALES:
• ID: {socioConMembresia.Id}
• Nombre: {socioConMembresia.Nombre}
• Apellido: {socioConMembresia.Apellido}
• DNI: {socioConMembresia.DNI}
• Teléfono: {socioConMembresia.Telefono}
• Email: {socioConMembresia.Mail}
• Dirección: {(string.IsNullOrEmpty(socioConMembresia.Direccion) ? "No especificada" : socioConMembresia.Direccion)}

📅 INFORMACIÓN DE MEMBRESÍA:
• Fecha de Ingreso: {(socioConMembresia.FechaIngreso?.ToString("dd/MM/yyyy") ?? "No especificada")}
• Estado: {socioConMembresia.Estado}
• Plan: {socioConMembresia.NombrePlan}
• Estado de Membresía: {socioConMembresia.EstadoMembresia}
• Vencimiento: {socioConMembresia.VencimientoInfo}";

            MessageBox.Show(detalles, "Detalles del Socio", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnVer_Click(object sender, EventArgs e)
        {
            MostrarDetalleSocio();
        }

        private async void btnAgregar_Click(object sender, EventArgs e)
        {
            var form = new FormSocioDetalle(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await CargarSocios();
            }
        }

        private async void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvSocios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un socio para editar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var socioConMembresia = (SocioConMembresia)dgvSocios.SelectedRows[0].DataBoundItem;
            // SocioConMembresia hereda de Socio, así que podemos pasarlo directamente
            var form = new FormSocioDetalle(socioConMembresia);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await CargarSocios();
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvSocios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un socio para procesar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var socioConMembresia = (SocioConMembresia)dgvSocios.SelectedRows[0].DataBoundItem;
            
            try
            {
                if (socioConMembresia.Estado == "Activo")
                {
                    // Socio activo - opción de dar de baja
                    var result = MessageBox.Show($"¿Desea dar de baja al socio {socioConMembresia.Nombre} {socioConMembresia.Apellido}?\n\nUna vez dado de baja, podrá eliminarlo definitivamente.", 
                        "Confirmar Baja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        await _socioManager.DeleteAsync(socioConMembresia.Id);
                        MessageBox.Show("Socio dado de baja correctamente.\n\nPuede eliminarlo definitivamente seleccionándolo nuevamente y presionando Eliminar.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await CargarSocios();
                    }
                }
                else
                {
                    // Socio inactivo - opción de eliminar definitivamente
                    var result = MessageBox.Show($"El socio {socioConMembresia.Nombre} {socioConMembresia.Apellido} está dado de baja.\n\n" +
                        "¿Desea eliminarlo DEFINITIVAMENTE del sistema?\n\n" +
                        "⚠️ Esta acción NO se puede deshacer y eliminará:\n" +
                        "• El registro del socio\n" +
                        "• Todos sus pagos\n" +
                        "• Todo el historial de asistencias\n" +
                        "• Todas las inscripciones a clases", 
                        "Confirmar Eliminación Definitiva", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        await _socioManager.EliminarDefinitivamenteAsync(socioConMembresia.Id);
                        MessageBox.Show("Socio eliminado definitivamente del sistema.", "Eliminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await CargarSocios();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text.Trim();
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                await CargarSocios();
                return;
            }

            try
            {
                var socios = await _socioManager.GetAllAsync();
                var filtrados = socios.Where(s => 
                    s.DNI.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    s.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    s.Apellido.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    (s.Mail ?? "").Contains(busqueda, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                dgvSocios.DataSource = null;
                dgvSocios.DataSource = filtrados;
                lblTotal.Text = $"Encontrados: {filtrados.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en la búsqueda: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnBuscar.PerformClick();
            }
        }

        private async void btnMorosos_Click(object sender, EventArgs e)
        {
            try
            {
                var reporteManager = new ReporteManager();
                var morosos = await reporteManager.GetSociosConCuotaVencidaAsync();
                
                dgvSocios.DataSource = null;
                dgvSocios.DataSource = morosos;
                lblTotal.Text = $"Socios con cuota vencida: {morosos.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRefrescar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            await CargarSocios();
        }
    }
}
