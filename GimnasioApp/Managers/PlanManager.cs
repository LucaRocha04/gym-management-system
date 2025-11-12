using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using GimnasioApp.Models;
using GimnasioApp.Connection;

namespace GimnasioApp.Managers
{
    /// <summary>
    /// CRUD para planes.
    /// </summary>
    public class PlanManager
    {
        private Plan MapReader(SqliteDataReader rdr)
        {
            return new Plan
            {
                Id = rdr.GetInt32(rdr.GetOrdinal("id_plan")),
                NombrePlan = rdr.GetString(rdr.GetOrdinal("nombre_plan")),
                DuracionDias = rdr.GetInt32(rdr.GetOrdinal("duracion_dias")),
                Precio = rdr.GetDecimal(rdr.GetOrdinal("precio")),
                Descripcion = rdr.IsDBNull(rdr.GetOrdinal("descripcion")) 
                    ? "" : rdr.GetString(rdr.GetOrdinal("descripcion"))
            };
        }

        public async Task<List<Plan>> GetAllAsync()
        {
            var list = new List<Plan>();
            const string sql = "SELECT * FROM planes;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) list.Add(MapReader(rdr));
            return list;
        }

        public async Task<Plan?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM planes WHERE id_plan = @id LIMIT 1;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync())
            {
                return MapReader(rdr);
            }
            return null;
        }

        public async Task<int> AddAsync(Plan p)
        {
            if (p.Precio <= 0) throw new ArgumentException("Precio debe ser mayor a 0.");
            if (p.DuracionDias <= 0) throw new ArgumentException("Duración debe ser mayor a 0.");

            const string sql = @"INSERT INTO planes (nombre_plan, duracion_dias, precio, descripcion)
                                 VALUES (@n,@d,@pr,@desc);
                                 SELECT last_insert_rowid();";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", p.NombrePlan);
            cmd.Parameters.AddWithValue("@d", p.DuracionDias);
            cmd.Parameters.AddWithValue("@pr", p.Precio);
            cmd.Parameters.AddWithValue("@desc", p.Descripcion ?? string.Empty);
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(res);
        }

        public async Task UpdateAsync(Plan p)
        {
            if (p.Id <= 0) throw new ArgumentException("Id inválido.");
            if (p.Precio <= 0) throw new ArgumentException("Precio debe ser mayor a 0.");
            if (p.DuracionDias <= 0) throw new ArgumentException("Duración debe ser mayor a 0.");

            const string sql = @"UPDATE planes SET nombre_plan=@n, duracion_dias=@d, precio=@pr, descripcion=@desc
                                 WHERE id_plan=@id;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", p.NombrePlan);
            cmd.Parameters.AddWithValue("@d", p.DuracionDias);
            cmd.Parameters.AddWithValue("@pr", p.Precio);
            cmd.Parameters.AddWithValue("@desc", p.Descripcion ?? string.Empty);
            cmd.Parameters.AddWithValue("@id", p.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM planes WHERE id_plan=@id;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
