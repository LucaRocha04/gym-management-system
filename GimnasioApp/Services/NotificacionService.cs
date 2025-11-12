using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GimnasioApp.Managers;
using GimnasioApp.Services;

namespace GimnasioApp.Services
{
    public class NotificacionService
    {
        private readonly EmailServiceBrevo _emailService;
        private readonly SocioManager _socioManager;

        public NotificacionService()
        {
            _emailService = new EmailServiceBrevo();
            _socioManager = new SocioManager();
        }

        /// <summary>
        /// Envía recordatorios a socios cuya membresía vence en los próximos días
        /// </summary>
        /// <param name="diasAntes">Días antes del vencimiento para enviar recordatorio</param>
        /// <returns>Número de recordatorios enviados</returns>
        public async Task<int> EnviarRecordatoriosVencimientoAsync(int diasAntes = 7)
        {
            try
            {
                if (!_emailService.EstaConfigurado())
                {
                    Console.WriteLine("Email no configurado. No se pueden enviar recordatorios.");
                    return 0;
                }

                var sociosConMembresia = await _socioManager.GetAllWithMembresiaAsync();
                var recordatoriosEnviados = 0;
                var fechaLimite = DateTime.Now.Date.AddDays(diasAntes);

                foreach (var socio in sociosConMembresia)
                {
                    // Solo procesar socios activos con email y membresía próxima a vencer
                    if (socio.Estado != "Activo" || string.IsNullOrWhiteSpace(socio.Mail))
                        continue;

                    if (socio.FechaVencimiento.HasValue)
                    {
                        var fechaVencimiento = socio.FechaVencimiento.Value.Date;
                        var diasParaVencer = (fechaVencimiento - DateTime.Now.Date).Days;

                        // Enviar recordatorio si vence en los próximos X días
                        if (diasParaVencer <= diasAntes && diasParaVencer >= 0)
                        {
                            try
                            {
                                var planManager = new PlanManager();
                                var plan = await planManager.GetByIdAsync(socio.PlanId ?? 0);
                                var nombrePlan = plan?.NombrePlan ?? "Plan Básico";

                                var success = await _emailService.EnviarRecordatorioVencimientoAsync(
                                    socio.Mail,
                                    $"{socio.Nombre} {socio.Apellido}",
                                    fechaVencimiento,
                                    nombrePlan
                                );

                                if (success)
                                {
                                    recordatoriosEnviados++;
                                    Console.WriteLine($"Recordatorio enviado a {socio.Nombre} {socio.Apellido} - Vence en {diasParaVencer} días");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error enviando recordatorio a {socio.Nombre} {socio.Apellido}: {ex.Message}");
                            }
                        }
                    }
                }

                Console.WriteLine($"Proceso completado. {recordatoriosEnviados} recordatorios enviados.");
                return recordatoriosEnviados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en proceso de recordatorios: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Procesa la cola de emails pendientes
        /// </summary>
        public async Task<int> ProcesarColaPendientesAsync()
        {
            return await _emailService.ProcesarColaAsync();
        }

        /// <summary>
        /// Envía recordatorios urgentes (1-3 días antes del vencimiento)
        /// </summary>
        public async Task<int> EnviarRecordatoriosUrgentesAsync()
        {
            return await EnviarRecordatoriosVencimientoAsync(3);
        }

        /// <summary>
        /// Envía recordatorios normales (7 días antes del vencimiento)
        /// </summary>
        public async Task<int> EnviarRecordatoriosNormalesAsync()
        {
            return await EnviarRecordatoriosVencimientoAsync(7);
        }

        /// <summary>
        /// Obtiene estadísticas de membresías próximas a vencer
        /// </summary>
        public async Task<Dictionary<string, int>> ObtenerEstadisticasVencimientosAsync()
        {
            var estadisticas = new Dictionary<string, int>
            {
                ["VencenHoy"] = 0,
                ["VencenEn1Dia"] = 0,
                ["VencenEn3Dias"] = 0,
                ["VencenEn7Dias"] = 0,
                ["VencenEn15Dias"] = 0,
                ["Vencidos"] = 0
            };

            try
            {
                var sociosConMembresia = await _socioManager.GetAllWithMembresiaAsync();
                var hoy = DateTime.Now.Date;

                foreach (var socio in sociosConMembresia)
                {
                    if (socio.FechaVencimiento.HasValue && socio.Estado == "Activo")
                    {
                        var fechaVencimiento = socio.FechaVencimiento.Value.Date;
                        var diasParaVencer = (fechaVencimiento - hoy).Days;

                        if (diasParaVencer < 0)
                            estadisticas["Vencidos"]++;
                        else if (diasParaVencer == 0)
                            estadisticas["VencenHoy"]++;
                        else if (diasParaVencer == 1)
                            estadisticas["VencenEn1Dia"]++;
                        else if (diasParaVencer <= 3)
                            estadisticas["VencenEn3Dias"]++;
                        else if (diasParaVencer <= 7)
                            estadisticas["VencenEn7Dias"]++;
                        else if (diasParaVencer <= 15)
                            estadisticas["VencenEn15Dias"]++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo estadísticas: {ex.Message}");
            }

            return estadisticas;
        }
    }
}