using System;

namespace GimnasioApp.Models
{
    /// <summary>
    /// Modelo extendido de Socio que incluye información de membresía
    /// </summary>
    public class SocioConMembresia : Socio
    {
        public DateTime? FechaVencimiento { get; set; }
        public string EstadoMembresia { get; set; } = string.Empty;
        public string NombrePlan { get; set; } = string.Empty;
        public int DiasRestantes { get; set; }
        
        /// <summary>
        /// Propiedad calculada para mostrar información de vencimiento de forma amigable
        /// </summary>
        public string VencimientoInfo 
        { 
            get 
            {
                if (!FechaVencimiento.HasValue)
                    return "Sin Plan";
                
                var diasRestantes = (FechaVencimiento.Value.Date - DateTime.Today).Days;
                
                if (diasRestantes < 0)
                    return $"Vencida ({Math.Abs(diasRestantes)} días)";
                else if (diasRestantes == 0)
                    return "Vence hoy";
                else if (diasRestantes <= 7)
                    return $"Vence en {diasRestantes} día(s)";
                else
                    return $"Vence el {FechaVencimiento.Value:dd/MM/yyyy}";
            }
        }
    }
}