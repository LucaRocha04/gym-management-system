using GimnasioApp.Connection;
using System;

namespace ConsoleTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando inicialización de base de datos...");
            
            try
            {
                await DatabaseConnection.InitializeDatabaseAsync();
                Console.WriteLine("Inicialización completada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}