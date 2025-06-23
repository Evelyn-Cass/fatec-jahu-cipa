using CipaFatecJahu.Models;
using CipaFatecJahu.Services;
using CipaFatecJahu.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CipaFatecJahu.Controllers
{
    public class ContactController : Controller
    {
        private readonly EmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContactController(EmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _emailService = emailService;
            _userManager = userManager;
        }

        [Route("Contact")]
        public IActionResult Contact(bool success)
        {
            ViewBag.Success = success;
            return View();
        }

        [HttpPost]
        [Route("Contact")]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var body = $@"
                              <!DOCTYPE html>
                                <html>
                                    <head>
                                        <meta charset=""utf-8"">
                                    </head>
                                    <body>
                                        <p><strong>Nome:</strong> {model.Name}</p>
                                        <p><strong>Email:</strong> {model.Email}</p>
                                        <pre><strong>Mensagem:</strong> {model.Text}</pre>
                                    </body>
                                </html>";

                await _emailService.SendEmailAsync("ADDEMAIL",model.Subject,body);
                return RedirectToAction("Contact", new { success = true });
            }
            return View(model);
        }


    }
}
