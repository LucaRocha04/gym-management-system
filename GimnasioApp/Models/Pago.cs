using System;

namespace GimnasioApp.Models
{
    /// <summary>
    /// Modelo para pagos realizados por socios.
    /// </summary>
    public class Pago
    {
        public int IdPago { get; set; }
        public int SocioId { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; } = "Efectivo"; // 'Efectivo','Tarjeta','Transferencia'
        public string Observaciones { get; set; } = string.Empty;
    }
}