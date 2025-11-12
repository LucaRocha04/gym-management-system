using GimnasioApp.Desktop.Theme;

namespace GimnasioApp.Desktop.Forms
{
    public static class ThemeHelper
    {
        public static void ApplyThemeToAllForms()
        {
            // Esta función se puede usar para aplicar el tema a formularios ya abiertos
            // pero principalmente aplicamos el tema en el constructor de cada form
        }
        
        public static void ApplyThemeToForm(Form form)
        {
            UITheme.Apply(form);
            
            // Aplicar tema después de que el formulario esté completamente cargado
            form.Load += (sender, e) => UITheme.Apply(form);
        }
    }
}