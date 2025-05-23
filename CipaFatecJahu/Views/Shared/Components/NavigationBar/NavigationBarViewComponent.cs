using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CipaFatecJahu.Views.Shared.Components.NavigationBar
{
    public class NavigationBarViewComponent : ViewComponent
    {
        ContextMongodb _context = new ContextMongodb();
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mandates = await _context.Mandates.Find(u => true).ToListAsync();

            var claimsPrincipal = User as ClaimsPrincipal;
            string? firstName = null;
            if (claimsPrincipal != null)
            {
                var firstNameClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "firstName");
                firstName = firstNameClaim != null ? firstNameClaim.Value : claimsPrincipal.Identity?.Name;
            }

            ViewBag.FirstName = firstName;

            return View(mandates);
        }
    }
}
