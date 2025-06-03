namespace ciberseguridad.Models
{
    public class RespuestaEvaluacion
    {
        public int Id { get; set; }  // Agrega esto
        public int ResultadoId { get; set; }
        public int PreguntaNumero { get; set; }
        public string RespuestaSeleccionada { get; set; } = string.Empty;
    }
}
