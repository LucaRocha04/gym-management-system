using System;

namespace GimnasioApp.Models
{
    /// <summary>
    /// Modelo para planes del gimnasio.
    /// </summary>
    public class Plan
    {
        public int Id { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public int DuracionDias { get; set; }
        public decimal Precio { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
