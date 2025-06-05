using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CipaFatecJahu.Views.Shared.Components.NavigationBar
{
    public class NavigationBarViewComponent : ViewComponent
    {
        private readonly ContextMongodb _context = new ContextMongodb();
        private readonly UserManager<ApplicationUser> _userManager;

        public NavigationBarViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mandates = await _context.Mandates.Find(_ => true)
                .SortByDescending(m => m.StartYear)
                .ToListAsync();

            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = (ClaimsPrincipal)User;
                var claims = await _userManager.GetClaimsAsync(await _userManager.GetUserAsync(user));
                ViewBag.FirstName = claims.FirstOrDefault(c => c.Type == "firstName")?.Value;
            }

            return View(mandates);
        }
    }
}
