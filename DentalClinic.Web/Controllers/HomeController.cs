using DentalClinic.Application.Interfaces;
using DentalClinic.Domain.Interfaces;
using DentalClinic.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DentalClinic.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            IUserManagementService userManagementService,
            IUserRepository userRepository)
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
