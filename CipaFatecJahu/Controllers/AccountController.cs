using System.ComponentModel.DataAnnotations;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CipaFatecJahu.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser>? _userManager;
        private SignInManager<ApplicationUser>? _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Required][EmailAddress] string email,
                                               [Required] string password,
                                               bool remember)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    if (user.Status == "Inativo")
                    {
                        ModelState.AddModelError(string.Empty, "Login não Autorizado");
                        return View();
                    }
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, password, remember, true);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Verifique suas credenciais");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Verifique suas credenciais");
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

}
