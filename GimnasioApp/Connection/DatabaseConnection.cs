using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace GimnasioApp.Connection
{
    /// <summary>
    /// Maneja la conexión a la base de datos SQLite.
    /// Proporciona métodos async para abrir la conexión y testearla.
    /// </summary>
    public static class DatabaseConnection
    {
        // Archivo de base de datos SQLite en el directorio de la aplicación
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gimnasio.db");
        private static readonly string ConnectionString = $"Data Source={DbPath}";

        /// <summary>
        /// Abre y devuelve una SqliteConnection ya abierta.
        /// El caller es responsable de disposarla (using).
        /// </summary>
        public static async Task<SqliteConnection> OpenConnectionAsync()
        {
            var conn = new SqliteConnection(ConnectionString);
            try
            {
                await conn.OpenAsync();
                return conn; // conexión abierta
            }
            catch (Exception)
            {
                // Si falla abrir, asegurar dispose y relanzar para manejo externo.
                conn.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Prueba la conexión abriendo y cerrando la conexión.
        /// Devuelve true si fue exitosa, false si se catchuea excepción.
        /// Mensajes de excepción se propagan con detalle si se quiere.
        /// </summary>
        public static async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var conn = await OpenConnectionAsync();
                await conn.CloseAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a la DB: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Devuelve el string de conexión (para repos/managers).
        /// </summary>
        public static string GetConnectionString() => ConnectionString;

        /// <summary>
        /// Inicializa la base de datos SQLite si no existe.
        /// Ejecuta los scripts de creación de tablas y datos iniciales.
        /// </summary>
        public static async Task InitializeDatabaseAsync()
        {
            try
            {
                // Si no existe el archivo de base de datos, lo creamos
                if (!File.Exists(DbPath))
                {
                    Console.WriteLine("Creando nueva base de datos SQLite...");
                    
                    // Crear el directorio si no existe
                    var directory = Path.GetDirectoryName(DbPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                using var conn = await OpenConnectionAsync();
                
                // Verificar si la tabla usuarios existe
                using var checkTableCmd = new SqliteCommand(
                    "SELECT name FROM sqlite_master WHERE type='table' AND name='usuarios'", conn);
                var tableExists = await checkTableCmd.ExecuteScalarAsync();
                
                if (tableExists == null)
                {
                    // Ejecutar script de creación de tablas
                    var createTablesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "create_tables.sql");
                    if (File.Exists(createTablesPath))
                    {
                        var createTablesScript = await File.ReadAllTextAsync(createTablesPath);
                        var createCommands = createTablesScript.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        foreach (var commandText in createCommands)
                        {
                            var trimmedCommand = commandText.Trim();
                            if (!string.IsNullOrEmpty(trimmedCommand))
                            {
                                using var cmd = new SqliteCommand(trimmedCommand, conn);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        Console.WriteLine("Tablas creadas exitosamente.");
                    }
                }

                // Verificar si ya hay datos (usuarios)
                using var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM usuarios", conn);
                var userCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                
                if (userCount == 0)
                {
                    Console.WriteLine("No hay usuarios, ejecutando script de datos iniciales...");
                    // Ejecutar script de datos iniciales
                    var seedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "seed_data.sql");
                    Console.WriteLine($"Buscando script en: {seedDataPath}");
                    
                    if (File.Exists(seedDataPath))
                    {
                        Console.WriteLine("Archivo de seed encontrado, leyendo contenido...");
                        var seedScript = await File.ReadAllTextAsync(seedDataPath);
                        
                        // Ejecutar script completo en vez de dividirlo
                        try
                        {
                            Console.WriteLine("Ejecutando script completo...");
                            using var cmd = new SqliteCommand(seedScript, conn);
                            var rowsAffected = await cmd.ExecuteNonQueryAsync();
                            Console.WriteLine($"Script ejecutado, filas afectadas: {rowsAffected}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error ejecutando script completo: {ex.Message}");
                            
                            // Como fallback, intentar comando por comando excluyendo comentarios
                            Console.WriteLine("Intentando ejecutar línea por línea...");
                            var lines = seedScript.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            var currentCommand = "";
                            
                            foreach (var line in lines)
                            {
                                var trimmedLine = line.Trim();
                                
                                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("--"))
                                    continue;
                                
                                currentCommand += " " + trimmedLine;
                                
                                if (trimmedLine.EndsWith(";"))
                                {
                                    try
                                    {
                                        using var lineCmd = new SqliteCommand(currentCommand.Trim(), conn);
                                        var lineRows = await lineCmd.ExecuteNonQueryAsync();
                                        Console.WriteLine($"Ejecutado: {currentCommand.Substring(0, Math.Min(50, currentCommand.Length))}... ({lineRows} filas)");
                                        currentCommand = "";
                                    }
                                    catch (Exception lineEx)
                                    {
                                        Console.WriteLine($"Error en comando: {lineEx.Message}");
                                        Console.WriteLine($"Comando: {currentCommand}");
                                        currentCommand = "";
                                    }
                                }
                            }
                        }
                        Console.WriteLine("Datos iniciales insertados exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("Archivo de seed no encontrado!");
                    }
                }

                Console.WriteLine($"Base de datos SQLite lista en: {DbPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando base de datos: {ex.Message}");
                throw;
            }
        }
    }
}