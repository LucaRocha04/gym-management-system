using GimnasioApp.Models;
using GimnasioApp.Repository;

namespace GimnasioApp.Services
{
    public class MiembroService
    {
        private readonly IMiembroRepository _repository;

        public MiembroService(IMiembroRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Miembro>> ObtenerTodosLosActivosAsync()
        {
            return await _repository.ObtenerTodosAsync();
        }

        public async Task<Miembro?> ObtenerPorIdAsync(int id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task<int> RegistrarMiembroAsync(Miembro miembro)
        {
            miembro.FechaRegistro = DateTime.Now;
            miembro.Activo = true;
            return await _repository.AgregarAsync(miembro);
        }

        public async Task ActualizarMiembroAsync(Miembro miembro)
        {
            await _repository.ActualizarAsync(miembro);
        }

        public async Task DarDeBajaMiembroAsync(int id)
        {
            await _repository.EliminarAsync(id);
        }
    }
}