using Dapper;
using Microsoft.Data.Sqlite;
using GimnasioApp.Models;
using GimnasioApp.Connection;

namespace GimnasioApp.Repository
{
    public class MiembroRepository : IMiembroRepository
    {
        private readonly string _connectionString;

        public MiembroRepository()
        {
            _connectionString = DatabaseConnection.GetConnectionString();
        }

        public async Task<List<Miembro>> ObtenerTodosAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "SELECT * FROM miembros WHERE activo = true";
            var miembros = await connection.QueryAsync<Miembro>(sql);
            return miembros.ToList();
        }

        public async Task<Miembro?> ObtenerPorIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "SELECT * FROM miembros WHERE id = @Id AND activo = true";
            return await connection.QueryFirstOrDefaultAsync<Miembro>(sql, new { Id = id });
        }

        public async Task<int> AgregarAsync(Miembro miembro)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"INSERT INTO miembros (nombre, apellido, email, telefono, fecha_registro, activo) 
                       VALUES (@Nombre, @Apellido, @Email, @Telefono, @FechaRegistro, @Activo);
                       SELECT last_insert_rowid();";
            return await connection.ExecuteScalarAsync<int>(sql, miembro);
        }

        public async Task ActualizarAsync(Miembro miembro)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"UPDATE miembros 
                       SET nombre = @Nombre, apellido = @Apellido, 
                           email = @Email, telefono = @Telefono 
                       WHERE id = @Id";
            await connection.ExecuteAsync(sql, miembro);
        }

        public async Task EliminarAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "UPDATE miembros SET activo = false WHERE id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
