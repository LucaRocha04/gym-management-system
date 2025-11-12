using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using GimnasioApp.Models;
using GimnasioApp.Connection;

namespace GimnasioApp.Managers
{
    /// <summary>
    /// Control de asistencias.
    /// </summary>
    public class AsistenciaManager
    {
        public async Task<int> AddAsistenciaAsync(Asistencia a)
        {
            // Validar socio activo
            const string chkSql = "SELECT estado FROM socios WHERE id_socio=@id LIMIT 1;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using (var chk = new SqliteCommand(chkSql, conn))
            {
                chk.Parameters.AddWithValue("@id", a.SocioId);
                var val = await chk.ExecuteScalarAsync();
                if (val == null) throw new InvalidOperationException("Socio no existe.");
                if (val.ToString() != "Activo") throw new InvalidOperationException("Socio no está activo.");
            }

            const string sql = @"INSERT INTO asistencias (id_socio, fecha, hora_entrada, hora_salida, observaciones)
                                 VALUES (@id,@fecha,@he,@hs,@obs);
                                 SELECT last_insert_rowid();";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", a.SocioId);
            cmd.Parameters.AddWithValue("@fecha", a.Fecha.Date);
            cmd.Parameters.AddWithValue("@he", a.HoraEntrada);
            cmd.Parameters.AddWithValue("@hs", a.HoraSalida.HasValue ? (object)a.HoraSalida.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@obs", a.Observaciones ?? string.Empty);
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(res);
        }

        public async Task<List<Asistencia>> GetAsistenciasBySocioAsync(int id)
        {
            var list = new List<Asistencia>();
            const string sql = "SELECT * FROM asistencias WHERE id_socio=@id ORDER BY fecha DESC, hora_entrada DESC;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) list.Add(MapReader(rdr));
            return list;
        }

        public async Task<List<Asistencia>> GetAsistenciasByDateAsync(DateTime date)
        {
            var list = new List<Asistencia>();
            const string sql = "SELECT * FROM asistencias WHERE fecha = @fecha ORDER BY hora_entrada;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@fecha", date.Date);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) list.Add(MapReader(rdr));
            return list;
        }

        public async Task UpdateAsistenciaAsync(Asistencia asistencia)
        {
            const string sql = @"UPDATE asistencias 
                        SET hora_salida = @salida 
                        WHERE id_asistencia = @id;";
            
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            
            cmd.Parameters.AddWithValue("@id", asistencia.IdAsistencia);
            cmd.Parameters.AddWithValue("@salida", asistencia.HoraSalida);
            
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Asistencia>> GetAsistenciasByRangeAsync(DateTime desde, DateTime hasta)
        {
            var list = new List<Asistencia>();
            const string sql = "SELECT * FROM asistencias WHERE fecha BETWEEN @desde AND @hasta ORDER BY fecha, hora_entrada;";
            
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            
            cmd.Parameters.AddWithValue("@desde", desde.Date);
            cmd.Parameters.AddWithValue("@hasta", hasta.Date);
            
            using var rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(MapReader(rdr));
            }
            return list;
        }

        private Asistencia MapReader(SqliteDataReader rdr)
        {
            return new Asistencia
            {
                IdAsistencia = rdr.GetInt32(rdr.GetOrdinal("id_asistencia")),
                SocioId = rdr.GetInt32(rdr.GetOrdinal("id_socio")),
                Fecha = rdr.GetDateTime(rdr.GetOrdinal("fecha")),
                HoraEntrada = rdr.GetTimeSpan(rdr.GetOrdinal("hora_entrada")),
                HoraSalida = rdr.IsDBNull(rdr.GetOrdinal("hora_salida")) 
                    ? null : (TimeSpan?)rdr.GetTimeSpan(rdr.GetOrdinal("hora_salida")),
                Observaciones = rdr.IsDBNull(rdr.GetOrdinal("observaciones")) 
                    ? "" : rdr.GetString(rdr.GetOrdinal("observaciones"))
            };
        }
    }
}
