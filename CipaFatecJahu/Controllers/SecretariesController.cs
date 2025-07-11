﻿using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CipaFatecJahu.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class SecretariesController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public SecretariesController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        public IActionResult Index()
        {
            // Obtém todos os usuários  
            var roleName = "Secretário";
            var appUsers = _userManager.Users.ToList()
                .Where(u => _userManager.IsInRoleAsync(u, roleName).Result)
                .OrderBy(u => u.Name) // Ordena alfabeticamente pelo nome  
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
                    TempData["message"] = "Secretário Cadastrado com sucesso!";
                    return RedirectToAction("Index", "Secretaries");
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
        public IActionResult ChangePassword(string email)
        {
            if (email == null)
            {
                return NotFound();
            }
            var appuser = _userManager.Users.FirstOrDefault(u => u.Email == email);
            if (appuser == null)
            {
                return NotFound();
            }
            var user = new User
            {
                Name = appuser.Name,
                Email = appuser.Email
            };
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.Password != user.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "As senhas não conferem");
                    return View(user);
                }

                var appuser = await _userManager.FindByEmailAsync(user.Email);
                if (appuser == null)
                {
                    ModelState.AddModelError("", "Usuário não encontrado.");
                    return View(user);
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(appuser);
                var result = await _userManager.ResetPasswordAsync(appuser, resetToken, user.Password);

                if (result.Succeeded)
                {
                    TempData["message"] = "Senha alterada com sucesso!";
                    return RedirectToAction("Index", "Secretaries");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(user);
        }
    }
}
