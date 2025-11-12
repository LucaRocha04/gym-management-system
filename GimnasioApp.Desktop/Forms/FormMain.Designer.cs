namespace GimnasioApp.Desktop.Forms
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem sociosToolStripMenuItem;
        private ToolStripMenuItem planesToolStripMenuItem;
        private ToolStripMenuItem pagosToolStripMenuItem;
        private ToolStripMenuItem asistenciasToolStripMenuItem;
        private ToolStripMenuItem reportesToolStripMenuItem;
        private ToolStripMenuItem usuariosToolStripMenuItem;
        private ToolStripMenuItem accionesRapidasToolStripMenuItem;
        private ToolStripMenuItem consultasToolStripMenuItem;
        private ToolStripMenuItem salirToolStripMenuItem;
        private Label lblBienvenida;
        private ToolStripMenuItem gestionarSociosToolStripMenuItem;
        private ToolStripMenuItem gestionarPlanesToolStripMenuItem;
        private ToolStripMenuItem registrarPagoToolStripMenuItem;
        private ToolStripMenuItem registrarAsistenciaToolStripMenuItem;
        private ToolStripMenuItem verReportesToolStripMenuItem;
        private ToolStripMenuItem gestionarUsuariosToolStripMenuItem;
        private ToolStripMenuItem registrarEntradaToolStripMenuItem;
        private ToolStripMenuItem registrarSalidaToolStripMenuItem;
        private ToolStripMenuItem verSociosActivosToolStripMenuItem;
        private ToolStripMenuItem verPlanesToolStripMenuItem;
    private ToolStripMenuItem clasesToolStripMenuItem;
    private ToolStripMenuItem gestionarMisClasesToolStripMenuItem;
    private ToolStripMenuItem inscribirEnClasesToolStripMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.menuStrip1 = new MenuStrip();
            this.sociosToolStripMenuItem = new ToolStripMenuItem();
            this.gestionarSociosToolStripMenuItem = new ToolStripMenuItem();
            this.planesToolStripMenuItem = new ToolStripMenuItem();
            this.gestionarPlanesToolStripMenuItem = new ToolStripMenuItem();
            this.pagosToolStripMenuItem = new ToolStripMenuItem();
            this.registrarPagoToolStripMenuItem = new ToolStripMenuItem();
            this.asistenciasToolStripMenuItem = new ToolStripMenuItem();
            this.registrarAsistenciaToolStripMenuItem = new ToolStripMenuItem();
            this.reportesToolStripMenuItem = new ToolStripMenuItem();
            this.verReportesToolStripMenuItem = new ToolStripMenuItem();
            this.usuariosToolStripMenuItem = new ToolStripMenuItem();
            this.gestionarUsuariosToolStripMenuItem = new ToolStripMenuItem();
            this.accionesRapidasToolStripMenuItem = new ToolStripMenuItem();
            this.registrarEntradaToolStripMenuItem = new ToolStripMenuItem();
            this.registrarSalidaToolStripMenuItem = new ToolStripMenuItem();
            this.consultasToolStripMenuItem = new ToolStripMenuItem();
            this.verSociosActivosToolStripMenuItem = new ToolStripMenuItem();
            this.verPlanesToolStripMenuItem = new ToolStripMenuItem();
            this.salirToolStripMenuItem = new ToolStripMenuItem();
            this.lblBienvenida = new Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            
            // menuStrip1
            this.menuStrip1.Items.AddRange(new ToolStripItem[] {
                this.sociosToolStripMenuItem,
                this.planesToolStripMenuItem,
                this.pagosToolStripMenuItem,
                this.asistenciasToolStripMenuItem,
                this.reportesToolStripMenuItem,
                this.usuariosToolStripMenuItem,
                this.accionesRapidasToolStripMenuItem,
                this.consultasToolStripMenuItem,
                this.salirToolStripMenuItem});
            this.menuStrip1.Location = new Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new Size(1000, 24);
            this.menuStrip1.TabIndex = 0;
            
            // sociosToolStripMenuItem
            this.sociosToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.gestionarSociosToolStripMenuItem});
            this.sociosToolStripMenuItem.Name = "sociosToolStripMenuItem";
            this.sociosToolStripMenuItem.Size = new Size(55, 20);
            this.sociosToolStripMenuItem.Text = "Socios";
            
            // gestionarSociosToolStripMenuItem
            this.gestionarSociosToolStripMenuItem.Name = "gestionarSociosToolStripMenuItem";
            this.gestionarSociosToolStripMenuItem.Size = new Size(180, 22);
            this.gestionarSociosToolStripMenuItem.Text = "Gestionar Socios";
            this.gestionarSociosToolStripMenuItem.Click += new EventHandler(this.gestionarSociosToolStripMenuItem_Click);
            
            // planesToolStripMenuItem
            this.planesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.gestionarPlanesToolStripMenuItem});
            this.planesToolStripMenuItem.Name = "planesToolStripMenuItem";
            this.planesToolStripMenuItem.Size = new Size(53, 20);
            this.planesToolStripMenuItem.Text = "Planes";
            
            // gestionarPlanesToolStripMenuItem
            this.gestionarPlanesToolStripMenuItem.Name = "gestionarPlanesToolStripMenuItem";
            this.gestionarPlanesToolStripMenuItem.Size = new Size(180, 22);
            this.gestionarPlanesToolStripMenuItem.Text = "Gestionar Planes";
            this.gestionarPlanesToolStripMenuItem.Click += new EventHandler(this.gestionarPlanesToolStripMenuItem_Click);
            
            // pagosToolStripMenuItem
            this.pagosToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.registrarPagoToolStripMenuItem});
            this.pagosToolStripMenuItem.Name = "pagosToolStripMenuItem";
            this.pagosToolStripMenuItem.Size = new Size(51, 20);
            this.pagosToolStripMenuItem.Text = "Pagos";
            
            // registrarPagoToolStripMenuItem
            this.registrarPagoToolStripMenuItem.Name = "registrarPagoToolStripMenuItem";
            this.registrarPagoToolStripMenuItem.Size = new Size(180, 22);
            this.registrarPagoToolStripMenuItem.Text = "Gestionar Pagos";
            this.registrarPagoToolStripMenuItem.Click += new EventHandler(this.registrarPagoToolStripMenuItem_Click);
            
            // asistenciasToolStripMenuItem
            this.asistenciasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.registrarAsistenciaToolStripMenuItem});
            this.asistenciasToolStripMenuItem.Name = "asistenciasToolStripMenuItem";
            this.asistenciasToolStripMenuItem.Size = new Size(79, 20);
            this.asistenciasToolStripMenuItem.Text = "Asistencias";
            
            // registrarAsistenciaToolStripMenuItem
            this.registrarAsistenciaToolStripMenuItem.Name = "registrarAsistenciaToolStripMenuItem";
            this.registrarAsistenciaToolStripMenuItem.Size = new Size(180, 22);
            this.registrarAsistenciaToolStripMenuItem.Text = "Gestionar Asistencias";
            this.registrarAsistenciaToolStripMenuItem.Click += new EventHandler(this.registrarAsistenciaToolStripMenuItem_Click);
            
            // reportesToolStripMenuItem
            this.reportesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.verReportesToolStripMenuItem});
            this.reportesToolStripMenuItem.Name = "reportesToolStripMenuItem";
            this.reportesToolStripMenuItem.Size = new Size(65, 20);
            this.reportesToolStripMenuItem.Text = "Reportes";
            
            // verReportesToolStripMenuItem
            this.verReportesToolStripMenuItem.Name = "verReportesToolStripMenuItem";
            this.verReportesToolStripMenuItem.Size = new Size(180, 22);
            this.verReportesToolStripMenuItem.Text = "Ver Reportes";
            this.verReportesToolStripMenuItem.Click += new EventHandler(this.verReportesToolStripMenuItem_Click);
            
            // usuariosToolStripMenuItem
            this.usuariosToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.gestionarUsuariosToolStripMenuItem});
            this.usuariosToolStripMenuItem.Name = "usuariosToolStripMenuItem";
            this.usuariosToolStripMenuItem.Size = new Size(64, 20);
            this.usuariosToolStripMenuItem.Text = "Usuarios";
            
            // gestionarUsuariosToolStripMenuItem
            this.gestionarUsuariosToolStripMenuItem.Name = "gestionarUsuariosToolStripMenuItem";
            this.gestionarUsuariosToolStripMenuItem.Size = new Size(180, 22);
            this.gestionarUsuariosToolStripMenuItem.Text = "Gestionar Usuarios";
            this.gestionarUsuariosToolStripMenuItem.Click += new EventHandler(this.gestionarUsuariosToolStripMenuItem_Click);
            
            // accionesRapidasToolStripMenuItem
            this.accionesRapidasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.registrarEntradaToolStripMenuItem,
                this.registrarSalidaToolStripMenuItem});
            this.accionesRapidasToolStripMenuItem.Name = "accionesRapidasToolStripMenuItem";
            this.accionesRapidasToolStripMenuItem.Size = new Size(114, 20);
            this.accionesRapidasToolStripMenuItem.Text = "Acciones Rápidas";
            
            // registrarEntradaToolStripMenuItem
            this.registrarEntradaToolStripMenuItem.Name = "registrarEntradaToolStripMenuItem";
            this.registrarEntradaToolStripMenuItem.Size = new Size(180, 22);
            this.registrarEntradaToolStripMenuItem.Text = "Registrar Entrada";
            this.registrarEntradaToolStripMenuItem.Click += new EventHandler(this.registrarEntradaToolStripMenuItem_Click);
            
            // registrarSalidaToolStripMenuItem
            this.registrarSalidaToolStripMenuItem.Name = "registrarSalidaToolStripMenuItem";
            this.registrarSalidaToolStripMenuItem.Size = new Size(180, 22);
            this.registrarSalidaToolStripMenuItem.Text = "Registrar Salida";
            this.registrarSalidaToolStripMenuItem.Click += new EventHandler(this.registrarSalidaToolStripMenuItem_Click);
            
            // consultasToolStripMenuItem
            this.consultasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.verSociosActivosToolStripMenuItem,
                this.verPlanesToolStripMenuItem});
            this.consultasToolStripMenuItem.Name = "consultasToolStripMenuItem";
            this.consultasToolStripMenuItem.Size = new Size(71, 20);
            this.consultasToolStripMenuItem.Text = "Consultas";
            
            // verSociosActivosToolStripMenuItem
            this.verSociosActivosToolStripMenuItem.Name = "verSociosActivosToolStripMenuItem";
            this.verSociosActivosToolStripMenuItem.Size = new Size(180, 22);
            this.verSociosActivosToolStripMenuItem.Text = "Ver Socios Activos";
            this.verSociosActivosToolStripMenuItem.Click += new EventHandler(this.verSociosActivosToolStripMenuItem_Click);
            
            // verPlanesToolStripMenuItem
            this.verPlanesToolStripMenuItem.Name = "verPlanesToolStripMenuItem";
            this.verPlanesToolStripMenuItem.Size = new Size(180, 22);
            this.verPlanesToolStripMenuItem.Text = "Ver Planes";
            this.verPlanesToolStripMenuItem.Click += new EventHandler(this.verPlanesToolStripMenuItem_Click);

            // clasesToolStripMenuItem
            this.clasesToolStripMenuItem = new ToolStripMenuItem();
            this.gestionarMisClasesToolStripMenuItem = new ToolStripMenuItem();
            this.inscribirEnClasesToolStripMenuItem = new ToolStripMenuItem();
            this.clasesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.gestionarMisClasesToolStripMenuItem,
                this.inscribirEnClasesToolStripMenuItem});
            this.clasesToolStripMenuItem.Name = "clasesToolStripMenuItem";
            this.clasesToolStripMenuItem.Size = new Size(53, 20);
            this.clasesToolStripMenuItem.Text = "Clases";
            // gestionarMisClasesToolStripMenuItem
            this.gestionarMisClasesToolStripMenuItem.Name = "gestionarMisClasesToolStripMenuItem";
            this.gestionarMisClasesToolStripMenuItem.Size = new Size(188, 22);
            this.gestionarMisClasesToolStripMenuItem.Text = "Gestionar mis clases";
            this.gestionarMisClasesToolStripMenuItem.Click += new EventHandler(this.gestionarMisClasesToolStripMenuItem_Click);
            // inscribirEnClasesToolStripMenuItem
            this.inscribirEnClasesToolStripMenuItem.Name = "inscribirEnClasesToolStripMenuItem";
            this.inscribirEnClasesToolStripMenuItem.Size = new Size(188, 22);
            this.inscribirEnClasesToolStripMenuItem.Text = "Inscribir a clases";
            this.inscribirEnClasesToolStripMenuItem.Click += new EventHandler(this.inscribirEnClasesToolStripMenuItem_Click);
            // Agregar 'Clases' al menú una vez creado
            this.menuStrip1.Items.Add(this.clasesToolStripMenuItem);
            
            // salirToolStripMenuItem
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new Size(41, 20);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Click += new EventHandler(this.salirToolStripMenuItem_Click);
            
            // lblBienvenida
            this.lblBienvenida.AutoSize = true;
            this.lblBienvenida.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblBienvenida.Location = new Point(20, 40);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.Size = new Size(300, 25);
            this.lblBienvenida.TabIndex = 1;
            this.lblBienvenida.Text = "Bienvenido al Sistema";
            
            // FormMain
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 600);
            this.Controls.Add(this.lblBienvenida);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Sistema de Gestión - Gimnasio";
            this.WindowState = FormWindowState.Maximized;
            this.Load += new EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
