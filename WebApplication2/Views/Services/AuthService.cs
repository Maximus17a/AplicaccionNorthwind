using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using NorthwindSalesAnalysis.Models.Repository;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NorthwindSalesAnalysis.Services
{
    public class AuthService
    {
        private readonly INorthwindRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(INorthwindRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Succeeded, string ErrorMessage)> ValidateUserAsync(string username, string password)
        {
            var user = await _repository.GetUsuarioByNombreAsync(username);

            if (user == null)
            {
                Console.WriteLine($"Usuario {username} no encontrado");
                return (false, "Usuario no encontrado");
            }

            // Verificar contraseña con comparación directa
            bool isPasswordValid = password == user.Password;

            if (!isPasswordValid)
            {
                Console.WriteLine($"Contraseña incorrecta para usuario {username}");
                return (false, "Contraseña incorrecta");
            }

            // Crear claims para la autenticación
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Surname, user.Apellido),
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return (true, string.Empty);
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}