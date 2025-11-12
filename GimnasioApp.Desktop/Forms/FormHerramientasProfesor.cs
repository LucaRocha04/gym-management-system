using System;
using System.Drawing;
using System.Windows.Forms;
using GimnasioApp.Models;

namespace GimnasioApp.Desktop.Forms
{
    public partial class FormHerramientasProfesor : Form
    {
        private readonly Usuario _profesor;
        private TabControl tabControl = null!;
        private TabPage tabIMC = null!;
        private TabPage tabRutinas = null!;

        // Controles para IMC
        private TextBox txtPeso = null!;
        private TextBox txtAltura = null!;
        private Label lblResultadoIMC = null!;
        private Label lblClasificacion = null!;
        private Button btnCalcularIMC = null!;

        public FormHerramientasProfesor(Usuario profesor)
        {
            _profesor = profesor;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Herramientas del Profesor - " + _profesor.NombreUsuario;
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 252);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // TabControl principal
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10F);

            CreateTabIMC();
            CreateTabRutinas();

            this.Controls.Add(tabControl);
        }

        private void CreateTabIMC()
        {
            tabIMC = new TabPage("📊 Calculadora IMC");
            tabIMC.BackColor = Color.FromArgb(248, 249, 252);
            tabIMC.Padding = new Padding(20);

            // Panel principal
            var mainPanel = new Panel();
            mainPanel.Location = new Point(20, 20);
            mainPanel.Size = new Size(740, 480);
            mainPanel.BackColor = Color.White;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;

            // Título
            var lblTitulo = new Label();
            lblTitulo.Text = "Calculadora de Índice de Masa Corporal (IMC)";
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(52, 152, 219);
            lblTitulo.Location = new Point(30, 30);
            lblTitulo.Size = new Size(600, 30);

            // Peso
            var lblPeso = new Label();
            lblPeso.Text = "Peso (kg):";
            lblPeso.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblPeso.Location = new Point(50, 100);
            lblPeso.Size = new Size(100, 25);

            txtPeso = new TextBox();
            txtPeso.Location = new Point(150, 98);
            txtPeso.Size = new Size(150, 25);
            txtPeso.Font = new Font("Segoe UI", 11F);

            // Altura
            var lblAltura = new Label();
            lblAltura.Text = "Altura (m):";
            lblAltura.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblAltura.Location = new Point(50, 140);
            lblAltura.Size = new Size(100, 25);

            txtAltura = new TextBox();
            txtAltura.Location = new Point(150, 138);
            txtAltura.Size = new Size(150, 25);
            txtAltura.Font = new Font("Segoe UI", 11F);

            // Botón calcular
            btnCalcularIMC = new Button();
            btnCalcularIMC.Text = "Calcular IMC";
            btnCalcularIMC.Location = new Point(50, 190);
            btnCalcularIMC.Size = new Size(150, 40);
            btnCalcularIMC.BackColor = Color.FromArgb(52, 152, 219);
            btnCalcularIMC.ForeColor = Color.White;
            btnCalcularIMC.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCalcularIMC.FlatStyle = FlatStyle.Flat;
            btnCalcularIMC.Click += BtnCalcularIMC_Click;

            // Resultado
            var lblTextoResultado = new Label();
            lblTextoResultado.Text = "Resultado:";
            lblTextoResultado.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTextoResultado.Location = new Point(50, 268);
            lblTextoResultado.Size = new Size(120, 25);

            lblResultadoIMC = new Label();
            lblResultadoIMC.Text = "---";
            lblResultadoIMC.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblResultadoIMC.ForeColor = Color.FromArgb(46, 204, 113);
            lblResultadoIMC.Location = new Point(180, 260);
            lblResultadoIMC.Size = new Size(170, 50);
            lblResultadoIMC.TextAlign = ContentAlignment.MiddleLeft;

            lblClasificacion = new Label();
            lblClasificacion.Text = "";
            lblClasificacion.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblClasificacion.Location = new Point(50, 305);
            lblClasificacion.Size = new Size(600, 50);
            lblClasificacion.ForeColor = Color.FromArgb(44, 62, 80);

            // Panel de información
            var panelInfo = CreateIMCInfoPanel();
            panelInfo.Location = new Point(400, 90);
            panelInfo.Size = new Size(300, 200);

            mainPanel.Controls.AddRange(new Control[] {
                lblTitulo, lblPeso, txtPeso, lblAltura, txtAltura,
                btnCalcularIMC, lblTextoResultado, lblResultadoIMC,
                lblClasificacion, panelInfo
            });

            tabIMC.Controls.Add(mainPanel);
            tabControl.TabPages.Add(tabIMC);
        }

        private Panel CreateIMCInfoPanel()
        {
            var panel = new Panel();
            panel.BackColor = Color.FromArgb(236, 240, 241);
            panel.BorderStyle = BorderStyle.FixedSingle;

            var lblInfo = new Label();
            lblInfo.Text = "Clasificación IMC:\n\n" +
                          "• Bajo peso: < 18.5\n" +
                          "• Normal: 18.5 - 24.9\n" +
                          "• Sobrepeso: 25.0 - 29.9\n" +
                          "• Obesidad I: 30.0 - 34.9\n" +
                          "• Obesidad II: 35.0 - 39.9\n" +
                          "• Obesidad III: ≥ 40.0";
            lblInfo.Font = new Font("Segoe UI", 9F);
            lblInfo.ForeColor = Color.FromArgb(44, 62, 80);
            lblInfo.Location = new Point(10, 10);
            lblInfo.Size = new Size(280, 180);

            panel.Controls.Add(lblInfo);
            return panel;
        }



        private void CreateTabRutinas()
        {
            tabRutinas = new TabPage("💪 Consejos y Rutinas");
            tabRutinas.BackColor = Color.FromArgb(248, 249, 252);
            tabRutinas.Padding = new Padding(20);

            var mainPanel = new Panel();
            mainPanel.Location = new Point(20, 20);
            mainPanel.Size = new Size(740, 480);
            mainPanel.BackColor = Color.White;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;

            // Título
            var lblTitulo = new Label();
            lblTitulo.Text = "Guías Rápidas de Entrenamiento";
            lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(155, 89, 182);
            lblTitulo.Location = new Point(30, 30);
            lblTitulo.Size = new Size(600, 30);

            // RichTextBox con información
            var rtbConsejos = new RichTextBox();
            rtbConsejos.Location = new Point(30, 80);
            rtbConsejos.Size = new Size(680, 370);
            rtbConsejos.Font = new Font("Segoe UI", 10F);
            rtbConsejos.ReadOnly = true;
            rtbConsejos.BackColor = Color.FromArgb(250, 250, 250);
            
            rtbConsejos.Text = @"🏋️ RUTINAS BÁSICAS POR OBJETIVO

📈 PÉRDIDA DE PESO:
• Cardio: 30-45 min, 3-5 veces por semana
• Entrenamiento de fuerza: 2-3 veces por semana
• Déficit calórico: 300-500 calorías/día
• Hidratación: 2-3 litros de agua/día

💪 GANANCIA MUSCULAR:
• Entrenamiento de fuerza: 4-5 veces por semana
• Cardio ligero: 2-3 veces por semana (20-30 min)
• Superávit calórico: 200-500 calorías/día
• Proteína: 1.6-2.2g por kg de peso corporal

🏃 RESISTENCIA:
• Cardio prolongado: 45-60 min, 4-5 veces por semana
• Entrenamiento por intervalos: 2 veces por semana
• Ejercicios funcionales: 2-3 veces por semana
• Carbohidratos complejos como base energética

⚖️ MANTENIMIENTO:
• Ejercicio mixto: 3-4 veces por semana
• Combinar cardio (20-30 min) + fuerza (30-40 min)
• Dieta equilibrada con todas las macronutrientes
• Descanso adecuado: 7-8 horas de sueño

🎯 TIPS GENERALES:
• Calentamiento: 5-10 minutos antes del ejercicio
• Enfriamiento: 5-10 minutos después del ejercicio
• Progresión gradual: Aumentar intensidad 10% semanal
• Descanso: 48-72h entre entrenamientos del mismo grupo muscular
• Variación: Cambiar rutina cada 4-6 semanas para evitar adaptación";

            mainPanel.Controls.AddRange(new Control[] { lblTitulo, rtbConsejos });

            tabRutinas.Controls.Add(mainPanel);
            tabControl.TabPages.Add(tabRutinas);
        }

        private void BtnCalcularIMC_Click(object? sender, EventArgs e)
        {
            try
            {
                // Limpiar resultados previos
                lblResultadoIMC.Text = "---";
                lblClasificacion.Text = "";

                // Validar que los campos no estén vacíos
                if (string.IsNullOrWhiteSpace(txtPeso.Text) || string.IsNullOrWhiteSpace(txtAltura.Text))
                {
                    MessageBox.Show("Por favor, ingrese el peso y la altura.", "Datos incompletos", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validar peso
                if (!decimal.TryParse(txtPeso.Text.Replace(",", "."), System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out decimal peso) || peso <= 0 || peso > 500)
                {
                    MessageBox.Show("Por favor, ingrese un peso válido (entre 1 y 500 kg).", "Peso inválido", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPeso.Focus();
                    return;
                }

                // Validar altura
                if (!decimal.TryParse(txtAltura.Text.Replace(",", "."), System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out decimal altura) || altura <= 0 || altura > 3)
                {
                    MessageBox.Show("Por favor, ingrese una altura válida en metros (ej: 1.75).", "Altura inválida", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAltura.Focus();
                    return;
                }

                // Calcular IMC: peso (kg) / altura (m)²
                decimal imc = peso / (altura * altura);
                
                // Mostrar resultado con 1 decimal usando formato invariante
                lblResultadoIMC.Text = imc.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
                
                // Determinar clasificación y color
                string clasificacion;
                Color color;
                
                if (imc < 18.5m)
                {
                    clasificacion = "Bajo peso - Se recomienda consultar con un nutricionista";
                    color = Color.FromArgb(52, 152, 219);
                }
                else if (imc < 25m)
                {
                    clasificacion = "Peso normal - ¡Mantén tus hábitos saludables!";
                    color = Color.FromArgb(46, 204, 113);
                }
                else if (imc < 30m)
                {
                    clasificacion = "Sobrepeso - Considera aumentar la actividad física";
                    color = Color.FromArgb(241, 196, 15);
                }
                else if (imc < 35m)
                {
                    clasificacion = "Obesidad grado I - Se recomienda consulta médica";
                    color = Color.FromArgb(230, 126, 34);
                }
                else if (imc < 40m)
                {
                    clasificacion = "Obesidad grado II - Consulta médica necesaria";
                    color = Color.FromArgb(231, 76, 60);
                }
                else
                {
                    clasificacion = "Obesidad grado III - Atención médica urgente";
                    color = Color.FromArgb(192, 57, 43);
                }

                lblClasificacion.Text = clasificacion;
                lblClasificacion.ForeColor = color;
                lblResultadoIMC.ForeColor = color;

                // Debug: Mostrar valores para verificación
                System.Diagnostics.Debug.WriteLine($"Peso: {peso}, Altura: {altura}, IMC: {imc:F2}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular el IMC: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}