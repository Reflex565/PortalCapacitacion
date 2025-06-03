
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using ciberseguridad.Models;

namespace ciberseguridad.Services
{
    public class AuthService
    {
        private ClaimsPrincipal _usuarioActual = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly ProtectedSessionStorage _session;

        public ClaimsPrincipal UsuarioActual => _usuarioActual;

        public AuthService(ProtectedSessionStorage session)
        {
            _session = session;
        }

        public async Task Login(Usuario usuario)
        {
            var nombreCompleto = $"{usuario.Nombre} {usuario.ApellidoPaterno} {usuario.ApellidoMaterno}";

            var identidad = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, nombreCompleto),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            }, "apiauth");

            _usuarioActual = new ClaimsPrincipal(identidad);
            await _session.SetAsync("usuario_logeado", usuario);
        }

        public async Task Logout()
        {
            await _session.DeleteAsync("usuario_logeado");
            _usuarioActual = new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}
