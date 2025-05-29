using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> History()
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
            return View(await _context.Documents.Find(u => u.MaterialId == new Guid("2a7d3f2e-9b4e-4b8d-8b7e-2c3d3a5f6d9c")).ToListAsync());
        }
        [AllowAnonymous]
        public async Task<IActionResult> Legislation()
        {
            return View(await _context.Documents.Find(u => u.MaterialId == new Guid("1a6d3f2e-8b4e-4b8d-8b7e-2c3d3a5f6d9c")).ToListAsync());
        }
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
        public IActionResult Create()
        {
            return RedirectToAction("Material");
        }

        [Route("Documents/ATA/Create")]
        public async Task<IActionResult> CreateAta()
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/ATA/Create")]
        public async Task<IActionResult> CreateAta([Bind("Id,Name,DocumentCreationDate,MeetingDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment)
        {
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {
                if (document.MeetingDate == null || document.MeetingDate == DateOnly.MinValue)
                {
                    ModelState.AddModelError("MeetingDate", "O campo Data da Reunião é obrigatório");
                    return View(document);
                }
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (attachment == null || attachment.Length == 0)
                {
                    ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
                    return View(document);
                }
                var extension = Path.GetExtension(attachment.FileName);
                if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                    return View(document);
                }
                document.Id = Guid.NewGuid();
                document.MaterialId = new Guid("9b927360-b531-4bb9-9e09-1a3093f8507a");
                document.DocumentCreationDate = DateTime.Now.AddHours(-3);
                document.Status = "Ativo";
                document.UserId = new Guid(_userManager.GetUserId(User));

                var randomFileName = $"ATA-{Guid.NewGuid()}.pdf";
                document.Attachment = Path.Combine("docs/", randomFileName);

                await _context.Documents.InsertOneAsync(document); //Insert

                var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot", "docs", randomFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }


        [Route("Documents/ATA/Edit")]
        public async Task<IActionResult> EditAta(Guid? id)
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
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documents/ATA/Edit")]
        public async Task<IActionResult> EditAta(Guid id, [Bind("Id,Name,DocumentCreationDate,MeetingDate,Status,Attachment,UserId,MandateId,MaterialId")] Document document, IFormFile? attachment, bool changeAttachment)
        {
            if (id != document.Id)
            {
                return NotFound();
            }
            var mandates = SearchMandates();
            ViewData["Mandates"] = new SelectList(mandates, "Id", "mandate");
            if (ModelState.IsValid)
            {

                if (document.MeetingDate == null || document.MeetingDate == DateOnly.MinValue)
                {
                    ModelState.AddModelError("MeetingDate", "O campo Data da Reunião é obrigatório");
                    return View(document);
                }
                if (!document.MandateId.HasValue || document.MandateId == Guid.Empty)
                {
                    ModelState.AddModelError("MandateId", "O campo Mandato é obrigatório!");
                    return View(document);
                }
                if (changeAttachment == true)
                {
                    if (attachment == null || attachment.Length == 0)
                    {
                        ModelState.AddModelError("Attachment", "O campo Anexo é obrigatório!");
                        return View(document);
                    }
                    var extension = Path.GetExtension(attachment.FileName);
                    if (string.IsNullOrEmpty(extension) || !extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("Attachment", "Somente arquivos PDF são permitidos.");
                        return View(document);
                    }

                }
                //aplicando
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
                if (changeAttachment == true)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                          "wwwroot", "docs", attachment.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(stream);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(document);
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





                //aplicando
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

        private List<dynamic> SearchMandates()
        {
            return _context.Mandates.Find(_ => true).ToList().Select(m => new { m.Id, mandate = $"{m.StartYear.Year}/{m.TerminationYear.Year}" }).ToList<dynamic>();
        }
    }
}
