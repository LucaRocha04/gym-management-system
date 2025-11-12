using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using GimnasioApp.Models;
using GimnasioApp.Connection;

namespace GimnasioApp.Managers
{
    public class UsuarioManager
    {
        public async Task<List<Usuario>> GetAllAsync()
        {
            const string sql = @"SELECT id_usuario, nombre_usuario, mail, password, rol FROM usuarios ORDER BY nombre_usuario";
            var list = new List<Usuario>();
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            using SqliteDataReader reader = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                    NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                    Mail = reader.IsDBNull(reader.GetOrdinal("mail")) ? string.Empty : reader.GetString(reader.GetOrdinal("mail")),
                    Password = reader.GetString(reader.GetOrdinal("password")),
                    Rol = reader.GetString(reader.GetOrdinal("rol"))
                });
            }
            return list;
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            const string sql = @"SELECT id_usuario, nombre_usuario, mail, password, rol FROM usuarios WHERE id_usuario=@id LIMIT 1";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using SqliteDataReader reader = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                    NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                    Mail = reader.IsDBNull(reader.GetOrdinal("mail")) ? string.Empty : reader.GetString(reader.GetOrdinal("mail")),
                    Password = reader.GetString(reader.GetOrdinal("password")),
                    Rol = reader.GetString(reader.GetOrdinal("rol"))
                };
            }
            return null;
        }

        public async Task<bool> ExistsByNombreAsync(string nombreUsuario)
        {
            const string sql = @"SELECT COUNT(1) FROM usuarios WHERE nombre_usuario=@n";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", nombreUsuario);
            var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return result > 0;
        }

        public async Task<int> AddAsync(Usuario u)
        {
            const string sql = @"INSERT INTO usuarios (nombre_usuario, mail, password, rol)
                                 VALUES (@n, @m, @p, @r);
                                 SELECT last_insert_rowid();";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@n", u.NombreUsuario);
            if (string.IsNullOrWhiteSpace(u.Mail))
                cmd.Parameters.AddWithValue("@m", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@m", u.Mail);
            cmd.Parameters.AddWithValue("@p", u.Password);
            cmd.Parameters.AddWithValue("@r", u.Rol);
            var idObj = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(idObj);
        }

        public async Task<bool> UpdateRolAsync(int id, string nuevoRol)
        {
            const string sql = @"UPDATE usuarios SET rol=@r WHERE id_usuario=@id";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@r", nuevoRol);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> ResetPasswordAsync(int id, string newPassword)
        {
            const string sql = @"UPDATE usuarios SET password=@p WHERE id_usuario=@id";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", newPassword);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<(bool ok, Usuario? usuario)> ValidateLoginAsync(string userOrMail, string password)
        {
            const string sql = @"SELECT id_usuario, nombre_usuario, mail, password, rol
                                FROM usuarios
                                WHERE nombre_usuario = @u OR mail = @u
                                LIMIT 1;";
            try
            {
                using var conn = await DatabaseConnection.OpenConnectionAsync();
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", userOrMail);
                using SqliteDataReader reader = (SqliteDataReader)await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var pwd = reader.GetString(reader.GetOrdinal("password"));
                    if (pwd == password)
                    {
                        var usuario = new Usuario
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                            NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                            Mail = reader.IsDBNull(reader.GetOrdinal("mail")) ? string.Empty : reader.GetString(reader.GetOrdinal("mail")),
                            Password = pwd,
                            Rol = reader.GetString(reader.GetOrdinal("rol"))
                        };
                        return (true, usuario);
                    }
                }
                return (false, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Usuario?> GetUsuarioByMailAsync(string mail)
        {
            const string sql = @"SELECT id_usuario, nombre_usuario, mail, password, rol
                                FROM usuarios
                                WHERE mail = @m LIMIT 1;";
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@m", mail);
            using SqliteDataReader reader = (SqliteDataReader)await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                    NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                    Mail = reader.GetString(reader.GetOrdinal("mail")),
                    Password = reader.GetString(reader.GetOrdinal("password")),
                    Rol = reader.GetString(reader.GetOrdinal("rol"))
                };
            }
            return null;
        }

        /// <summary>
        /// Elimina un usuario de la base de datos. Valida que no se pueda eliminar al último Administrador.
        /// </summary>
        public async Task<bool> EliminarAsync(int id)
        {
            using var conn = await DatabaseConnection.OpenConnectionAsync();
            using var tx = await conn.BeginTransactionAsync();
            try
            {
                // Verificar que no sea el último admin
                const string sqlCountAdmin = @"SELECT COUNT(*) FROM usuarios WHERE rol='Administrador'";
                using var cmdCount = new SqliteCommand(sqlCountAdmin, conn, (SqliteTransaction)tx);
                var countAdmin = Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

                // Obtener rol del usuario a eliminar
                const string sqlGetRol = @"SELECT rol FROM usuarios WHERE id_usuario=@id";
                using var cmdRol = new SqliteCommand(sqlGetRol, conn, (SqliteTransaction)tx);
                cmdRol.Parameters.AddWithValue("@id", id);
                var rol = await cmdRol.ExecuteScalarAsync() as string;

                if (rol == "Administrador" && countAdmin <= 1)
                {
                    throw new InvalidOperationException("No se puede eliminar al único administrador del sistema");
                }

                // Eliminar usuario
                const string sqlDel = @"DELETE FROM usuarios WHERE id_usuario=@id";
                using var cmdDel = new SqliteCommand(sqlDel, conn, (SqliteTransaction)tx);
                cmdDel.Parameters.AddWithValue("@id", id);
                var rows = await cmdDel.ExecuteNonQueryAsync();

                await tx.CommitAsync();
                return rows > 0;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
