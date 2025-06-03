using System.Data.SqlClient;
using ciberseguridad.Models;

namespace ciberseguridad.Data
{
    public class EvaluacionService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public EvaluacionService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> GuardarResultadoEvaluacion(ResultadoEvaluacion resultado)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = @"INSERT INTO ResultadosEvaluacion (UsuarioId, Tema, Calificacion)
                     OUTPUT INSERTED.Id
                     VALUES (@UsuarioId, @Tema, @Calificacion)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UsuarioId", resultado.UsuarioId);
            command.Parameters.AddWithValue("@Tema", resultado.Tema);
            command.Parameters.AddWithValue("@Calificacion", resultado.Calificacion);

            await connection.OpenAsync();
            int resultadoId = (int)await command.ExecuteScalarAsync();
            return resultadoId;
        }

        public async Task GuardarRespuestas(List<RespuestaEvaluacion> respuestas)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            foreach (var respuesta in respuestas)
            {
                var query = @"INSERT INTO RespuestasEvaluacion (ResultadoId, PreguntaNumero, RespuestaSeleccionada)
                      VALUES (@ResultadoId, @PreguntaNumero, @RespuestaSeleccionada)";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ResultadoId", respuesta.ResultadoId);
                command.Parameters.AddWithValue("@PreguntaNumero", respuesta.PreguntaNumero);
                command.Parameters.AddWithValue("@RespuestaSeleccionada", respuesta.RespuestaSeleccionada);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<ResultadoEvaluacion?> ObtenerResultadoPorTema(int usuarioId, string tema)
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT TOP 1 * FROM ResultadosEvaluacion WHERE UsuarioId = @UsuarioId AND Tema = @Tema";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
            cmd.Parameters.AddWithValue("@Tema", tema);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ResultadoEvaluacion
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                    Tema = reader.GetString(reader.GetOrdinal("Tema")),
                    Calificacion = reader.GetInt32(reader.GetOrdinal("Calificacion")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha"))
                };
            }

            return null;
        }


        public async Task<List<RespuestaEvaluacion>> ObtenerRespuestasPorResultado(int resultadoId)
        {
            List<RespuestaEvaluacion> respuestas = new();

            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT * FROM RespuestasEvaluacion WHERE ResultadoId = @ResultadoId";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@ResultadoId", resultadoId);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                respuestas.Add(new RespuestaEvaluacion
                {
                    ResultadoId = reader.GetInt32(reader.GetOrdinal("ResultadoId")),
                    PreguntaNumero = reader.GetInt32(reader.GetOrdinal("PreguntaNumero")),
                    RespuestaSeleccionada = reader.GetString(reader.GetOrdinal("RespuestaSeleccionada"))
                });
            }

            return respuestas;
        }



    }
}
