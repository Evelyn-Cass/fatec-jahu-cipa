using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CipaFatecJahu.Controllers
{
    [Authorize(Roles = "Administrador,Secretário")]
    public class MandatesController : Controller
    {
        ContextMongodb _context = new ContextMongodb();
        private UserManager<ApplicationUser> _userManager;
        public MandatesController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        // GET: Mandates
        [Route("History/Mandates")]
        public async Task<IActionResult> History()
        {
            var mandates = await _context.Mandates
                    .Find(u => true)
                    .SortByDescending(u => u.DocumentCreationDate)
                    .ToListAsync();
            var mandatesWithUsers = mandates.Select(m =>
            {
                var user = _userManager.Users.FirstOrDefault(u => u.Id == m.UserId);
                return new MandateWithUser
                {
                    Id = m.Id,
                    StartYear = m.StartYear,
                    TerminationYear = m.TerminationYear,
                    DocumentCreationDate = m.DocumentCreationDate,
                    UserId = m.UserId,
                    UserName = user?.Name
                };
            }).ToList();

            return View(mandatesWithUsers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartYear,TerminationYear,DocumentCreationDate")] Mandate mandate)
        {
            if (ModelState.IsValid)
            {
                if (mandate.StartYear > mandate.TerminationYear)
                {
                    ModelState.AddModelError(string.Empty, "O ano de início não pode ser maior que o ano de término.");
                    return View(mandate);
                }
                var existingMandate = await _context.Mandates
                    .Find(m => m.StartYear.Year == mandate.StartYear.Year && m.TerminationYear.Year == mandate.TerminationYear.Year)
                    .FirstOrDefaultAsync();
                if (existingMandate != null)
                {
                    ModelState.AddModelError(string.Empty, "Já existe um mandato cadastrado com esse ano de início e término.");
                    return View(mandate);
                }
                if (mandate.TerminationYear.Year - mandate.StartYear.Year != 1)
                {
                    ModelState.AddModelError(string.Empty, "O intervalo entre o ano de início e término deve ser exatamente 1 ano.");
                    return View(mandate);
                }

                mandate.DocumentCreationDate = DateTime.Now.AddHours(-3);
                mandate.Id = Guid.NewGuid();
                mandate.UserId = new Guid(_userManager.GetUserId(User));
                await _context.Mandates.InsertOneAsync(mandate);
                return RedirectToAction(nameof(History));
            }
            return View(mandate);
        }
    }
}
