using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booking.Data;
using Booking.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.AspNetCore.Authorization;

namespace Booking.Controllers
{
    [Authorize]
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            dbContext = context;
            userManager = manager;
        }

        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id is null) return NotFound();

            //  olika sätt att hitta user:
            // dbContext.Users bla bla
            // User.Identity  bla bla

            var userId = userManager.GetUserId(User);

            // If not logged in
            if (userId is null) return NotFound(); 

            // Annat onödigt långt sätt att göra det på
            //var gymClass = await dbContext.GymClasses
            //    .Include(g => g.AttendedMembers)
            //    .FirstOrDefaultAsync(c => c.Id == id);

            //var attending = gymClass?
            //    .AttendedMembers.FirstOrDefault(a => a.ApplicationUserId == userId);

            var attending = dbContext.ApplicationUserGymClasses
                .Find(userId, id);

            // if user not in attendedmembers then add booking
            if (attending is null)
            {
                var booking = new ApplicationUserGymClass { ApplicationUserId = userId, GymClassId = (int)id };
                dbContext.ApplicationUserGymClasses.Add(booking);
            }
            else
            {
                dbContext.ApplicationUserGymClasses.Remove(attending);
            }
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            // How override QueryFilter
            return View(await dbContext.GymClasses
              //  .IgnoreQueryFilters()
                .ToListAsync());
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await dbContext.GymClasses
                .Include(c => c.AttendedMembers)
                .ThenInclude(a => a.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(gymClass);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await dbContext.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(gymClass);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await dbContext.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await dbContext.GymClasses.FindAsync(id);
            dbContext.GymClasses.Remove(gymClass);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return dbContext.GymClasses.Any(e => e.Id == id);
        }
    }
}
