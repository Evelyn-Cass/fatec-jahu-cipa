using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CipaFatecJahu.Data;
using CipaFatecJahu.Models;

namespace CipaFatecJahu.Controllers
{
    public class MandatesController : Controller
    {
        private readonly CipaFatecJahuContext _context;

        public MandatesController(CipaFatecJahuContext context)
        {
            _context = context;
        }

        // GET: Mandates
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mandate.ToListAsync());
        }

        // GET: Mandates/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mandate = await _context.Mandate
                .FirstOrDefaultAsync(m => m.Id == id);
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
                mandate.Id = Guid.NewGuid();
                _context.Add(mandate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mandate);
        }

        // GET: Mandates/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mandate = await _context.Mandate.FindAsync(id);
            if (mandate == null)
            {
                return NotFound();
            }
            return View(mandate);
        }

        // POST: Mandates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,StartYear,TerminationYear,DocumentCreationDate")] Mandate mandate)
        {
            if (id != mandate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mandate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MandateExists(mandate.Id))
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
            return View(mandate);
        }

        // GET: Mandates/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mandate = await _context.Mandate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mandate == null)
            {
                return NotFound();
            }

            return View(mandate);
        }

        // POST: Mandates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var mandate = await _context.Mandate.FindAsync(id);
            if (mandate != null)
            {
                _context.Mandate.Remove(mandate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MandateExists(Guid id)
        {
            return _context.Mandate.Any(e => e.Id == id);
        }
    }
}
