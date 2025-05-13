using CipaFatecJahu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CipaFatecJahu.Controllers
{
    public class DocumentsController : Controller
    {

        ContextMongodb _context = new ContextMongodb();

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            return View(await _context.Documents.Find(u => true).ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.Find(m => m.Id == id)
                .FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Number,DocumentCreationDate,MeetingDate,LawPublication,Situation,Attachement,UserId,MandateId,MaterialId")] Document document)
        {
            if (ModelState.IsValid)
            {
                document.Id = Guid.NewGuid();
                await _context.Documents.InsertOneAsync(document);
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Number,DocumentCreationDate,MeetingDate,LawPublication,Situation,Attachement,UserId,MandateId,MaterialId")] Document document)
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
