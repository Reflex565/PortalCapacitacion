using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using ciberseguridad.Models;

namespace ciberseguridad.Services
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;
        private readonly ProtectedSessionStorage _session;

        public CustomAuthProvider(AuthService authService, ProtectedSessionStorage session)
        {
            _authService = authService;
            _session = session;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var result = await _session.GetAsync<Usuario>("usuario_logeado");

                if (result.Success && result.Value != null)
                {
                    var usuario = result.Value;
                    var nombreCompleto = $"{usuario.Nombre} {usuario.ApellidoPaterno} {usuario.ApellidoMaterno}";

                    var identidad = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Name, nombreCompleto),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            }, "apiauth");

                    var usuarioActual = new ClaimsPrincipal(identidad);
                    return new AuthenticationState(usuarioActual);
                }
            }
            catch (InvalidOperationException)
            {
                // estamos en prerender: ignora silenciosamente
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al recuperar usuario en sesión: {ex.Message}");
                // Opcional: puedes guardar en log o usar JS interop para mostrar error
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }


        public void NotificarCambio()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
