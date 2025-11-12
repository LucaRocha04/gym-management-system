using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GimnasioApp.Desktop.Theme
{
    public static class UITheme
    {
        // Paleta moderna para gimnasio
        private static readonly Color Bg = Color.FromArgb(248, 249, 252);
        private static readonly Color Text = Color.FromArgb(44, 62, 80);
        private static readonly Color Primary = Color.FromArgb(52, 152, 219);        // Azul moderno
        private static readonly Color PrimaryDark = Color.FromArgb(41, 128, 185);
        private static readonly Color Accent = Color.FromArgb(46, 204, 113);         // Verde deportivo
        private static readonly Color AccentDark = Color.FromArgb(39, 174, 96);
        private static readonly Color Danger = Color.FromArgb(231, 76, 60);          // Rojo para eliminar
        private static readonly Color Warning = Color.FromArgb(241, 196, 15);        // Amarillo para advertencias
        private static readonly Color Muted = Color.FromArgb(236, 240, 241);         // Gris claro
        private static readonly Color CardBg = Color.White;
        private static readonly Font BaseFont = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font TitleFont = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);

        public static void Apply(Form form)
        {
            form.BackColor = Bg;
            form.ForeColor = Text;
            form.Font = BaseFont;

            // Mejorar la apariencia del formulario
            if (form.FormBorderStyle == FormBorderStyle.Sizable)
            {
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
            }

            // Estilo por control
            foreach (Control ctrl in form.Controls)
            {
                StyleControl(ctrl);
            }

            // Agregar evento Paint para efectos visuales
            form.Paint += Form_Paint;
        }

        private static void Form_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is Form form)
            {
                // Agregar una sutil sombra interna en la parte superior
                using (var brush = new LinearGradientBrush(
                    new Rectangle(0, 0, form.Width, 20), 
                    Color.FromArgb(10, 0, 0, 0), 
                    Color.Transparent, 
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, form.Width, 20);
                }
            }
        }

        private static void StyleControl(Control ctrl)
        {
            switch (ctrl)
            {
                case Button b:
                    StyleButton(b);
                    break;
                case DataGridView dgv:
                    StyleGrid(dgv);
                    break;
                case Panel p:
                    StylePanel(p);
                    break;
                case Label l:
                    StyleLabel(l);
                    break;
                case TextBox tb:
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    tb.BackColor = Color.White;
                    tb.ForeColor = Text;
                    break;
                case ComboBox cb:
                    cb.FlatStyle = FlatStyle.System;
                    cb.BackColor = Color.White;
                    cb.ForeColor = Text;
                    break;
                case DateTimePicker dt:
                    dt.CalendarMonthBackground = Color.White;
                    dt.CalendarForeColor = Text;
                    break;
                case ListBox lb:
                    lb.BackColor = Color.White;
                    lb.ForeColor = Text;
                    break;
            }

            // Aplicar recursivamente
            foreach (Control child in ctrl.Controls)
                StyleControl(child);
        }

        private static void StyleButton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = Primary;
            b.ForeColor = Color.White;
            b.Padding = new Padding(12, 8, 12, 8);
            b.Cursor = Cursors.Hand;
            b.Font = new Font(BaseFont.FontFamily, BaseFont.Size, FontStyle.Regular);

            // Asignar colores según el nombre/función del botón
            var name = (b.Name ?? string.Empty).ToLowerInvariant();
            var text = (b.Text ?? string.Empty).ToLowerInvariant();
            
            Color originalColor = Primary;
            Color hoverColor = PrimaryDark;

            if (name.Contains("cancel") || text.Contains("cancel") || name.Contains("cerrar"))
            {
                originalColor = Color.FromArgb(149, 165, 166); // Gris neutral
                hoverColor = Color.FromArgb(127, 140, 141);
            }
            else if (name.Contains("eliminar") || name.Contains("delete") || text.Contains("eliminar") || text.Contains("quitar"))
            {
                originalColor = Danger;
                hoverColor = Color.FromArgb(192, 57, 43);
            }
            else if (name.Contains("guardar") || name.Contains("save") || text.Contains("guardar") || name.Contains("agregar") || name.Contains("inscrib"))
            {
                originalColor = Accent;
                hoverColor = AccentDark;
            }
            else if (name.Contains("refresc") || name.Contains("buscar") || text.Contains("buscar") || text.Contains("refresh"))
            {
                originalColor = Color.FromArgb(155, 89, 182);
                hoverColor = Color.FromArgb(142, 68, 173);
            }
            else if (name.Contains("editar") || name.Contains("edit") || text.Contains("editar"))
            {
                originalColor = Warning;
                hoverColor = Color.FromArgb(212, 172, 13);
            }

            b.BackColor = originalColor;

            // Eventos hover
            b.MouseEnter += (_, __) => { b.BackColor = hoverColor; };
            b.MouseLeave += (_, __) => { b.BackColor = originalColor; };
        }

        private static void StyleGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = CardBg;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.GridColor = Color.FromArgb(220, 221, 225);
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Encabezados con degradado suave
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(BaseFont.FontFamily, BaseFont.Size + 0.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 40;

            // Filas con mejor contraste
            dgv.DefaultCellStyle.BackColor = CardBg;
            dgv.DefaultCellStyle.ForeColor = Text;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(74, 144, 226);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = BaseFont;
            dgv.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
            
            // Alternancia de filas más sutil
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(74, 144, 226);
            
            dgv.RowTemplate.Height = 35;
        }

        private static void StylePanel(Panel p)
        {
            var name = (p.Name ?? string.Empty).ToLowerInvariant();
            if (name.Contains("paneltop"))
            {
                p.BackColor = Color.FromArgb(241, 243, 246);
                p.Padding = new Padding(15, 10, 15, 10);
            }
            else if (name.Contains("panelbottom"))
            {
                p.BackColor = Color.FromArgb(248, 249, 250);
                p.Padding = new Padding(15, 10, 15, 10);
            }
            else
            {
                p.BackColor = Color.Transparent;
            }
        }

        private static void StyleLabel(Label l)
        {
            l.ForeColor = Text;
            
            var name = (l.Name ?? string.Empty).ToLowerInvariant();
            var text = (l.Text ?? string.Empty);
            
            // Labels de título
            if (name.Contains("titulo") || name.Contains("title") || name.Contains("bienvenida") || 
                text.Contains("Bienvenido") || text.StartsWith("Gestión"))
            {
                l.Font = TitleFont;
                l.ForeColor = Color.FromArgb(44, 62, 80);
            }
            // Labels de estadísticas
            else if (name.Contains("estadistica") || name.Contains("total") || name.Contains("count"))
            {
                l.Font = new Font(BaseFont.FontFamily, BaseFont.Size + 1, FontStyle.Bold);
                l.ForeColor = Primary;
            }
        }
    }
}
