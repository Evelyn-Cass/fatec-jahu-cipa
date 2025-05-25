using System.Security.Claims;
using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Driver;

namespace CipaFatecJahu.Controllers
{
    [Authorize(Roles = "Administrador,Secretário")]
    public class DocumentsController : Controller
    {

        ContextMongodb _context = new ContextMongodb();
        private UserManager<ApplicationUser> _userManager;
        public DocumentsController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            return View(await _context.Documents.Find(u => true).ToListAsync());
        }

        public IActionResult Material()
        {
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> Studies()
        {
            return View(await _context.Documents.Find(u => u.MaterialId == "2a7d3f2e-9b4e-4b8d-8b7e-2c3d3a5f6d9c").ToListAsync());
        }
        [AllowAnonymous]
        public async Task<IActionResult> Legislation()
        {
            return View(await _context.Documents.Find(u => u.MaterialId == "1a6d3f2e-8b4e-4b8d-8b7e-2c3d3a5f6d9c").ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var document = await _context.Documents.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
               return RedirectToAction("Material");
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Route("Documents/Create/Ata")]
        public IActionResult CreateAta()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/Create/Ata")]
        public async Task<IActionResult> CreateAta([Bind("Id,Name,DocumentCreationDate,MeetingDate,Status,Attachement,UserId,MandateId,MaterialId")] Document document)
        {
            if (ModelState.IsValid)
            {
                document.Id = Guid.NewGuid();
                document.MaterialId = "9b927360-b531-4bb9-9e09-1a3093f8507a";
                document.DocumentCreationDate = DateTime.Now;
                document.Status = "Ativo";

                if (_userManager == null) {
                    ModelState.AddModelError(string.Empty, "vazio");
                    return View(document);
                }
                if (User == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuário não autenticado.");
                    return View(document);
                }
                document.UserId = _userManager.GetUserId(User);

                await _context.Documents.InsertOneAsync(document);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Number,DocumentCreationDate,MeetingDate,LawPublication,Status,Attachement,UserId,MandateId,MaterialId")] Document document)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Documents.ReplaceOneAsync(m => m.Id == document.Id, document);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _context.Documents.DeleteOneAsync(m => m.Id == id);
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(Guid id)
        {
            return _context.Documents.Find(e => e.Id == id).Any();
        }
    }
}
