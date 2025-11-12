using System;

namespace GimnasioApp.Models
{
    /// <summary>
    /// Clase o sesión dictada por un profesor (ej. Zumba).
    /// </summary>
    public class Clase
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public int Cupo { get; set; }
        public int ProfesorId { get; set; }
        public string Estado { get; set; } = "Activa"; // 'Activa' | 'Cancelada'
    }
}
