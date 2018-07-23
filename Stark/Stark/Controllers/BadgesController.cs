using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stark.Models;

namespace Stark.Controllers
{
    public class BadgesController : Controller
    {
        private readonly starkContext _context;

        public BadgesController(starkContext context)
        {
            _context = context;
        }

        // GET: Badges
        public async Task<IActionResult> Index()
        {
            return View(await _context.Badge.ToListAsync());
        }

        // GET: Badges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badge
                .FirstOrDefaultAsync(m => m.BadgeId == id);
            if (badge == null)
            {
                return NotFound();
            }

            return View(badge);
        }

        // GET: Badges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Badges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BadgeId,Title,Type,Description")] Badge badge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(badge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(badge);
        }

        // GET: Badges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badge.FindAsync(id);
            if (badge == null)
            {
                return NotFound();
            }
            return View(badge);
        }

        // POST: Badges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BadgeId,Title,Type,Description")] Badge badge)
        {
            if (id != badge.BadgeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(badge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BadgeExists(badge.BadgeId))
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
            return View(badge);
        }

        // GET: Badges/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false) //saveChangesError is passed by redirect from the POST Delete if exception is caught - Used to prompt the user in the view.
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badge
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.BadgeId == id);
            if (badge == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Please consider referential integrity when deleting records that contain foreign key constraints.";
            }

            return View(badge);
        }

        // POST: Badges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var badge = await _context.Badge
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.BadgeId == id);
            if (badge == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Badge.Remove(badge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException) //usually when deleting a record that triggers foreign key constraints
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
        private bool BadgeExists(int id)
        {
            return _context.Badge.Any(e => e.BadgeId == id);
        }
    }
}
