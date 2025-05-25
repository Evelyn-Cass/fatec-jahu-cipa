using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CipaFatecJahu.Views.Shared.Components.NavigationBar
{
    public class NavigationBarViewComponent : ViewComponent
    {
        ContextMongodb _context = new ContextMongodb();
        private UserManager<ApplicationUser> _userManager;
        public NavigationBarViewComponent(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mandates = await _context.Mandates.Find(u => true).ToListAsync();
            if (User.Identity.IsAuthenticated && User != null)
            {
                var user = (ClaimsPrincipal)User;
                var claims = await _userManager.GetClaimsAsync(await _userManager.GetUserAsync(user));
                ViewBag.FirstName = claims.FirstOrDefault(c => c.Type == "firstName")?.Value;
            }
            return View(mandates);
        }
    }
}
