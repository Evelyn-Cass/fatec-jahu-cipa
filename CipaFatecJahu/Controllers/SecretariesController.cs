using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CipaFatecJahu.Controllers
{
    public class SecretariesController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public SecretariesController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        // Substitua o uso de ToListAsync() por ToList() ao buscar usuários do UserManager
        public async Task<IActionResult> Index()
        {
          // Obtém todos os usuários
            var roleName = "Secretário";
            var appUsers = _userManager.Users.ToList()
                .Where(u => _userManager.IsInRoleAsync(u, roleName).Result)
                .ToList();

            var users = appUsers.Select(u => new User
            {
                Name = u.Name,
                Email = u.Email,
                Status = u.Status
            }).ToList();

            return View(users);
        }

        public IActionResult Create()
        {
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

        public async Task<IActionResult> ChangeStatus(string email, string status)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(status))
            {
                return NotFound();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            user.Status = user.Status == "Ativo" ? "Inativo" : "Ativo";

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("Index");
        }
        private bool DocumentExists(string email)
        {
            return _userManager.Users.Any(u => u.Email == email);
        }
    }//fim da classe
}//fim namespace
