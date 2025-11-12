using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using GimnasioApp.Models;
using GimnasioApp.Connection;
using GimnasioApp.Services;

namespace GimnasioApp.Managers
{
    /// <summary>
    /// Gestión de pagos.
    /// </summary>
    public class PagoManager
    {
        public async Task<int> AddPagoAsync(Pago p)
        {
            if (p.Monto <= 0) throw new ArgumentException("Monto debe ser mayor a 0.");
            // Validar socio existe
            const string checkSql = "SELECT COUNT(*) FROM socios WHERE id_socio=@id AND estado='Activo';";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using (var chk = new SqliteCommand(checkSql, conn))
            {
                chk.Parameters.AddWithValue("@id", p.SocioId);
                var cnt = Convert.ToInt32(await chk.ExecuteScalarAsync());
                if (cnt == 0) throw new InvalidOperationException("Socio no existe o no está activo.");
            }

            const string sql = @"INSERT INTO pagos (id_socio, fecha_pago, monto, metodo, observaciones)
                                 VALUES (@id,@fecha,@monto,@metodo,@obs);
                                 SELECT last_insert_rowid();";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", p.SocioId);
            cmd.Parameters.AddWithValue("@fecha", p.FechaPago);
            cmd.Parameters.AddWithValue("@monto", p.Monto);
            cmd.Parameters.AddWithValue("@metodo", p.Metodo ?? "Efectivo");
            cmd.Parameters.AddWithValue("@obs", p.Observaciones ?? string.Empty);
            var res = await cmd.ExecuteScalarAsync();
            var pagoId = Convert.ToInt32(res);

            // Enviar email de confirmación de pago
            try
            {
                var socioManager = new SocioManager();
                var socio = await socioManager.GetByIdAsync(p.SocioId);
                
                if (socio != null && !string.IsNullOrWhiteSpace(socio.Mail))
                {
                    var planManager = new PlanManager();
                    var plan = await planManager.GetByIdAsync(socio.PlanId ?? 0);
                    var nombrePlan = plan?.NombrePlan ?? "Plan Básico";

                    var emailService = new EmailServiceBrevo();
                    await emailService.EnviarConfirmacionPagoAsync(
                        socio.Mail, 
                        $"{socio.Nombre} {socio.Apellido}", 
                        p.Monto, 
                        p.Metodo ?? "Efectivo", 
                        nombrePlan);
                }
            }
            catch (Exception ex)
            {
                // Log del error, pero no fallar la operación
                Console.WriteLine($"Error enviando email de confirmación de pago: {ex.Message}");
            }

            return pagoId;
        }

        public async Task<List<Pago>> GetPagosBySocioAsync(int id)
        {
            var list = new List<Pago>();
            const string sql = "SELECT * FROM pagos WHERE id_socio=@id ORDER BY fecha_pago DESC;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) list.Add(MapReader(rdr));
            return list;
        }

        public async Task<List<Pago>> GetPagosByMonthAsync(int year, int month)
        {
            var list = new List<Pago>();
            const string sql = @"SELECT * FROM pagos WHERE strftime('%Y', fecha_pago)=@y AND strftime('%m', fecha_pago)=@m ORDER BY fecha_pago DESC;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@y", year.ToString());
            cmd.Parameters.AddWithValue("@m", month.ToString("00"));
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) list.Add(MapReader(rdr));
            return list;
        }

        public async Task<decimal> GetIngresosMensualesAsync(int year, int month)
        {
            const string sql = @"SELECT COALESCE(SUM(monto), 0) 
                        FROM pagos 
                        WHERE strftime('%Y', fecha_pago) = @year 
                        AND strftime('%m', fecha_pago) = @month;";
            
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            
            cmd.Parameters.AddWithValue("@year", year.ToString());
            cmd.Parameters.AddWithValue("@month", month.ToString("00"));
            
            return Convert.ToDecimal(await cmd.ExecuteScalarAsync());
        }

        private Pago MapReader(SqliteDataReader rdr)
        {
            return new Pago
            {
                IdPago = rdr.GetInt32(rdr.GetOrdinal("id_pago")),
                SocioId = rdr.GetInt32(rdr.GetOrdinal("id_socio")),
                FechaPago = rdr.GetDateTime(rdr.GetOrdinal("fecha_pago")),
                Monto = rdr.GetDecimal(rdr.GetOrdinal("monto")),
                Metodo = rdr.GetString(rdr.GetOrdinal("metodo")),
                Observaciones = rdr.IsDBNull(rdr.GetOrdinal("observaciones")) 
                    ? "" : rdr.GetString(rdr.GetOrdinal("observaciones"))
            };
        }
    }
}
