using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stark.Models;

namespace Stark.Controllers
{
    public class CarsController : Controller
    {
       private readonly starkContext _context;

        public CarsController(starkContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cars.ToListAsync());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //the following anon type references hold the number of each type of review for this (id) Plate
            var awful = _context.Review.Count(s => s.Badge.Type == 0 && s.LicenceId == id);
            var bad = _context.Review.Count(s => s.Badge.Type == 1 && s.LicenceId == id);
            var good = _context.Review.Count(s => s.Badge.Type == 3 && s.LicenceId == id);
            var excellent = _context.Review.Count(s => s.Badge.Type == 4 && s.LicenceId == id);

            //for reading from Cars and related data using LINQ query to include related objects
            var cars = await _context.Cars 
                .Include(s=>s.Review) //
                .ThenInclude(e=>e.Badge)
                .AsNoTracking() //performance
                .SingleOrDefaultAsync(m => m.LicenceId == id); 
            if (cars == null)
            {
                return NotFound();
            }
            ViewData["awful"] = awful.ToString();
            ViewData["bad"] = bad.ToString();
            ViewData["good"] = good.ToString();
            ViewData["excellent"] = excellent.ToString();
            return View(cars);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicenceId,Plate")] Cars cars)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cars);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception) //improvised way to make sure only one record of a particular plate exists
                {
                    return RedirectToAction("CreateExists","Reviews", new { sequence = cars.Plate }); //redirects to the Create Review View for when the plate already exists in the db
                }
                return RedirectToAction("CreateNew", "Reviews", new { id = cars.LicenceId }); //redirects to the Create Review View for when the plate does not yet exists in the db
            }
            return RedirectToAction("Index", "Upload", new { saveChangesError = true }); //only called when the user does not select a plate in the view  - redirects to the Upload View so they can try again
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cars = await _context.Cars.FindAsync(id);
            if (cars == null)
            {
                return NotFound();
            }
            return View(cars);
        }
        

        // POST: Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LicenceId,Plate")] Cars cars)
        {
            if (id != cars.LicenceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cars);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarsExists(cars.LicenceId))
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
            return View(cars);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false) //saveChangesError is passed by redirect from the POST Delete if exception is caught - Used to prompt the user in the view.
        {
            if (id == null)
            {
                return NotFound();
            }

            var cars = await _context.Cars
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.LicenceId == id);
            if (cars == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Please consider referential integrity when deleting records that contain foreign key constraints.";
            }

            return View(cars);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cars = await _context.Cars
                .AsNoTracking()
                .SingleOrDefaultAsync(m =>m.LicenceId==id);
            if (cars == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Cars.Remove(cars);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException) //usually when deleting a record that triggers foreign key constraints
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool CarsExists(int id)
        {
            return _context.Cars.Any(e => e.LicenceId == id);
        }
    }
}
