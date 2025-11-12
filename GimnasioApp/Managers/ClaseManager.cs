using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using GimnasioApp.Models;
using GimnasioApp.Connection;

namespace GimnasioApp.Managers
{
    /// <summary>
    /// Manejo de clases (sesiones) dictadas por profesores.
    /// </summary>
    public class ClaseManager
    {
        public async Task<int> CrearAsync(Clase c)
        {
            if (string.IsNullOrWhiteSpace(c.Nombre)) throw new ArgumentException("El nombre es requerido");
            if (c.Cupo <= 0) throw new ArgumentException("El cupo debe ser mayor a 0");
            if (c.HoraFin <= c.HoraInicio) throw new ArgumentException("La hora fin debe ser posterior a la hora inicio");

            const string sql = @"INSERT INTO clases
                (nombre, descripcion, fecha, hora_inicio, hora_fin, cupo, id_profesor, estado)
                VALUES (@n, @d, @f, @hi, @hf, @cupo, @prof, @e);
                SELECT last_insert_rowid();";

            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", c.Nombre.Trim());
            cmd.Parameters.AddWithValue("@d", string.IsNullOrWhiteSpace(c.Descripcion) ? (object)DBNull.Value : c.Descripcion.Trim());
            cmd.Parameters.AddWithValue("@f", c.Fecha.Date);
            cmd.Parameters.AddWithValue("@hi", c.HoraInicio);
            cmd.Parameters.AddWithValue("@hf", c.HoraFin);
            cmd.Parameters.AddWithValue("@cupo", c.Cupo);
            cmd.Parameters.AddWithValue("@prof", c.ProfesorId);
            cmd.Parameters.AddWithValue("@e", c.Estado);
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        public async Task<List<Clase>> GetPorProfesorAsync(int profesorId)
        {
            const string sql = @"SELECT * FROM clases WHERE id_profesor=@p ORDER BY fecha DESC, hora_inicio DESC";
            var list = new List<Clase>();
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", profesorId);
            using var rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) list.Add(Map(rdr));
            return list;
        }

        public async Task<List<Clase>> GetProximasAsync(DateTime? desde = null)
        {
            var d = (desde ?? DateTime.Today).Date;
            const string sql = @"SELECT * FROM clases WHERE fecha >= @d AND UPPER(estado)='ACTIVA' ORDER BY fecha, hora_inicio";
            var list = new List<Clase>();
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@d", d);
            using var rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) list.Add(Map(rdr));
            return list;
        }

        public async Task<Clase?> GetByIdAsync(int id)
        {
            const string sql = @"SELECT * FROM clases WHERE id_clase=@id LIMIT 1";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync()) return Map(rdr);
            return null;
        }

        public async Task<bool> CancelarAsync(int id)
        {
            const string sql = @"UPDATE clases SET estado='Cancelada' WHERE id_clase=@id";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<int> GetInscritosCountAsync(int claseId)
        {
            const string sql = @"SELECT COUNT(*) FROM clase_inscripciones WHERE id_clase=@c";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@c", claseId);
            var obj = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// Elimina permanentemente una clase y sus inscripciones asociadas (FK ON DELETE CASCADE).
        /// Sólo permite eliminar si la clase pertenece al profesor indicado.
        /// </summary>
        public async Task<bool> EliminarAsync(int claseId, int profesorId)
        {
            const string sql = @"DELETE FROM clases WHERE id_clase=@id AND id_profesor=@p";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", claseId);
            cmd.Parameters.AddWithValue("@p", profesorId);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        private Clase Map(SqliteDataReader rdr)
        {
            return new Clase
            {
                Id = rdr.GetInt32(rdr.GetOrdinal("id_clase")),
                Nombre = rdr.GetString(rdr.GetOrdinal("nombre")),
                Descripcion = rdr.IsDBNull(rdr.GetOrdinal("descripcion")) ? string.Empty : rdr.GetString(rdr.GetOrdinal("descripcion")),
                Fecha = rdr.GetDateTime(rdr.GetOrdinal("fecha")),
                HoraInicio = rdr.GetTimeSpan(rdr.GetOrdinal("hora_inicio")),
                HoraFin = rdr.GetTimeSpan(rdr.GetOrdinal("hora_fin")),
                Cupo = rdr.GetInt32(rdr.GetOrdinal("cupo")),
                ProfesorId = rdr.GetInt32(rdr.GetOrdinal("id_profesor")),
                Estado = rdr.GetString(rdr.GetOrdinal("estado"))
            };
        }
    }
}
