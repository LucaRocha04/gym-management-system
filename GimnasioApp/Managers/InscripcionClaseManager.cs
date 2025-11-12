using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using GimnasioApp.Connection;
using GimnasioApp.Models;

namespace GimnasioApp.Managers
{
    /// <summary>
    /// Maneja inscripciones de socios a clases.
    /// </summary>
    public class InscripcionClaseManager
    {
        public async Task<bool> InscribirAsync(int claseId, int socioId)
        {
            // Validaciones: no duplicado, clase activa, hay cupo, socio activo
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var tx = await conn.BeginTransactionAsync();

            try
            {
                // Validar que el socio esté activo
                const string sqlSocio = @"SELECT estado FROM socios WHERE id_socio=@s";
                using (var cmdSocio = new SqliteCommand(sqlSocio, conn, (SqliteTransaction)tx))
                {
                    cmdSocio.Parameters.AddWithValue("@s", socioId);
                    using var rdrSocio = (SqliteDataReader)await cmdSocio.ExecuteReaderAsync();
                    if (!await rdrSocio.ReadAsync()) throw new InvalidOperationException("Socio no encontrado");
                    var estadoSocio = rdrSocio.GetString(rdrSocio.GetOrdinal("estado"));
                    if (!string.Equals(estadoSocio, "Activo", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("No se puede inscribir un socio inactivo");
                }

                // Clase activa y obtener cupo
                int cupo = 0;
                const string sqlClase = @"SELECT cupo, estado FROM clases WHERE id_clase=@c";
                using (var cmdClase = new SqliteCommand(sqlClase, conn, (SqliteTransaction)tx))
                {
                    cmdClase.Parameters.AddWithValue("@c", claseId);
                    using var rdr = (SqliteDataReader)await cmdClase.ExecuteReaderAsync();
                    if (!await rdr.ReadAsync()) throw new InvalidOperationException("Clase no encontrada");
                    var estado = rdr.GetString(rdr.GetOrdinal("estado"));
                    if (!string.Equals(estado, "Activa", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("La clase no está activa");
                    cupo = rdr.GetInt32(rdr.GetOrdinal("cupo"));
                }

                // Duplicado
                const string sqlDup = @"SELECT COUNT(*) FROM clase_inscripciones WHERE id_clase=@c AND id_socio=@s";
                using (var cmdDup = new SqliteCommand(sqlDup, conn, (SqliteTransaction)tx))
                {
                    cmdDup.Parameters.AddWithValue("@c", claseId);
                    cmdDup.Parameters.AddWithValue("@s", socioId);
                    var cnt = Convert.ToInt32(await cmdDup.ExecuteScalarAsync());
                    if (cnt > 0) throw new InvalidOperationException("El socio ya está inscripto en esta clase");
                }

                // Cupo disponible
                int insc = 0;
                const string sqlCount = @"SELECT COUNT(*) FROM clase_inscripciones WHERE id_clase=@c";
                using (var cmdCnt = new SqliteCommand(sqlCount, conn, (SqliteTransaction)tx))
                {
                    cmdCnt.Parameters.AddWithValue("@c", claseId);
                    insc = Convert.ToInt32(await cmdCnt.ExecuteScalarAsync());
                }
                if (insc >= cupo) throw new InvalidOperationException("No hay cupos disponibles");

                // Insertar
                const string sqlIns = @"INSERT INTO clase_inscripciones (id_clase, id_socio) VALUES (@c, @s)";
                using (var cmdIns = new SqliteCommand(sqlIns, conn, (SqliteTransaction)tx))
                {
                    cmdIns.Parameters.AddWithValue("@c", claseId);
                    cmdIns.Parameters.AddWithValue("@s", socioId);
                    await cmdIns.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<(int SocioId, string NombreSocio)>> GetInscritosAsync(int claseId)
        {
            const string sql = @"SELECT s.id_socio, (s.nombre || ' ' || s.apellido) AS nombre
                                 FROM clase_inscripciones ci
                                 JOIN socios s ON s.id_socio = ci.id_socio
                                 WHERE ci.id_clase = @c
                                 ORDER BY nombre";
            var list = new List<(int, string)>();
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@c", claseId);
            using var rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add((rdr.GetInt32(0), rdr.GetString(1)));
            }
            return list;
        }

        public async Task<bool> DesinscribirAsync(int claseId, int socioId)
        {
            const string sql = @"DELETE FROM clase_inscripciones WHERE id_clase=@c AND id_socio=@s";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@c", claseId);
            cmd.Parameters.AddWithValue("@s", socioId);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}
