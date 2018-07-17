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
    public class ReviewsController : Controller
    {

        private readonly starkContext _context;

        public ReviewsController(starkContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(string searchString)
        {
            IQueryable<Review> starkContext = _context.Review.Include(r => r.Badge).Include(r => r.Licence);
            if (!String.IsNullOrEmpty(searchString))
            {
                starkContext =_context.Review.Include(r => r.Badge).Include(r => r.Licence).Where(r => r.Licence.Plate.Contains(searchString.Trim())||r.Badge.Title.Contains(searchString.Trim())||r.UserIp.Contains(searchString.Trim()));
            }
                       
            return View(await starkContext.ToListAsync());
            
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Badge)
                .Include(r => r.Licence)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            var badges = _context.Badge.Select(m => new { Text = m.Title + " -- " + (BadgeType)Enum.ToObject(typeof(BadgeType), m.Type) + " -- " + m.Description, Value = m.BadgeId }).ToList();
            ViewData["BadgeId"] = new SelectList(badges, "Value", "Text");
            ViewData["LicenceId"] = new SelectList(_context.Cars, "LicenceId", "Plate");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,LicenceId,BadgeId,CreateDate,UserIp")] Review review)
        {
            var badges = _context.Badge.Select(m => new { Text = m.Title + " -- " + (BadgeType)Enum.ToObject(typeof(BadgeType), m.Type) + " -- " + m.Description, Value = m.BadgeId }).ToList();
            string userip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(1).ToString();
            review.UserIp = userip;
            review.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BadgeId"] = new SelectList(badges, "Value", "Text");
            ViewData["LicenceId"] = new SelectList(_context.Cars, "LicenceId", "Plate", review.LicenceId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var badges = _context.Badge.Select(m => new { Text = m.Title + " -- " + (BadgeType)Enum.ToObject(typeof(BadgeType), m.Type) + " -- " + m.Description, Value = m.BadgeId }).ToList();
            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["BadgeId"] = new SelectList(badges, "Value", "Text");
            ViewData["LicenceId"] = new SelectList(_context.Cars, "LicenceId", "Plate", review.LicenceId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,LicenceId,BadgeId,CreateDate,UserIp")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
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
            var badges = _context.Badge.Select(m => new { Text = m.Title + " -- " + (BadgeType)Enum.ToObject(typeof(BadgeType), m.Type) + " -- " + m.Description, Value = m.BadgeId }).ToList();
            ViewData["BadgeId"] = new SelectList(badges, "Value", "Text");
            ViewData["LicenceId"] = new SelectList(_context.Cars, "LicenceId", "Plate", review.LicenceId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            } 

            var review = await _context.Review
                .Include(r => r.Badge)
                .Include(r => r.Licence)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Review.FindAsync(id);
            _context.Review.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.ReviewId == id);
        }
    }
}
