using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using ciberseguridad.Models;

namespace ciberseguridad.Data
{
    public class UsuarioService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UsuarioService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> RegistrarUsuario(Usuario usuario)
        {
            string hash = CalcularHash(usuario.Password);

            // Si el rol no fue especificado, lo asignamos como "Usuario"
            string rol = string.IsNullOrWhiteSpace(usuario.Rol) ? "Usuario" : usuario.Rol;

            using var connection = new SqlConnection(_connectionString);
            var query = @"INSERT INTO Usuarios (Nombre, ApellidoPaterno, ApellidoMaterno, Telefono, Correo, PasswordHash, Rol)
                  VALUES (@Nombre, @ApellidoPaterno, @ApellidoMaterno, @Telefono, @Correo, @PasswordHash, @Rol)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            command.Parameters.AddWithValue("@ApellidoPaterno", usuario.ApellidoPaterno);
            command.Parameters.AddWithValue("@ApellidoMaterno", usuario.ApellidoMaterno);
            command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
            command.Parameters.AddWithValue("@Correo", usuario.Correo);
            command.Parameters.AddWithValue("@PasswordHash", hash);
            command.Parameters.AddWithValue("@Rol", rol);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }


        public async Task<Usuario?> ObtenerUsuarioPorCredenciales(string correo, string password)
        {
            string hash = CalcularHash(password);

            using var connection = new SqlConnection(_connectionString);
            var query = @"SELECT Id, Nombre, ApellidoPaterno, ApellidoMaterno, Correo FROM Usuarios
                          WHERE Correo = @Correo AND PasswordHash = @PasswordHash";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Correo", correo);
            command.Parameters.AddWithValue("@PasswordHash", hash);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Usuario
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    ApellidoPaterno = reader.GetString(2),
                    ApellidoMaterno = reader.GetString(3),
                    Correo = reader.GetString(4)
                };
            }

            return null;
        }

        private string CalcularHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

