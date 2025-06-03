namespace ciberseguridad.Models
{
    public class Usuario
    {
        public int Id { get; set; }  // ✅ Ya presente
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // ✅ NUEVO CAMPO
        public string Rol { get; set; } = "Usuario"; // U mayúscula como en tu base
    }
}
