using System;
using System.IO;
using System.Text.Json;

namespace GimnasioApp.Services
{
    public class EmailConfig
    {
        // Configuración Brevo (Sendinblue)
        public string BrevoApiKey { get; set; } = "";
        public string EmailFrom { get; set; } = "contacto@gimnasiolr.com";
        public string EmailFromName { get; set; } = "Gimnasio LR";
        
        // Información del gimnasio
        public string NombreGimnasio { get; set; } = "Gimnasio LR";
        public string DireccionGimnasio { get; set; } = "Av. Libertador 1234, San Miguel de Tucumán";
        public string TelefonoGimnasio { get; set; } = "3813860020";
        public string HorariosGimnasio { get; set; } = "Lunes a Viernes: 6:00 a 22:00 hs - Sábados: 8:00 a 20:00 hs";

        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emailconfig.json");

        public static EmailConfig CargarConfiguracion()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    return JsonSerializer.Deserialize<EmailConfig>(json) ?? new EmailConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando configuración de email: {ex.Message}");
            }

            // Si no existe el archivo o hay error, crear uno nuevo con valores por defecto
            var defaultConfig = new EmailConfig();
            defaultConfig.GuardarConfiguracion();
            return defaultConfig;
        }

        public void GuardarConfiguracion()
        {
            try
            {
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando configuración de email: {ex.Message}");
            }
        }

        public bool EstaConfigurado()
        {
            return !string.IsNullOrWhiteSpace(BrevoApiKey) && 
                   !string.IsNullOrWhiteSpace(EmailFrom);
        }
    }
}