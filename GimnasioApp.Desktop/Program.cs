using GimnasioApp.Desktop.Forms;
using GimnasioApp.Connection;

namespace GimnasioApp.Desktop
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            ApplicationConfiguration.Initialize();
            
            // Inicializar base de datos SQLite
            try
            {
                await DatabaseConnection.InitializeDatabaseAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar la base de datos: {ex.Message}", 
                    "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Application.Run(new SplashForm());
        }
    }
}
