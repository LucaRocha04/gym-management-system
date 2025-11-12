using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using GimnasioApp.Models;
using GimnasioApp.Connection;

namespace GimnasioApp.Managers
{
    /// <summary>
    /// Reportes: total socios, ingresos mensuales, socios con cuota vencida.
    /// </summary>
    public class ReporteManager
    {
        public async Task<int> GetTotalSociosAsync()
        {
            const string sql = "SELECT COUNT(*) FROM socios WHERE estado='Activo';";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(res);
        }

        public async Task<decimal> GetIngresosMensualesAsync(int year, int month)
        {
            const string sql = "SELECT COALESCE(SUM(monto),0) FROM pagos WHERE strftime('%Y', fecha_pago)=@y AND strftime('%m', fecha_pago)=@m;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@y", year.ToString());
            cmd.Parameters.AddWithValue("@m", month.ToString("00"));
            var res = await cmd.ExecuteScalarAsync();
            return Convert.ToDecimal(res);
        }

        /// <summary>
        /// Heurística: socios sin pagos en últimos 30 días.
        /// </summary>
        public async Task<List<Socio>> GetSociosConCuotaVencidaAsync()
        {
            var list = new List<Socio>();
            const string sql = @"
                SELECT s.* FROM socios s
                LEFT JOIN (
                    SELECT id_socio, MAX(fecha_pago) AS last_pago FROM pagos GROUP BY id_socio
                ) p ON s.id_socio = p.id_socio
                WHERE s.estado = 'Activo' AND (p.last_pago IS NULL OR p.last_pago < date('now', '-30 days'));
            ";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(MapReader(rdr));
            }
            return list;
        }

        public Socio MapReader(SqliteDataReader rdr)
        {
            return new Socio
            {
                Id = rdr.GetInt32(rdr.GetOrdinal("id_socio")),
                Nombre = rdr.GetString(rdr.GetOrdinal("nombre")),
                Apellido = rdr.GetString(rdr.GetOrdinal("apellido")),
                DNI = rdr.GetString(rdr.GetOrdinal("dni")),
                Mail = rdr.IsDBNull(rdr.GetOrdinal("mail")) 
                    ? "" : rdr.GetString(rdr.GetOrdinal("mail")),
                Telefono = rdr.IsDBNull(rdr.GetOrdinal("telefono")) ? "" : rdr.GetString(rdr.GetOrdinal("telefono")),
                Direccion = rdr.IsDBNull(rdr.GetOrdinal("direccion")) ? "" : rdr.GetString(rdr.GetOrdinal("direccion")),
                FechaIngreso = rdr.IsDBNull(rdr.GetOrdinal("fecha_ingreso")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("fecha_ingreso")),
                PlanId = rdr.IsDBNull(rdr.GetOrdinal("id_plan")) ? (int?)null : rdr.GetInt32(rdr.GetOrdinal("id_plan")),
                Estado = rdr.IsDBNull(rdr.GetOrdinal("estado")) ? "Activo" : rdr.GetString(rdr.GetOrdinal("estado"))
            };
        }
    }
}
