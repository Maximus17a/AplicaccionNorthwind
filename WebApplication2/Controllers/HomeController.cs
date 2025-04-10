using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindSalesAnalysis.Models.Repository;


namespace NorthwindSalesAnalysis.Controllers
{
    public class HomeController : Controller
    {
        private readonly INorthwindRepository _repository;

        public HomeController(INorthwindRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            // Mostrar informaci�n general del dashboard
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            // Aqu� podr�amos mostrar un resumen de los informes disponibles
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}