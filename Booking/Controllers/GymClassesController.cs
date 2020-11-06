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
using Booking.Models.ViewModels;
using Booking.Filters;

namespace Booking.Controllers
{
    // AutorizeFilter added in Startup.cs now
    // so I comment this out
    //[Authorize]   // Login required (except for actions when [AllowAnonymous] is used)
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            dbContext = context;
            userManager = manager;
        }

        public async Task<IActionResult> GetBookings()
        {
            var userId = userManager.GetUserId(User);
            var model = new IndexViewModel
            {
                GymClasses = await dbContext.ApplicationUserGymClasses
                    .IgnoreQueryFilters()
                    .Where(u => u.ApplicationUserId == userId)
                    .Select(g => new GymClassViewModel
                    {
                        Id = g.GymClass.Id,
                        Name = g.GymClass.Name,
                        StartDate = g.GymClass.StartDate,
                        Duration = g.GymClass.Duration,
                        IsAttending = g.GymClass.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                    })
                    .ToListAsync()
            };
            return View(nameof(Index),  model);
        }

        [RequiredIdAndModelFilter] 
        public async Task<IActionResult> BookingToggle(int? id)
        {
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

            // TODO: check that id (GymClass) exist

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
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);
            var model = new IndexViewModel
            {
                GymClasses = await dbContext.GymClasses
                    .Include(c => c.AttendedMembers)
                    .Select(g => new GymClassViewModel
                    {
                        Id = g.Id,
                        Name = g.Name,
                        StartDate = g.StartDate,
                        Duration = g.Duration,
                        IsAttending = g.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                    })
                    .ToListAsync()
            };
            return View(nameof(Index), model);
        }

        [RequiredIdAndModelFilter]
        public async Task<IActionResult> Details(int? id)
        {
            var gymClass = await dbContext.GymClasses
                .Include(c => c.AttendedMembers)
                .ThenInclude(a => a.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            return View(gymClass);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,Duration,Description")] CreateGymClassViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var gymClass = new GymClass
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    StartDate = viewModel.StartDate,
                    Duration = viewModel.Duration,
                    Description = viewModel.Description
                };

                dbContext.Add(gymClass);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [RequiredIdAndModelFilter]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var gymClass = await dbContext.GymClasses.FindAsync(id);

            if (gymClass == null)
            {
                return NotFound();
            }

            var viewModel = new EditGymClassViewModel
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                StartDate = gymClass.StartDate,
                Duration = gymClass.Duration,
                Description = gymClass.Description
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        [RequiredIdAndModelFilter]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            var gymClass = await dbContext.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
  
            return View(gymClass);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
