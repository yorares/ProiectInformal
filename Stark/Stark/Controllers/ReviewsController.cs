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
        public async Task<IActionResult> Index(string sortOrder,string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "plate_desc" : ""; //used for sort button in the View
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";//used for sort button in the View
            ViewData["CurrentFilter"] = searchString; //used for search button in the View
            IQueryable<Review> starkContext = _context.Review.Include(r => r.Badge).Include(r => r.Licence);
            if (!String.IsNullOrEmpty(searchString)) //if anything was entered in the search box
            {
                //context passed to view is now limited to only the records that match searchString - operator OR used so all the columns can be filtered
                starkContext =_context.Review.Include(r => r.Badge).Include(r => r.Licence).Where(r => r.Licence.Plate.ToUpper().Contains(searchString.ToUpper().Trim())||r.Badge.Title.ToUpper().Contains(searchString.ToUpper().Trim())||r.UserIp.Contains(searchString.Trim())||((BadgeType)r.Badge.Type).ToString().Contains(searchString.Trim())||r.CreateDate.Date.ToString().Contains(searchString.Trim()));
            }
            switch (sortOrder) //used to determine which way the context is Ordered 
            {
                case "plate_desc":
                    starkContext = starkContext.OrderByDescending(s => s.Licence.Plate);
                    break;
                case "Date":
                    starkContext = starkContext.OrderBy(s => s.CreateDate);
                    break;
                case "date_desc":
                    starkContext = starkContext.OrderByDescending(s => s.CreateDate);
                    break;
                default:
                    starkContext = starkContext.OrderBy(s => s.CreateDate);
                    break;
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
        // GET: Reviews/Create when the Plate already exists in the DB
        public IActionResult CreateExists(string sequence, bool? saveChangesError = false)
        {
            //"badges" used for the View - all the field of a Badge record are now visible in the dropdown
            var badges = _context.Badge.Select(m => new { Text = m.Title + " -- " + (BadgeType)Enum.ToObject(typeof(BadgeType), m.Type) + " -- " + m.Description, Value = m.BadgeId }).ToList(); //for the View - all the field of a Badge record are visible

            //"aux" - selects the plate from the db (matching it with the parameter "sequence" that came from Cars POST redirect
            var aux = _context.Cars.Where(r => r.Plate == sequence);
            ViewData["BadgeId"] = new SelectList(badges, "Value", "Text");
            ViewData["LicenceId"] = new SelectList(aux, "LicenceId", "Plate");
            if (saveChangesError.GetValueOrDefault()) //spam check
            {
                ViewData["ErrorMessage"] ="You've recently reviewed this driver. You can review the same driver once every 72 hours.";
            }

            return View();
        }

        // GET: Reviews/Create when the Plate did not exist in the DB before the Upload POST call (that redirected to a Cars Create POST call)
        public IActionResult CreateNew(int id)
        {
            //"badges" used for the View - all the field of a Badge record are now visible in the dropdown
            var badges = _context.Badge.Select(m => new { Text = m.Title + " -- " + (BadgeType)Enum.ToObject(typeof(BadgeType), m.Type) + " -- " + m.Description, Value = m.BadgeId }).ToList();
            //"aux" - selects the (freshly added) plate from the db (matching it with the parameter "id" that came from Cars POST redirect
            var aux = _context.Cars.Where(r => r.LicenceId == id);
            ViewData["BadgeId"] = new SelectList(badges, "Value", "Text");
            ViewData["LicenceId"] = new SelectList(aux, "LicenceId", "Plate");
            return View();
        }
        // GET: Reviews/Create - only called when adding Reviews "the admin way"
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
            //"badges" used for the View - all the field of a Badge record are now visible in the dropdown
            var badges = _context.Badge.Select(m => new { Text = m.Title + " -- " + (BadgeType)Enum.ToObject(typeof(BadgeType), m.Type) + " -- " + m.Description, Value = m.BadgeId }).ToList();
            //userip  - local variable used to get the IP of the user to be then assigned to the UserIp property of Review
            string userip = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(1).ToString();
            review.UserIp = userip;
            //spam - used to determine if the same user (same ip) has already reviewed the same Car (same LicenseId) in the last 72 hours
            bool spam = _context.Review.Any(s => s.LicenceId == review.LicenceId && s.UserIp == review.UserIp && (review.CreateDate.Subtract(s.CreateDate).TotalDays<3.0d));
            if ((ModelState.IsValid)&&(!spam))
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (spam==true)
            {
                return RedirectToAction("CreateExists", new { saveChangesError = true }); //for generating a spam error message in the View
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
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,LicenceId,BadgeId, CreateDate,UserIp")] Review review)
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
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
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
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed";
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Review.AsNoTracking().SingleOrDefaultAsync(m =>m.ReviewId ==id);
            if (review==null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Review.Remove(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.ReviewId == id);
        }
    }
}
