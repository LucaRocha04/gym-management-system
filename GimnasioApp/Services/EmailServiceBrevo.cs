using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace GimnasioApp.Services
{
    public class EmailServiceBrevo
    {
        private readonly EmailConfig _config;
        private readonly string _queuePath;
        private readonly HttpClient _httpClient;

        public EmailServiceBrevo()
        {
            _config = EmailConfig.CargarConfiguracion();
            _queuePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "email_queue");
            Directory.CreateDirectory(_queuePath);
            _httpClient = new HttpClient();
        }

        public bool EstaConfigurado()
        {
            return _config.EstaConfigurado();
        }

        public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string contenido, bool esHtml = true)
        {
            try
            {
                if (!_config.EstaConfigurado())
                {
                    Console.WriteLine("Email no configurado. Guardando en cola offline...");
                    await GuardarEnColaAsync(destinatario, asunto, contenido, esHtml);
                    return true; // Se guardó en cola
                }

                var emailData = new
                {
                    sender = new { name = _config.EmailFromName, email = _config.EmailFrom },
                    to = new[] { new { email = destinatario } },
                    subject = asunto,
                    htmlContent = esHtml ? contenido : $"<pre>{contenido}</pre>"
                };

                var json = JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("api-key", _config.BrevoApiKey);

                var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Email enviado exitosamente a {destinatario}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error enviando email: {response.StatusCode} - {errorContent}");
                    await GuardarEnColaAsync(destinatario, asunto, contenido, esHtml);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando email: {ex.Message}");
                await GuardarEnColaAsync(destinatario, asunto, contenido, esHtml);
                return false;
            }
        }

        public async Task<bool> EnviarBienvenidaAsync(string emailDestino, string nombreCompleto, string nombrePlan)
        {
            var asunto = $"Confirmación de registro - {_config.NombreGimnasio}";
            var cuerpo = GenerarEmailBienvenida(nombreCompleto, nombrePlan);
            return await EnviarEmailAsync(emailDestino, asunto, cuerpo);
        }

        public async Task<bool> EnviarConfirmacionPagoAsync(string emailDestino, string nombreCompleto, decimal monto, string metodoPago, string nombrePlan)
        {
            var asunto = $"Confirmación de Pago - {_config.NombreGimnasio}";
            var cuerpo = GenerarEmailConfirmacionPago(nombreCompleto, monto, metodoPago, nombrePlan);
            return await EnviarEmailAsync(emailDestino, asunto, cuerpo);
        }

        public async Task<bool> EnviarRecordatorioVencimientoAsync(string emailDestino, string nombreCompleto, DateTime fechaVencimiento, string nombrePlan)
        {
            var asunto = $"Recordatorio: Tu membresía vence pronto - {_config.NombreGimnasio}";
            var cuerpo = GenerarEmailRecordatorioVencimiento(nombreCompleto, fechaVencimiento, nombrePlan);
            return await EnviarEmailAsync(emailDestino, asunto, cuerpo);
        }

        private string GenerarEmailBienvenida(string nombreCompleto, string nombrePlan)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #ffffff; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 25px; border: 1px solid #e0e0e0; }}
        .header {{ color: #333; margin-bottom: 20px; }}
        .content {{ color: #333; line-height: 1.5; font-size: 14px; }}
        .info-box {{ background-color: #f8f9fa; padding: 12px; border-left: 3px solid #007bff; margin: 15px 0; }}
        .footer {{ margin-top: 25px; padding-top: 15px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Confirmación de registro - {_config.NombreGimnasio}</h2>
        </div>
        
        <div class='content'>
            <p>Estimado/a {nombreCompleto},</p>
            
            <p>Su registro en {_config.NombreGimnasio} ha sido procesado correctamente.</p>
            
            <div class='info-box'>
                <strong>Información de su membresía:</strong><br>
                Plan contratado: {nombrePlan}<br>
                Fecha de registro: {DateTime.Now:dd/MM/yyyy}
            </div>
            
            <p><strong>Servicios disponibles:</strong></p>
            <ul>
                <li>Acceso a instalaciones deportivas</li>
                <li>Participación en actividades grupales</li>
                <li>Uso de equipamiento disponible</li>
                <li>Acceso a vestuarios</li>
            </ul>
            
            <p><strong>Horarios de atención:</strong><br>
            {_config.HorariosGimnasio}</p>
            
            <p>Para consultas, puede contactarnos durante nuestros horarios de atención.</p>
            
            <p>Saludos cordiales,<br>
            Equipo de {_config.NombreGimnasio}</p>
        </div>
        
        <div class='footer'>
            <strong>{_config.NombreGimnasio}</strong><br>
            {_config.DireccionGimnasio}<br>
            Tel: {_config.TelefonoGimnasio}<br>
            Email: {_config.EmailFrom}
        </div>
    </div>
</body>
</html>";
        }

        private string GenerarEmailConfirmacionPago(string nombreCompleto, decimal monto, string metodoPago, string nombrePlan)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; color: #27ae60; border-bottom: 3px solid #27ae60; padding-bottom: 20px; margin-bottom: 30px; }}
        .content {{ color: #333; line-height: 1.6; }}
        .payment-details {{ background-color: #d5f4e6; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ background-color: #ecf0f1; padding: 20px; margin-top: 30px; border-radius: 5px; text-align: center; color: #7f8c8d; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ Pago Confirmado</h1>
        </div>
        
        <div class='content'>
            <h2>Hola {nombreCompleto},</h2>
            
            <p>Tu pago ha sido procesado exitosamente. ¡Gracias por mantenerte activo en {_config.NombreGimnasio}!</p>
            
            <div class='payment-details'>
                <h3>Detalles del Pago</h3>
                <p><strong>Plan:</strong> {nombrePlan}</p>
                <p><strong>Monto:</strong> ${monto:N2}</p>
                <p><strong>Método de Pago:</strong> {metodoPago}</p>
                <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
            </div>
            
            <p>Tu membresía está activa y puedes seguir disfrutando de todas nuestras instalaciones.</p>
            
            <p><strong>Horarios de atención:</strong><br>
            {_config.HorariosGimnasio}</p>
        </div>
        
        <div class='footer'>
            <h4>{_config.NombreGimnasio}</h4>
            <p>📍 {_config.DireccionGimnasio} | 📞 {_config.TelefonoGimnasio}</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerarEmailRecordatorioVencimiento(string nombreCompleto, DateTime fechaVencimiento, string nombrePlan)
        {
            var diasRestantes = (fechaVencimiento.Date - DateTime.Now.Date).Days;
            var urgencia = diasRestantes <= 3 ? "¡URGENTE!" : "Recordatorio";
            var color = diasRestantes <= 3 ? "#e74c3c" : "#f39c12";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; color: {color}; border-bottom: 3px solid {color}; padding-bottom: 20px; margin-bottom: 30px; }}
        .content {{ color: #333; line-height: 1.6; }}
        .alert {{ background-color: {color}; color: white; padding: 20px; border-radius: 5px; margin: 20px 0; text-align: center; }}
        .footer {{ background-color: #ecf0f1; padding: 20px; margin-top: 30px; border-radius: 5px; text-align: center; color: #7f8c8d; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>{urgencia} Tu membresía vence pronto</h1>
        </div>
        
        <div class='content'>
            <h2>Hola {nombreCompleto},</h2>
            
            <p>Te escribimos para recordarte que tu membresía <strong>{nombrePlan}</strong> está próxima a vencer.</p>
            
            <div class='alert'>
                <h3>⚠️ Fecha de Vencimiento: {fechaVencimiento:dd/MM/yyyy}</h3>
                <p>Quedan {diasRestantes} día{(diasRestantes != 1 ? "s" : "")} para que expire tu membresía</p>
            </div>
            
            <p><strong>¿Cómo renovar?</strong></p>
            <ul>
                <li>🏃‍♂️ Visítanos en el gimnasio</li>
                <li>📞 Llámanos al {_config.TelefonoGimnasio}</li>
                <li>✉️ Responde este email</li>
            </ul>
            
            <p>No queremos que pierdas tu rutina de entrenamiento. ¡Renueva ahora y sigue alcanzando tus objetivos!</p>
            
            <p><strong>Nuestros horarios:</strong><br>
            {_config.HorariosGimnasio}</p>
        </div>
        
        <div class='footer'>
            <h4>{_config.NombreGimnasio}</h4>
            <p>📍 {_config.DireccionGimnasio} | 📞 {_config.TelefonoGimnasio}</p>
        </div>
    </div>
</body>
</html>";
        }

        private async Task GuardarEnColaAsync(string destinatario, string asunto, string contenido, bool esHtml)
        {
            try
            {
                var emailCola = new
                {
                    Destinatario = destinatario,
                    Asunto = asunto,
                    Contenido = contenido,
                    EsHtml = esHtml,
                    FechaCreacion = DateTime.Now,
                    Intentos = 0
                };

                var fileName = $"email_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.json";
                var filePath = Path.Combine(_queuePath, fileName);
                
                var json = JsonSerializer.Serialize(emailCola, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);
                
                Console.WriteLine($"Email guardado en cola: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando email en cola: {ex.Message}");
            }
        }

        public async Task<int> ProcesarColaAsync()
        {
            var processed = 0;
            try
            {
                if (!_config.EstaConfigurado())
                {
                    Console.WriteLine("Email no configurado. No se puede procesar la cola.");
                    return 0;
                }

                var files = Directory.GetFiles(_queuePath, "*.json");
                
                foreach (var file in files)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var emailData = JsonSerializer.Deserialize<dynamic>(json);
                        
                        // Intentar enviar el email
                        var success = await EnviarEmailAsync(
                            emailData.GetProperty("Destinatario").GetString(),
                            emailData.GetProperty("Asunto").GetString(),
                            emailData.GetProperty("Contenido").GetString(),
                            emailData.GetProperty("EsHtml").GetBoolean()
                        );

                        if (success)
                        {
                            File.Delete(file);
                            processed++;
                            Console.WriteLine($"Email de cola procesado exitosamente: {Path.GetFileName(file)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error procesando email de cola {Path.GetFileName(file)}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando cola de emails: {ex.Message}");
            }

            return processed;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}