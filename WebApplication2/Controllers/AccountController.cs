using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindSalesAnalysis.Models.Entities;
using NorthwindSalesAnalysis.Models.Repository;
using NorthwindSalesAnalysis.Models.ViewModels;
using NorthwindSalesAnalysis.Services;
using System;
using System.Threading.Tasks;

namespace NorthwindSalesAnalysis.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly INorthwindRepository _repository;

        public AccountController(AuthService authService, INorthwindRepository repository)
        {
            _authService = authService;
            _repository = repository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _authService.ValidateUserAsync(model.Username, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el usuario ya existe
                    var existingUser = await _repository.GetUsuarioByNombreAsync(model.Username);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError(string.Empty, "El nombre de usuario ya está en uso.");
                        return View(model);
                    }

                    // Crear nuevo usuario
                    var usuario = new Usuario
                    {
                        Nombre = model.Username,
                        Apellido = model.LastName,
                        Password = model.Password // Sin hashear
                    };

                    var result = await _repository.CreateUsuarioAsync(usuario);

                    if (result)
                    {
                        // Iniciar sesión automáticamente después del registro
                        await _authService.ValidateUserAsync(model.Username, model.Password);
                        return RedirectToLocal(returnUrl);
                    }

                    ModelState.AddModelError(string.Empty, "Error al crear el usuario.");
                }
                catch (Exception ex)
                {
                    // Capturar y mostrar el error específico
                    ModelState.AddModelError(string.Empty, $"Error detallado: {ex.Message}");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}