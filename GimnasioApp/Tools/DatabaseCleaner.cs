using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using GimnasioApp.Connection;

namespace GimnasioApp.Tools
{
    public class DatabaseCleaner
    {
        public static async Task LimpiarDatosPresentacionAsync()
        {
            try
            {
                using var conn = await DatabaseConnection.OpenConnectionAsync();
                
                // Leer el script SQL
                var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "limpiar_datos_presentacion.sql");
                if (!File.Exists(scriptPath))
                {
                    // Crear el script si no existe
                    var sqlContent = @"
-- Limpiar todos los datos para presentación
DELETE FROM asistencias;
DELETE FROM clase_inscripciones;
DELETE FROM pagos;
DELETE FROM socios;
DELETE FROM clases;

-- Resetear contadores eliminando las secuencias (para empezar desde 1)
DELETE FROM sqlite_sequence WHERE name IN ('socios', 'pagos', 'asistencias', 'clases', 'clase_inscripciones');
";
                    Directory.CreateDirectory(Path.GetDirectoryName(scriptPath));
                    await File.WriteAllTextAsync(scriptPath, sqlContent);
                }

                var sql = await File.ReadAllTextAsync(scriptPath);
                
                // Ejecutar cada comando por separado
                var commands = sql.Split(';', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var command in commands)
                {
                    var cleanCommand = command.Trim();
                    if (!string.IsNullOrEmpty(cleanCommand) && !cleanCommand.StartsWith("--"))
                    {
                        using var cmd = new SqliteCommand(cleanCommand, conn);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                
                Console.WriteLine("✅ Base de datos limpiada exitosamente para presentación");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error limpiando base de datos: {ex.Message}");
                throw;
            }
        }
    }
}