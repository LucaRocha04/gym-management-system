using System;

namespace GimnasioApp.Models
{
    /// <summary>
    
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; 
        public string Rol { get; set; } = "Recepcionista"; // 'Administrador','Recepcionista','Profesor'
    }
}