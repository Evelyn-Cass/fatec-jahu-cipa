using System.Diagnostics;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Mvc;

namespace CipaFatecJahu.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<AboutController> _logger;

        public ContactController(ILogger<AboutController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
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
