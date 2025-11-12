using GimnasioApp.Models;

namespace GimnasioApp.Repository
{
    public interface IMiembroRepository
    {
        Task<List<Miembro>> ObtenerTodosAsync();
        Task<Miembro?> ObtenerPorIdAsync(int id);
        Task<int> AgregarAsync(Miembro miembro);
        Task ActualizarAsync(Miembro miembro);
        Task EliminarAsync(int id);
    }
}