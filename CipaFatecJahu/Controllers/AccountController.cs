using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Authentication;
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
                if (_userManager == null || _signInManager == null)
                {
                    ModelState.AddModelError(string.Empty, "Erro interno de autenticação.");
                    return View();
                }

                ApplicationUser? user = await _userManager.FindByEmailAsync(email);

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
                        var firstName = user.Name?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, firstName)
                        };
                        var userRoles = await _userManager.GetRolesAsync(user);
                        foreach (var role in userRoles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync("Identity.Application", claimsPrincipal);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Usuário bloqueado");
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
