using GimnasioApp.Managers;
using GimnasioApp.Models;
using System.Linq;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormPagos : Form
    {
        private readonly PagoManager _pagoManager = new();
        private readonly SocioManager _socioManager = new();
        private DataGridView dgvPagos;
        private Button btnRegistrar, btnPorSocio, btnDelMes, btnRefrescar;
        private Label lblTotal, lblSuma;
        private Panel panelTop, panelBottom;
        private TextBox txtBuscarSocio;
        private Label lblBuscar;

        public FormPagos()
        {
            InitializeComponent();
        }

        private async void FormPagos_Load(object? sender, EventArgs e)
        {
            await CargarPagosDelMes();
        }

        private async Task CargarPagosDelMes()
        {
            try
            {
                var now = DateTime.Now;
                var pagos = await _pagoManager.GetPagosByMonthAsync(now.Year, now.Month);
                await MostrarPagos(pagos, $"Pagos de {now:MMMM yyyy}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task MostrarPagos(List<Pago> pagos, string titulo)
        {
            var pagosConSocio = new List<dynamic>();
            decimal total = 0;

            foreach (var p in pagos)
            {
                var socio = await _socioManager.GetByIdAsync(p.SocioId);
                pagosConSocio.Add(new
                {
                    ID = p.IdPago,
                    Socio = socio != null ? $"{socio.Nombre} {socio.Apellido}" : "Desconocido",
                    DNI = socio?.DNI ?? "",
                    Fecha = p.FechaPago,
                    Monto = p.Monto,
                    Metodo = p.Metodo,
                    Observaciones = p.Observaciones ?? ""
                });
                total += p.Monto;
            }

            dgvPagos.DataSource = null;
            dgvPagos.DataSource = pagosConSocio;

            if (dgvPagos.Columns.Count > 0)
            {
                dgvPagos.Columns["ID"]!.Width = 50;
                dgvPagos.Columns["Monto"]!.DefaultCellStyle.Format = "C2";
                dgvPagos.Columns["Monto"]!.Width = 100;
                dgvPagos.Columns["Fecha"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPagos.Columns["Fecha"]!.Width = 100;
            }

            lblTotal.Text = $"{titulo} - Total: {pagosConSocio.Count} pagos";
            lblSuma.Text = $"Suma total: {total:C2}";
        }

        private void btnRegistrar_Click(object? sender, EventArgs e)
        {
            using var form = new FormPagoRegistro();
            if (form.ShowDialog() == DialogResult.OK)
            {
                _ = CargarPagosDelMes();
            }
        }

        private async void btnPorSocio_Click(object? sender, EventArgs e)
        {
            string busqueda = txtBuscarSocio.Text.Trim();
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

                var pagos = await _pagoManager.GetPagosBySocioAsync(socio.Id);
                await MostrarPagos(pagos, $"Pagos de {socio.Nombre} {socio.Apellido}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelMes_Click(object? sender, EventArgs e)
        {
            await CargarPagosDelMes();
        }

        private async void btnRefrescar_Click(object? sender, EventArgs e)
        {
            await CargarPagosDelMes();
        }

        private void InitializeComponent()
        {
            this.dgvPagos = new DataGridView();
            this.btnRegistrar = new Button();
            this.btnPorSocio = new Button();
            this.btnDelMes = new Button();
            this.btnRefrescar = new Button();
            this.lblTotal = new Label();
            this.lblSuma = new Label();
            this.panelTop = new Panel();
            this.panelBottom = new Panel();
            this.txtBuscarSocio = new TextBox();
            this.lblBuscar = new Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            // panelTop
            this.panelTop.BackColor = Color.FromArgb(245, 245, 245);
            this.panelTop.Controls.Add(this.lblBuscar);
            this.panelTop.Controls.Add(this.txtBuscarSocio);
            this.panelTop.Controls.Add(this.btnPorSocio);
            this.panelTop.Controls.Add(this.btnDelMes);
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Size = new Size(900, 60);

            this.lblBuscar.Location = new Point(15, 18); this.lblBuscar.Size = new Size(100, 20); this.lblBuscar.Text = "Buscar socio:";
            this.txtBuscarSocio.Location = new Point(120, 15); this.txtBuscarSocio.Size = new Size(250, 23);
            this.btnPorSocio.Location = new Point(380, 13); this.btnPorSocio.Size = new Size(120, 35); this.btnPorSocio.Text = "Ver Pagos";
            this.btnPorSocio.BackColor = Color.FromArgb(33, 150, 243); this.btnPorSocio.ForeColor = Color.White; this.btnPorSocio.FlatStyle = FlatStyle.Standard;
            this.btnPorSocio.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            this.btnPorSocio.Click += btnPorSocio_Click;

            this.btnDelMes.Location = new Point(510, 13); this.btnDelMes.Size = new Size(120, 35); this.btnDelMes.Text = "Mes Actual";
            this.btnDelMes.BackColor = Color.FromArgb(103, 58, 183); this.btnDelMes.ForeColor = Color.White; this.btnDelMes.FlatStyle = FlatStyle.Standard;
            this.btnDelMes.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            this.btnDelMes.Click += btnDelMes_Click;

            // dgvPagos
            this.dgvPagos.AllowUserToAddRows = false;
            this.dgvPagos.AllowUserToDeleteRows = false;
            this.dgvPagos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPagos.BackgroundColor = Color.White;
            this.dgvPagos.Dock = DockStyle.Fill;
            this.dgvPagos.ReadOnly = true;
            this.dgvPagos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // panelBottom
            this.panelBottom.BackColor = Color.FromArgb(245, 245, 245);
            this.panelBottom.Controls.Add(this.btnRegistrar);
            this.panelBottom.Controls.Add(this.btnRefrescar);
            this.panelBottom.Controls.Add(this.lblTotal);
            this.panelBottom.Controls.Add(this.lblSuma);
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Size = new Size(1000, 70);

            this.btnRegistrar.Location = new Point(15, 25); this.btnRegistrar.Size = new Size(120, 35); this.btnRegistrar.Text = "Registrar Pago";
            this.btnRegistrar.BackColor = Color.FromArgb(76, 175, 80); this.btnRegistrar.ForeColor = Color.White; this.btnRegistrar.FlatStyle = FlatStyle.Standard;
            this.btnRegistrar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            this.btnRegistrar.Click += btnRegistrar_Click;

            this.btnRefrescar.Location = new Point(145, 25); this.btnRefrescar.Size = new Size(100, 35); this.btnRefrescar.Text = "Refrescar";
            this.btnRefrescar.BackColor = Color.FromArgb(158, 158, 158); this.btnRefrescar.ForeColor = Color.White; this.btnRefrescar.FlatStyle = FlatStyle.Standard;
            this.btnRefrescar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            this.btnRefrescar.Click += btnRefrescar_Click;

            this.lblTotal.Location = new Point(400, 3); this.lblTotal.Size = new Size(480, 20); this.lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblTotal.ForeColor = Color.FromArgb(63, 81, 181); this.lblTotal.Text = "Total: 0 pagos";
            this.lblTotal.TextAlign = ContentAlignment.MiddleRight;

            this.lblSuma.Location = new Point(400, 23); this.lblSuma.Size = new Size(480, 20); this.lblSuma.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblSuma.ForeColor = Color.FromArgb(76, 175, 80); this.lblSuma.Text = "Suma total: $0.00";
            this.lblSuma.TextAlign = ContentAlignment.MiddleRight;

            // FormPagos
            this.ClientSize = new Size(1000, 500);
            this.Controls.Add(this.dgvPagos);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.Name = "FormPagos";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Gestión de Pagos";
            this.Load += FormPagos_Load;
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagos)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }

    public class FormPagoRegistro : Form
    {
        private readonly PagoManager _pagoManager = new();
        private readonly SocioManager _socioManager = new();
        private TextBox txtBuscarSocio, txtMonto, txtObservaciones;
        private ComboBox cboMetodo;
        private Button btnBuscar, btnGuardar, btnCancelar;
        private Label lblSocioInfo;
        private Socio? _socioSeleccionado;

        public FormPagoRegistro()
        {
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
                    lblSocioInfo.Text = "Socio no encontrado";
                    lblSocioInfo.ForeColor = Color.Red;
                }
                else
                {
                    lblSocioInfo.Text = $"✓ {_socioSeleccionado.Nombre} {_socioSeleccionado.Apellido} - DNI: {_socioSeleccionado.DNI}";
                    lblSocioInfo.ForeColor = Color.Green;
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

            // Extraer solo los números para validación
            string numerosLimpios = new string(txtMonto.Text.Where(char.IsDigit).ToArray());
            if (!decimal.TryParse(numerosLimpios, out decimal monto) || monto <= 0)
            {
                MessageBox.Show("El monto debe ser mayor a 0", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var pago = new Pago
                {
                    SocioId = _socioSeleccionado.Id,
                    FechaPago = DateTime.Now,
                    Monto = monto,
                    Metodo = cboMetodo.SelectedItem?.ToString() ?? "Efectivo",
                    Observaciones = txtObservaciones.Text.Trim()
                };

                var id = await _pagoManager.AddPagoAsync(pago);
                MessageBox.Show($"Pago registrado con ID: {id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void TxtMonto_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Solo permitir números y backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        
        private void TxtMonto_TextChanged(object? sender, EventArgs e)
        {
            if (sender is TextBox txt && !string.IsNullOrEmpty(txt.Text))
            {
                // Remover formato anterior y mantener solo números
                string numeros = new string(txt.Text.Where(char.IsDigit).ToArray());
                
                if (!string.IsNullOrEmpty(numeros) && long.TryParse(numeros, out long valor))
                {
                    // Guardar posición del cursor
                    int cursorPos = txt.SelectionStart;
                    
                    // Formatear con puntos de miles
                    string textoFormateado = valor.ToString("N0");
                    
                    // Actualizar texto sin disparar evento recursivo
                    txt.TextChanged -= TxtMonto_TextChanged;
                    txt.Text = textoFormateado;
                    txt.TextChanged += TxtMonto_TextChanged;
                    
                    // Restaurar cursor al final
                    txt.SelectionStart = txt.Text.Length;
                }
            }
        }
        
        private void TxtMonto_Leave(object? sender, EventArgs e)
        {
            if (sender is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = "0";
            }
        }

        private void InitializeComponent()
        {
            txtBuscarSocio = new TextBox(); txtMonto = new TextBox(); txtObservaciones = new TextBox();
            cboMetodo = new ComboBox(); btnBuscar = new Button(); btnGuardar = new Button(); btnCancelar = new Button();
            lblSocioInfo = new Label();
            var lblBuscar = new Label(); var lblMonto = new Label(); var lblMetodo = new Label(); var lblObs = new Label();
            SuspendLayout();

            int y = 20;
            lblBuscar.Location = new Point(20, y); lblBuscar.Size = new Size(120, 20); lblBuscar.Text = "Buscar socio:";
            txtBuscarSocio.Location = new Point(150, y); txtBuscarSocio.Size = new Size(200, 23);
            btnBuscar.Location = new Point(360, y - 2); btnBuscar.Size = new Size(80, 35); btnBuscar.Text = "Buscar";
            btnBuscar.BackColor = Color.FromArgb(33, 150, 243); btnBuscar.ForeColor = Color.White; btnBuscar.FlatStyle = FlatStyle.Standard;
            btnBuscar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnBuscar.Click += btnBuscar_Click; y += 35;

            lblSocioInfo.Location = new Point(150, y); lblSocioInfo.Size = new Size(300, 20); lblSocioInfo.Text = "Busque un socio...";
            lblSocioInfo.ForeColor = Color.Gray; y += 35;

            lblMonto.Location = new Point(20, y); lblMonto.Size = new Size(120, 20); lblMonto.Text = "Monto ($):";
            txtMonto.Location = new Point(150, y); txtMonto.Size = new Size(200, 23);
            
            // Configurar formato de monto
            txtMonto.TextAlign = HorizontalAlignment.Right;
            txtMonto.KeyPress += TxtMonto_KeyPress;
            txtMonto.TextChanged += TxtMonto_TextChanged;
            txtMonto.Leave += TxtMonto_Leave;
            y += 35;

            lblMetodo.Location = new Point(20, y); lblMetodo.Size = new Size(120, 20); lblMetodo.Text = "Método:";
            cboMetodo.Location = new Point(150, y); cboMetodo.Size = new Size(200, 23); cboMetodo.DropDownStyle = ComboBoxStyle.DropDown;
            cboMetodo.Items.AddRange(new object[] { "Efectivo", "Tarjeta", "Transferencia" }); cboMetodo.SelectedIndex = 0;
            cboMetodo.Click += (s, e) => { if (!cboMetodo.DroppedDown) cboMetodo.DroppedDown = true; };
            y += 35;

            lblObs.Location = new Point(20, y); lblObs.Size = new Size(120, 20); lblObs.Text = "Observaciones:";
            txtObservaciones.Location = new Point(150, y); txtObservaciones.Size = new Size(290, 50); txtObservaciones.Multiline = true; y += 65;

            btnGuardar.Location = new Point(150, y); btnGuardar.Size = new Size(90, 35); btnGuardar.Text = "Guardar";
            btnGuardar.BackColor = Color.FromArgb(76, 175, 80); btnGuardar.ForeColor = Color.White; btnGuardar.FlatStyle = FlatStyle.Standard;
            btnGuardar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnGuardar.Click += btnGuardar_Click;

            btnCancelar.Location = new Point(250, y); btnCancelar.Size = new Size(90, 35); btnCancelar.Text = "Cancelar";
            btnCancelar.BackColor = Color.FromArgb(158, 158, 158); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Standard;
            btnCancelar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblBuscar, txtBuscarSocio, btnBuscar, lblSocioInfo, lblMonto, txtMonto, lblMetodo, cboMetodo, lblObs, txtObservaciones, btnGuardar, btnCancelar });
            ClientSize = new Size(470, y + 60); FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false; StartPosition = FormStartPosition.CenterParent; Text = "Registrar Pago";
            ResumeLayout(false);
        }
    }
}
