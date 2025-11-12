using System;

namespace GimnasioApp.Models
{
    /// <summary>
    /// Modelo para socios (clientes del gimnasio).
    /// </summary>
    public class Socio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public DateTime? FechaIngreso { get; set; }
        public int? PlanId { get; set; }
        public string Estado { get; set; } = "Activo"; // 'Activo' o 'Inactivo'
    }
}