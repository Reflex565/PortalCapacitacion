namespace ciberseguridad.Models
{
    public class ResultadoEvaluacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Tema { get; set; } = string.Empty;
        public int Calificacion { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
