using System;

namespace GimnasioApp.Models
{
    /// <summary>
    /// Modelo para registro de asistencias de socios.
    /// </summary>
    public class Asistencia
    {
        public int IdAsistencia { get; set; }
        public int SocioId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }
}