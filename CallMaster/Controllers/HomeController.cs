using System.Diagnostics;
using CallMaster.Models;
using Microsoft.AspNetCore.Mvc;

namespace CallMaster.Controllers
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
            //if (User.Identity.IsAuthenticated)  // Check if the user is already authenticated
            //{
            //    return RedirectToAction("Dashboard", "Home"); // Redirect to the Dashboard if authenticated
            //}

            return View(); // Otherwise, show the login page
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
