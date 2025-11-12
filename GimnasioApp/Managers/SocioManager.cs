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
    /// CRUD para socios.
    /// </summary>
    public class SocioManager
    {
        public async Task<List<Socio>> GetAllAsync()
        {
            var list = new List<Socio>();
            const string sql = "SELECT * FROM socios;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync()) 
            {
                list.Add(MapReader(rdr));
            }
            return list;
        }

        public async Task<Socio?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM socios WHERE id_socio = @id LIMIT 1;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            if (await rdr.ReadAsync()) return MapReader(rdr);
            return null;
        }

        public async Task<int> AddAsync(Socio s)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(s.Nombre) || string.IsNullOrWhiteSpace(s.Apellido)) throw new ArgumentException("Nombre y apellido son requeridos.");
            if (string.IsNullOrWhiteSpace(s.DNI)) throw new ArgumentException("DNI es requerido.");

            // Verificar DNI único
            const string checkSql = "SELECT COUNT(*) FROM socios WHERE dni = @dni;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using (var checkCmd = new SqliteCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("@dni", s.DNI);
                var cnt = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                if (cnt > 0) throw new InvalidOperationException("DNI ya registrado.");
            }

            const string insertSql = @"INSERT INTO socios
                (nombre, apellido, dni, telefono, mail, direccion, fecha_ingreso, id_plan, estado)
                VALUES (@nombre,@apellido,@dni,@telefono,@mail,@direccion,@fecha_ingreso,@id_plan,@estado);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(insertSql, conn);
            cmd.Parameters.AddWithValue("@nombre", s.Nombre);
            cmd.Parameters.AddWithValue("@apellido", s.Apellido);
            cmd.Parameters.AddWithValue("@dni", s.DNI);
            cmd.Parameters.AddWithValue("@telefono", s.Telefono ?? string.Empty);
            cmd.Parameters.AddWithValue("@mail", s.Mail ?? string.Empty);
            cmd.Parameters.AddWithValue("@direccion", s.Direccion ?? string.Empty);
            cmd.Parameters.AddWithValue("@fecha_ingreso", s.FechaIngreso ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@id_plan", s.PlanId.HasValue ? (object)s.PlanId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@estado", s.Estado ?? "Activo");

            var result = await cmd.ExecuteScalarAsync();
            var socioId = Convert.ToInt32(result);

            // Enviar email de bienvenida si tiene email y plan
            if (!string.IsNullOrWhiteSpace(s.Mail) && s.PlanId.HasValue)
            {
                try
                {
                    var planManager = new PlanManager();
                    var plan = await planManager.GetByIdAsync(s.PlanId.Value);
                    var nombrePlan = plan?.NombrePlan ?? "Plan Básico";

                    var emailService = new EmailServiceBrevo();
                    await emailService.EnviarBienvenidaAsync(s.Mail, $"{s.Nombre} {s.Apellido}", nombrePlan);
                }
                catch (Exception ex)
                {
                    // Log del error, pero no fallar la operación
                    Console.WriteLine($"Error enviando email de bienvenida: {ex.Message}");
                }
            }

            return socioId;
        }

        public async Task UpdateAsync(Socio s)
        {
            if (s.Id <= 0) throw new ArgumentException("Id inválido.");
            const string sql = @"UPDATE socios SET
                nombre=@nombre, apellido=@apellido, dni=@dni,
                telefono=@telefono, mail=@mail, direccion=@direccion,
                fecha_ingreso=@fecha_ingreso, id_plan=@id_plan, estado=@estado
                WHERE id_socio=@id;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nombre", s.Nombre);
            cmd.Parameters.AddWithValue("@apellido", s.Apellido);
            cmd.Parameters.AddWithValue("@dni", s.DNI);
            cmd.Parameters.AddWithValue("@telefono", s.Telefono ?? string.Empty);
            cmd.Parameters.AddWithValue("@mail", s.Mail ?? string.Empty);
            cmd.Parameters.AddWithValue("@direccion", s.Direccion ?? string.Empty);
            cmd.Parameters.AddWithValue("@fecha_ingreso", s.FechaIngreso ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@id_plan", s.PlanId.HasValue ? (object)s.PlanId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@estado", s.Estado ?? "Activo");
            cmd.Parameters.AddWithValue("@id", s.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Confirmación debe hacerse en UI; aquí se borra (o setea Inactivo).
            const string sql = "UPDATE socios SET estado='Inactivo' WHERE id_socio=@id;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarDefinitivamenteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Id inválido.");
            
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var transaction = conn.BeginTransaction();
            
            try
            {
                // Verificar que el socio esté dado de baja antes de eliminar
                const string checkSql = "SELECT estado FROM socios WHERE id_socio = @id;";
                using (var checkCmd = new SqliteCommand(checkSql, conn, transaction))
                {
                    checkCmd.Parameters.AddWithValue("@id", id);
                    var resultado = await checkCmd.ExecuteScalarAsync();
                    var estado = resultado?.ToString();
                    
                    if (estado != "Inactivo")
                    {
                        throw new InvalidOperationException("Solo se pueden eliminar socios que estén dados de baja (Inactivos).");
                    }
                }
                
                // Eliminar registros relacionados primero (usando transacción)
                const string deleteAsistencias = "DELETE FROM asistencias WHERE id_socio = @id;";
                using (var cmd = new SqliteCommand(deleteAsistencias, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
                
                const string deletePagos = "DELETE FROM pagos WHERE id_socio = @id;";
                using (var cmd = new SqliteCommand(deletePagos, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
                
                const string deleteInscripciones = "DELETE FROM clase_inscripciones WHERE id_socio = @id;";
                using (var cmd = new SqliteCommand(deleteInscripciones, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
                
                // Eliminar el socio
                const string deleteSocio = "DELETE FROM socios WHERE id_socio = @id;";
                using (var cmd = new SqliteCommand(deleteSocio, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
                
                // Verificar si quedan socios después de la eliminación
                const string countSocios = "SELECT COUNT(*) FROM socios;";
                using (var cmd = new SqliteCommand(countSocios, conn, transaction))
                {
                    var totalSocios = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    
                    // Si no quedan socios, resetear la secuencia de autoincremento
                    if (totalSocios == 0)
                    {
                        const string resetSequence = "DELETE FROM sqlite_sequence WHERE name = 'socios';";
                        using var resetCmd = new SqliteCommand(resetSequence, conn, transaction);
                        await resetCmd.ExecuteNonQueryAsync();
                    }
                }
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            const string sql = "SELECT COUNT(*) FROM socios;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        /// <summary>
        /// Calcula la fecha de vencimiento de la membresía basada en el último pago y el plan activo
        /// </summary>
        public async Task<DateTime?> GetFechaVencimientoAsync(int socioId)
        {
            try
            {
                // Obtener información del socio y su plan
                const string socioSql = @"
                    SELECT s.id_plan, p.duracion_dias 
                    FROM socios s 
                    LEFT JOIN planes p ON s.id_plan = p.id_plan 
                    WHERE s.id_socio = @socioId AND s.estado = 'Activo'";
                
                using var conn = await DatabaseConnection.OpenConnectionAsync();
                
                int? duracionDias = null;
                using (var socioCmd = new SqliteCommand(socioSql, conn))
                {
                    socioCmd.Parameters.AddWithValue("@socioId", socioId);
                    using var socioReader = await socioCmd.ExecuteReaderAsync();
                    if (await socioReader.ReadAsync())
                    {
                        var duracionOrdinal = socioReader.GetOrdinal("duracion_dias");
                        if (!socioReader.IsDBNull(duracionOrdinal))
                        {
                            duracionDias = socioReader.GetInt32(duracionOrdinal);
                        }
                    }
                }

                if (!duracionDias.HasValue)
                {
                    return null; // No tiene plan asignado
                }

                // Obtener el último pago
                const string pagoSql = @"
                    SELECT fecha_pago 
                    FROM pagos 
                    WHERE id_socio = @socioId 
                    ORDER BY fecha_pago DESC 
                    LIMIT 1";
                
                using var pagoCmd = new SqliteCommand(pagoSql, conn);
                pagoCmd.Parameters.AddWithValue("@socioId", socioId);
                using var pagoReader = await pagoCmd.ExecuteReaderAsync();
                
                if (await pagoReader.ReadAsync())
                {
                    var fechaOrdinal = pagoReader.GetOrdinal("fecha_pago");
                    var fechaUltimoPago = pagoReader.GetDateTime(fechaOrdinal);
                    return fechaUltimoPago.AddDays(duracionDias.Value);
                }

                return null; // No tiene pagos registrados
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene el estado de la membresía (Activa, Vencida, Por Vencer)
        /// </summary>
        public async Task<string> GetEstadoMembresiaAsync(int socioId)
        {
            var fechaVencimiento = await GetFechaVencimientoAsync(socioId);
            
            if (!fechaVencimiento.HasValue)
            {
                return "Sin Plan";
            }

            var hoy = DateTime.Today;
            var diasRestantes = (fechaVencimiento.Value.Date - hoy).Days;

            if (diasRestantes < 0)
            {
                return "Vencida";
            }
            else if (diasRestantes <= 7)
            {
                return "Por Vencer";
            }
            else
            {
                return "Activa";
            }
        }

        /// <summary>
        /// Obtiene todos los socios con información detallada de membresía
        /// </summary>
        public async Task<List<SocioConMembresia>> GetAllWithMembresiaAsync()
        {
            var list = new List<SocioConMembresia>();
            const string sql = @"
                SELECT s.*, p.nombre_plan, p.duracion_dias
                FROM socios s 
                LEFT JOIN planes p ON s.id_plan = p.id_plan
                ORDER BY s.apellido, s.nombre";
            
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            using SqliteDataReader rdr = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            
            while (await rdr.ReadAsync()) 
            {
                var socio = MapReaderWithMembresia(rdr);
                
                // Calcular fecha de vencimiento
                socio.FechaVencimiento = await GetFechaVencimientoAsync(socio.Id);
                socio.EstadoMembresia = await GetEstadoMembresiaAsync(socio.Id);
                
                if (socio.FechaVencimiento.HasValue)
                {
                    socio.DiasRestantes = (socio.FechaVencimiento.Value.Date - DateTime.Today).Days;
                }

                list.Add(socio);
            }
            return list;
        }

        private SocioConMembresia MapReaderWithMembresia(SqliteDataReader rdr)
        {
            return new SocioConMembresia
            {
                Id = rdr.GetInt32(rdr.GetOrdinal("id_socio")),
                Nombre = rdr.IsDBNull(rdr.GetOrdinal("nombre")) ? "" : rdr.GetString(rdr.GetOrdinal("nombre")),
                Apellido = rdr.IsDBNull(rdr.GetOrdinal("apellido")) ? "" : rdr.GetString(rdr.GetOrdinal("apellido")),
                DNI = rdr.IsDBNull(rdr.GetOrdinal("dni")) ? "" : rdr.GetString(rdr.GetOrdinal("dni")),
                Telefono = rdr.IsDBNull(rdr.GetOrdinal("telefono")) ? "" : rdr.GetString(rdr.GetOrdinal("telefono")),
                Mail = rdr.IsDBNull(rdr.GetOrdinal("mail")) ? "" : rdr.GetString(rdr.GetOrdinal("mail")),
                Direccion = rdr.IsDBNull(rdr.GetOrdinal("direccion")) ? "" : rdr.GetString(rdr.GetOrdinal("direccion")),
                FechaIngreso = rdr.IsDBNull(rdr.GetOrdinal("fecha_ingreso")) ? null : rdr.GetDateTime(rdr.GetOrdinal("fecha_ingreso")),
                PlanId = rdr.IsDBNull(rdr.GetOrdinal("id_plan")) ? null : (int?)rdr.GetInt32(rdr.GetOrdinal("id_plan")),
                Estado = rdr.IsDBNull(rdr.GetOrdinal("estado")) ? "Activo" : rdr.GetString(rdr.GetOrdinal("estado")),
                NombrePlan = rdr.IsDBNull(rdr.GetOrdinal("nombre_plan")) ? "Sin Plan" : rdr.GetString(rdr.GetOrdinal("nombre_plan"))
            };
        }

        public async Task<int> GetTotalSociosActivosAsync()
        {
            const string sql = "SELECT COUNT(*) FROM socios WHERE estado = 'Activo';";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        private Socio MapReader(SqliteDataReader rdr)
        {
            return new Socio
            {
                Id = rdr.GetInt32(rdr.GetOrdinal("id_socio")),
                Nombre = rdr.IsDBNull(rdr.GetOrdinal("nombre")) ? "" : rdr.GetString(rdr.GetOrdinal("nombre")),
                Apellido = rdr.IsDBNull(rdr.GetOrdinal("apellido")) ? "" : rdr.GetString(rdr.GetOrdinal("apellido")),
                DNI = rdr.IsDBNull(rdr.GetOrdinal("dni")) ? "" : rdr.GetString(rdr.GetOrdinal("dni")),
                Telefono = rdr.IsDBNull(rdr.GetOrdinal("telefono")) ? "" : rdr.GetString(rdr.GetOrdinal("telefono")),
                Mail = rdr.IsDBNull(rdr.GetOrdinal("mail")) ? "" : rdr.GetString(rdr.GetOrdinal("mail")),
                Direccion = rdr.IsDBNull(rdr.GetOrdinal("direccion")) ? "" : rdr.GetString(rdr.GetOrdinal("direccion")),
                FechaIngreso = rdr.IsDBNull(rdr.GetOrdinal("fecha_ingreso")) ? null : rdr.GetDateTime(rdr.GetOrdinal("fecha_ingreso")),
                PlanId = rdr.IsDBNull(rdr.GetOrdinal("id_plan")) ? null : (int?)rdr.GetInt32(rdr.GetOrdinal("id_plan")),
                Estado = rdr.IsDBNull(rdr.GetOrdinal("estado")) ? "Activo" : rdr.GetString(rdr.GetOrdinal("estado"))
            };
        }
    }
}
