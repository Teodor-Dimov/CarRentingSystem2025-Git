using CarRentingSystem2025.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CarRentingSystem2025.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = Response.StatusCode,
                OriginalPath = HttpContext.Request.Path
            };
            
            // Get error details from the exception if available
            var exceptionHandlerPathFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature != null)
            {
                errorViewModel.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
                errorViewModel.ErrorDetails = exceptionHandlerPathFeature.Error.ToString();
            }
            
            return View(errorViewModel);
        }

        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View();
        }

        public IActionResult Error500()
        {
            Response.StatusCode = 500;
            return View();
        }

        public IActionResult Error403()
        {
            Response.StatusCode = 403;
            return View();
        }

        public IActionResult Error401()
        {
            Response.StatusCode = 401;
            return View();
        }

        public IActionResult Maintenance()
        {
            Response.StatusCode = 503;
            return View();
        }
    }
}
