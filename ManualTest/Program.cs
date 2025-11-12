using System;
using Microsoft.Data.Sqlite;

var dbPath = @"c:\Users\BANGHOI7\Desktop\proyecto final\ConsoleTest\bin\Debug\net8.0\gimnasio.db";
var connectionString = $"Data Source={dbPath}";

try
{
    using var conn = new SqliteConnection(connectionString);
    await conn.OpenAsync();

    // Intentar insertar usuarios manualmente
    Console.WriteLine("Insertando usuarios manualmente...");
    
    var insertAdmin = @"INSERT INTO usuarios (nombre_usuario, mail, password, rol) VALUES ('admin', 'admin@gimnasio.local', 'admin123', 'Administrador')";
    
    try
    {
        using var cmd = new SqliteCommand(insertAdmin, conn);
        var rows = await cmd.ExecuteNonQueryAsync();
        Console.WriteLine($"Admin insertado: {rows} fila(s) afectada(s)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error insertando admin: {ex.Message}");
    }
    
    // Verificar si se insertó
    var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM usuarios", conn);
    var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
    Console.WriteLine($"Total usuarios después de inserción: {count}");
    
    if (count > 0)
    {
        var listCmd = new SqliteCommand("SELECT nombre_usuario, password, rol FROM usuarios", conn);
        using var reader = await listCmd.ExecuteReaderAsync();
        
        Console.WriteLine("\nUsuarios encontrados:");
        while (await reader.ReadAsync())
        {
            Console.WriteLine($"- {reader.GetString(0)} / {reader.GetString(1)} / {reader.GetString(2)}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPresiona cualquier tecla para continuar...");
Console.ReadKey();