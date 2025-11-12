using System;
using Microsoft.Data.Sqlite;

var dbPath = @"c:\Users\BANGHOI7\Desktop\proyecto final\ConsoleTest\bin\Debug\net8.0\gimnasio.db";
var connectionString = $"Data Source={dbPath}";

try
{
    using var conn = new SqliteConnection(connectionString);
    await conn.OpenAsync();

    // Verificar todas las tablas
    var tablesCmd = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table'", conn);
    using var tablesReader = await tablesCmd.ExecuteReaderAsync();
    
    Console.WriteLine("Tablas en la base de datos:");
    while (await tablesReader.ReadAsync())
    {
        Console.WriteLine($"- {tablesReader.GetString(0)}");
    }
    tablesReader.Close();
    
    Console.WriteLine();

    // Verificar usuarios
    var usuariosCmd = new SqliteCommand("SELECT COUNT(*) FROM usuarios", conn);
    var userCount = Convert.ToInt32(await usuariosCmd.ExecuteScalarAsync());
    Console.WriteLine($"Cantidad de usuarios: {userCount}");
    
    if (userCount > 0)
    {
        var cmd = new SqliteCommand("SELECT nombre_usuario, password, rol FROM usuarios", conn);
        using var reader = await cmd.ExecuteReaderAsync();

        Console.WriteLine("\nUsuarios en la base de datos:");
        Console.WriteLine("Usuario\t\tPassword\t\tRol");
        Console.WriteLine("-----------------------------------");

        while (await reader.ReadAsync())
        {
            var usuario = reader.GetString(0);
            var password = reader.GetString(1);
            var rol = reader.GetString(2);
            Console.WriteLine($"{usuario}\t\t{password}\t\t{rol}");
        }
    }
    
    // Verificar planes
    var planesCmd = new SqliteCommand("SELECT COUNT(*) FROM planes", conn);
    var planCount = Convert.ToInt32(await planesCmd.ExecuteScalarAsync());
    Console.WriteLine($"\nCantidad de planes: {planCount}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPresiona cualquier tecla para continuar...");
Console.ReadKey();