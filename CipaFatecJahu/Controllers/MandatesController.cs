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
        public async Task<IActionResult> History()
        {
            var mandates = await _context.Mandates.Find(u => true).ToListAsync();
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

        // GET: Mandates/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var mandate = await _context.Mandates.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (mandate == null)
            {
                return NotFound();
            }

            return View(mandate);
        }

        // GET: Mandates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mandates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        //// GET: Mandates/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var mandate = await _context.Mandates.Find(m => m.Id == id).FirstOrDefaultAsync();
        //    if (mandate == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(mandate);
        //}

        //// POST: Mandates/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,StartYear,TerminationYear,DocumentCreationDate")] Mandate mandate)
        //{
        //    if (id != mandate.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _context.Mandates.ReplaceOneAsync(m => m.Id == mandate.Id, mandate);
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MandateExists(mandate.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(History));
        //    }
        //    return View(mandate);
        //}
        private bool MandateExists(Guid id)
        {
            return _context.Mandates.Find(e => e.Id == id).Any();
        }
    }
}
