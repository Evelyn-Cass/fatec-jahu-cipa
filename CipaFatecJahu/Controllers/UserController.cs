using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CipaFatecJahu.Controllers
{
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public IActionResult Create(string role)
        {
            ViewBag.Role = role;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.Password != user.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "As senhas não conferem");
                    return View();
                }
                ApplicationUser appuser = new ApplicationUser();
                appuser.Name = user.Name;
                appuser.UserName = user.Email;
                appuser.Email = user.Email;
                appuser.Status = "Ativo";
                IdentityResult result = await _userManager.CreateAsync(appuser, user.Password);
                if (result.Succeeded)
                {
                    //add role
                    await _userManager.AddToRoleAsync(appuser, "Secretário");
                    var firstName = user.Name?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault() ?? string.Empty;
                    await _userManager.AddClaimAsync(appuser, new Claim("firstName", firstName));
                    ViewBag.Message = "Secretário Cadastrado com sucesso";
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }
        //[Authorize(Roles = "Administrador")]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CreateRole(UserRole userRole)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = userRole.RoleName });
                if (result.Succeeded)
                {
                    ViewBag.Message = "Perfil Cadastrado com sucesso";
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }
    }//fim da classe
}//fim namespace
