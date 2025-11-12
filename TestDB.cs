using Microsoft.Data.Sqlite;

var dbPath = @"c:\Users\BANGHOI7\Desktop\proyecto final\GimnasioApp.Desktop\bin\Debug\net8.0-windows\gimnasio.db";
var connectionString = $"Data Source={dbPath}";

using var conn = new SqliteConnection(connectionString);
await conn.OpenAsync();

var cmd = new SqliteCommand("SELECT nombre_usuario, password, rol FROM usuarios", conn);
using var reader = await cmd.ExecuteReaderAsync();

Console.WriteLine("Usuarios en la base de datos:");
Console.WriteLine("Usuario\t\tPassword\tRol");
Console.WriteLine("-----------------------------------");

while (await reader.ReadAsync())
{
    var usuario = reader.GetString(0);
    var password = reader.GetString(1);
    var rol = reader.GetString(2);
    Console.WriteLine($"{usuario}\t\t{password}\t{rol}");
}