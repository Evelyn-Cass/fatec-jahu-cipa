using System.Diagnostics;
using CipaFatecJahu.Models;
using CipaFatecJahu.ViewModel;
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

        [Route("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [Route("Contact")]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {

                Console.WriteLine("contact sucefull");
               
                return RedirectToAction("Contact", new { success = true });
            }
            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
