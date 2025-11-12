using System;

namespace GimnasioApp.Models
{
    /// <summary>
    /// Inscripción de un socio a una clase.
    /// </summary>
    public class ClaseInscripcion
    {
        public int Id { get; set; }
        public int ClaseId { get; set; }
        public int SocioId { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}
