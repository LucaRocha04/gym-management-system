using GimnasioApp.Managers;
using GimnasioApp.Models;
using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
	public partial class FormUsuarios : Form
	{
		private readonly UsuarioManager _usuarioManager = new();
		private DataGridView dgvUsuarios = null!;
		private Button btnAgregar = null!, btnCambiarRol = null!, btnResetPass = null!, btnEliminar = null!, btnRefrescar = null!;
		private Panel panelTop = null!, panelBottom = null!;
		private Label lblTitulo = null!;

		public FormUsuarios()
		{
			InitializeComponent();
			UITheme.Apply(this);
		}

		private async void FormUsuarios_Load(object? sender, EventArgs e)
		{
			await CargarUsuarios();
		}

		private async Task CargarUsuarios()
		{
			try
			{
				var usuarios = await _usuarioManager.GetAllAsync();
				var data = usuarios.Select(u => new
				{
					ID = u.Id,
					Usuario = u.NombreUsuario,
					Mail = string.IsNullOrWhiteSpace(u.Mail) ? "-" : u.Mail,
					Rol = u.Rol
				}).ToList();

				dgvUsuarios.DataSource = null;
				dgvUsuarios.DataSource = data;
				if (dgvUsuarios.Columns.Count > 0)
				{
					dgvUsuarios.Columns["ID"]!.Width = 60;
					dgvUsuarios.Columns["Usuario"]!.Width = 200;
					dgvUsuarios.Columns["Mail"]!.Width = 220;
					dgvUsuarios.Columns["Rol"]!.Width = 140;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void btnRefrescar_Click(object? sender, EventArgs e)
		{
			await CargarUsuarios();
		}

		private void btnAgregar_Click(object? sender, EventArgs e)
		{
			using var form = new FormUsuarioDetalle();
			if (form.ShowDialog() == DialogResult.OK)
			{
				_ = CargarUsuarios();
			}
		}

		private async void btnCambiarRol_Click(object? sender, EventArgs e)
		{
			if (dgvUsuarios.CurrentRow == null)
			{
				MessageBox.Show("Seleccione un usuario", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			int id = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells["ID"]!.Value);
			string actual = dgvUsuarios.CurrentRow.Cells["Rol"]!.Value?.ToString() ?? "Recepcionista";

			using var form = new FormCambiarRol(actual);
			if (form.ShowDialog() == DialogResult.OK)
			{
				try
				{
					await _usuarioManager.UpdateRolAsync(id, form.NuevoRol);
					await CargarUsuarios();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private async void btnResetPass_Click(object? sender, EventArgs e)
		{
			if (dgvUsuarios.CurrentRow == null)
			{
				MessageBox.Show("Seleccione un usuario", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			int id = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells["ID"]!.Value);
			string usuario = dgvUsuarios.CurrentRow.Cells["Usuario"]!.Value?.ToString() ?? "";

			using var form = new FormNuevaPassword(usuario);
			if (form.ShowDialog() == DialogResult.OK)
			{
				try
				{
					await _usuarioManager.ResetPasswordAsync(id, form.PasswordNueva);
					MessageBox.Show("Contraseña actualizada", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private async void btnEliminar_Click(object? sender, EventArgs e)
		{
			if (dgvUsuarios.CurrentRow == null)
			{
				MessageBox.Show("Seleccione un usuario", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			int id = Convert.ToInt32(dgvUsuarios.CurrentRow.Cells["ID"]!.Value);
			string usuario = dgvUsuarios.CurrentRow.Cells["Usuario"]!.Value?.ToString() ?? "";

			if (MessageBox.Show($"¿Eliminar permanentemente el usuario '{usuario}'?", "Confirmar eliminación", 
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				try
				{
					await _usuarioManager.EliminarAsync(id);
					MessageBox.Show("Usuario eliminado", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
					await CargarUsuarios();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void InitializeComponent()
		{
			this.dgvUsuarios = new DataGridView();
			this.btnAgregar = new Button();
			this.btnCambiarRol = new Button();
			this.btnResetPass = new Button();
			this.btnEliminar = new Button();
			this.btnRefrescar = new Button();
			this.panelTop = new Panel();
			this.panelBottom = new Panel();
			this.lblTitulo = new Label();
			((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
			this.panelTop.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();

			// panelTop
			this.panelTop.BackColor = Color.FromArgb(63, 81, 181);
			this.panelTop.Controls.Add(this.lblTitulo);
			this.panelTop.Dock = DockStyle.Top;
			this.panelTop.Size = new Size(900, 60);

			this.lblTitulo.ForeColor = Color.White; this.lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
			this.lblTitulo.Location = new Point(15, 15); this.lblTitulo.Size = new Size(400, 30); this.lblTitulo.Text = "Gestión de Usuarios";

			// dgvUsuarios
			this.dgvUsuarios.AllowUserToAddRows = false;
			this.dgvUsuarios.AllowUserToDeleteRows = false;
			this.dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvUsuarios.BackgroundColor = Color.White;
			this.dgvUsuarios.Dock = DockStyle.Fill;
			this.dgvUsuarios.ReadOnly = true;
			this.dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

			// panelBottom
			this.panelBottom.BackColor = Color.FromArgb(245, 245, 245);
			this.panelBottom.Controls.Add(this.btnAgregar);
			this.panelBottom.Controls.Add(this.btnCambiarRol);
			this.panelBottom.Controls.Add(this.btnResetPass);
			this.panelBottom.Controls.Add(this.btnEliminar);
			this.panelBottom.Controls.Add(this.btnRefrescar);
			this.panelBottom.Dock = DockStyle.Bottom;
			this.panelBottom.Size = new Size(900, 60);

			this.btnAgregar.Location = new Point(15, 13); this.btnAgregar.Size = new Size(130, 45); this.btnAgregar.Text = "Nuevo Usuario";
			this.btnAgregar.BackColor = Color.FromArgb(76, 175, 80); this.btnAgregar.ForeColor = Color.White; this.btnAgregar.FlatStyle = FlatStyle.Standard;
			this.btnAgregar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnAgregar.Click += btnAgregar_Click;

			this.btnCambiarRol.Location = new Point(155, 13); this.btnCambiarRol.Size = new Size(130, 45); this.btnCambiarRol.Text = "Cambiar Rol";
			this.btnCambiarRol.BackColor = Color.FromArgb(33, 150, 243); this.btnCambiarRol.ForeColor = Color.White; this.btnCambiarRol.FlatStyle = FlatStyle.Standard;
			this.btnCambiarRol.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnCambiarRol.Click += btnCambiarRol_Click;

			this.btnResetPass.Location = new Point(295, 13); this.btnResetPass.Size = new Size(150, 45); this.btnResetPass.Text = "Reset Contraseña";
			this.btnResetPass.BackColor = Color.FromArgb(255, 87, 34); this.btnResetPass.ForeColor = Color.White; this.btnResetPass.FlatStyle = FlatStyle.Standard;
			this.btnResetPass.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnResetPass.Click += btnResetPass_Click;

			this.btnEliminar.Location = new Point(455, 13); this.btnEliminar.Size = new Size(110, 45); this.btnEliminar.Text = "Eliminar";
			this.btnEliminar.BackColor = Color.FromArgb(244, 67, 54); this.btnEliminar.ForeColor = Color.White; this.btnEliminar.FlatStyle = FlatStyle.Standard;
			this.btnEliminar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnEliminar.Click += btnEliminar_Click;

			this.btnRefrescar.Location = new Point(575, 13); this.btnRefrescar.Size = new Size(110, 45); this.btnRefrescar.Text = "Refrescar";
			this.btnRefrescar.BackColor = Color.FromArgb(158, 158, 158); this.btnRefrescar.ForeColor = Color.White; this.btnRefrescar.FlatStyle = FlatStyle.Standard;
			this.btnRefrescar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
			this.btnRefrescar.Click += btnRefrescar_Click;

			// FormUsuarios
			this.ClientSize = new Size(900, 500);
			this.Controls.Add(this.dgvUsuarios);
			this.Controls.Add(this.panelTop);
			this.Controls.Add(this.panelBottom);
			this.Name = "FormUsuarios";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Gestión de Usuarios";
			this.Load += FormUsuarios_Load;
			((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
			this.panelTop.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);
		}
	}

	internal class FormUsuarioDetalle : Form
	{
		private readonly UsuarioManager _usuarioManager = new();
		private TextBox txtUsuario = null!, txtMail = null!, txtPassword = null!;
		private ComboBox cboRol = null!;
		private Button btnGuardar = null!, btnCancelar = null!;

		public FormUsuarioDetalle()
		{
			InitializeComponent();
		}

		private async void btnGuardar_Click(object? sender, EventArgs e)
		{
			string user = txtUsuario.Text.Trim();
			string mail = txtMail.Text.Trim();
			string pass = txtPassword.Text.Trim();
			string rol = cboRol.SelectedItem?.ToString() ?? "Recepcionista";

			if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
			{
				MessageBox.Show("Usuario y contraseña son obligatorios", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				if (await _usuarioManager.ExistsByNombreAsync(user))
				{
					MessageBox.Show("El nombre de usuario ya existe", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				var u = new Usuario { NombreUsuario = user, Mail = mail, Password = pass, Rol = rol };
				var id = await _usuarioManager.AddAsync(u);
				MessageBox.Show($"Usuario creado con ID: {id}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
				DialogResult = DialogResult.OK; Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void InitializeComponent()
		{
			txtUsuario = new TextBox(); txtMail = new TextBox(); txtPassword = new TextBox();
			cboRol = new ComboBox(); btnGuardar = new Button(); btnCancelar = new Button();
			var lblUsuario = new Label(); var lblMail = new Label(); var lblPass = new Label(); var lblRol = new Label();
			SuspendLayout();

			int y = 20;
			lblUsuario.Location = new Point(20, y); lblUsuario.Size = new Size(120, 20); lblUsuario.Text = "Usuario:";
			txtUsuario.Location = new Point(150, y); txtUsuario.Size = new Size(220, 23); y += 35;

			lblMail.Location = new Point(20, y); lblMail.Size = new Size(120, 20); lblMail.Text = "Mail:";
			txtMail.Location = new Point(150, y); txtMail.Size = new Size(220, 23); y += 35;

			lblPass.Location = new Point(20, y); lblPass.Size = new Size(120, 20); lblPass.Text = "Contraseña:";
			txtPassword.Location = new Point(150, y); txtPassword.Size = new Size(220, 23); txtPassword.UseSystemPasswordChar = true; y += 35;

			lblRol.Location = new Point(20, y); lblRol.Size = new Size(120, 20); lblRol.Text = "Rol:";
			cboRol.Location = new Point(150, y); cboRol.Size = new Size(220, 23); cboRol.DropDownStyle = ComboBoxStyle.DropDownList;
			cboRol.Items.AddRange(new object[] { "Administrador", "Recepcionista", "Profesor" }); cboRol.SelectedIndex = 1; y += 45;

			btnGuardar.Location = new Point(150, y); btnGuardar.Size = new Size(120, 32); btnGuardar.Text = "💾 Guardar";
			btnGuardar.BackColor = Color.FromArgb(76, 175, 80); btnGuardar.ForeColor = Color.White; btnGuardar.FlatStyle = FlatStyle.Flat;
			btnGuardar.Click += btnGuardar_Click;

			btnCancelar.Location = new Point(280, y); btnCancelar.Size = new Size(120, 32); btnCancelar.Text = "❌ Cancelar";
			btnCancelar.BackColor = Color.FromArgb(158, 158, 158); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Flat;
			btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

			Controls.AddRange(new Control[] { lblUsuario, txtUsuario, lblMail, txtMail, lblPass, txtPassword, lblRol, cboRol, btnGuardar, btnCancelar });
			ClientSize = new Size(410, y + 60); FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false; MinimizeBox = false; StartPosition = FormStartPosition.CenterParent; Text = "Nuevo Usuario";
			ResumeLayout(false);
		}
	}

	internal class FormCambiarRol : Form
	{
		private ComboBox cboRol = null!;
		private Button btnGuardar = null!, btnCancelar = null!;
		public string NuevoRol { get; private set; } = "Recepcionista";

		public FormCambiarRol(string rolActual)
		{
			InitializeComponent();
			cboRol.SelectedItem = rolActual ?? "Recepcionista";
		}

		private void btnGuardar_Click(object? sender, EventArgs e)
		{
			NuevoRol = cboRol.SelectedItem?.ToString() ?? "Recepcionista";
			DialogResult = DialogResult.OK; Close();
		}

		private void InitializeComponent()
		{
			cboRol = new ComboBox(); btnGuardar = new Button(); btnCancelar = new Button(); var lblRol = new Label();
			SuspendLayout();

			lblRol.Location = new Point(20, 20); lblRol.Size = new Size(120, 20); lblRol.Text = "Nuevo Rol:";
			cboRol.Location = new Point(150, 20); cboRol.Size = new Size(220, 23); cboRol.DropDownStyle = ComboBoxStyle.DropDownList;
			cboRol.Items.AddRange(new object[] { "Administrador", "Recepcionista", "Profesor" }); cboRol.SelectedIndex = 1;

			btnGuardar.Location = new Point(150, 60); btnGuardar.Size = new Size(120, 32); btnGuardar.Text = "💾 Guardar";
			btnGuardar.BackColor = Color.FromArgb(76, 175, 80); btnGuardar.ForeColor = Color.White; btnGuardar.FlatStyle = FlatStyle.Flat;
			btnGuardar.Click += btnGuardar_Click;

			btnCancelar.Location = new Point(280, 60); btnCancelar.Size = new Size(120, 32); btnCancelar.Text = "❌ Cancelar";
			btnCancelar.BackColor = Color.FromArgb(158, 158, 158); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Flat;
			btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

			Controls.AddRange(new Control[] { lblRol, cboRol, btnGuardar, btnCancelar });
			ClientSize = new Size(410, 110); FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false; MinimizeBox = false; StartPosition = FormStartPosition.CenterParent; Text = "Cambiar Rol";
			ResumeLayout(false);
		}
	}

	internal class FormNuevaPassword : Form
	{
		private TextBox txtPass = null!;
		private Button btnGuardar = null!, btnCancelar = null!;
		public string PasswordNueva { get; private set; } = string.Empty;

		public FormNuevaPassword(string usuario)
		{
			InitializeComponent(usuario);
		}

		private void btnGuardar_Click(object? sender, EventArgs e)
		{
			var pass = txtPass.Text.Trim();
			if (string.IsNullOrWhiteSpace(pass))
			{
				MessageBox.Show("Ingrese una contraseña", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			PasswordNueva = pass; DialogResult = DialogResult.OK; Close();
		}

		private void InitializeComponent(string usuario)
		{
			txtPass = new TextBox(); btnGuardar = new Button(); btnCancelar = new Button(); var lbl = new Label();
			SuspendLayout();

			lbl.Location = new Point(20, 20); lbl.Size = new Size(350, 20); lbl.Text = $"Nueva contraseña para {usuario}:";
			txtPass.Location = new Point(20, 45); txtPass.Size = new Size(350, 23); txtPass.UseSystemPasswordChar = true;

			btnGuardar.Location = new Point(20, 80); btnGuardar.Size = new Size(120, 32); btnGuardar.Text = "💾 Guardar";
			btnGuardar.BackColor = Color.FromArgb(76, 175, 80); btnGuardar.ForeColor = Color.White; btnGuardar.FlatStyle = FlatStyle.Flat;
			btnGuardar.Click += btnGuardar_Click;

			btnCancelar.Location = new Point(150, 80); btnCancelar.Size = new Size(120, 32); btnCancelar.Text = "❌ Cancelar";
			btnCancelar.BackColor = Color.FromArgb(158, 158, 158); btnCancelar.ForeColor = Color.White; btnCancelar.FlatStyle = FlatStyle.Flat;
			btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

			Controls.AddRange(new Control[] { lbl, txtPass, btnGuardar, btnCancelar });
			ClientSize = new Size(400, 130); FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false; MinimizeBox = false; StartPosition = FormStartPosition.CenterParent; Text = "Resetear Contraseña";
			ResumeLayout(false);
		}
	}
}
