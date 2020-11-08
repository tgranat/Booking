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
using Booking.Repositories;

namespace Booking.Controllers
{
    // AutorizeFilter added in Startup.cs now
    // so I comment this out
    //[Authorize]   // Login required (except for actions when [AllowAnonymous] is used)
    public class GymClassesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly UnitOfWork unitOfWork;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            userManager = manager;
            unitOfWork = new UnitOfWork(context);
        }

        public async Task<IActionResult> GetBookings()
        {
            var userId = userManager.GetUserId(User);
            var model = new IndexViewModel();
            var gymClasses = await unitOfWork.AppUserRepository.GetBookings(userId);

            model.GymClasses = gymClasses
                .Select(g => new GymClassViewModel
                {
                    Id = g.GymClass.Id,
                    Name = g.GymClass.Name,
                    StartDate = g.GymClass.StartDate,
                    Duration = g.GymClass.Duration,
                    IsAttending = g.GymClass.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                });
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

            var attending = unitOfWork.AppUserRepository.GetAttending(userId, id);

            // TODO: check that id (GymClass) exist

            if (attending is null)
            {
                var booking = new ApplicationUserGymClass { ApplicationUserId = userId, GymClassId = (int)id };
                unitOfWork.AppUserRepository.Add(booking);
            }
            else
            {
                unitOfWork.AppUserRepository.Remove(attending);
            }
            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(IndexViewModel viewModel = null)
        {
            var model = new IndexViewModel();
            var userId = userManager.GetUserId(User);

            if (!User.Identity.IsAuthenticated)
            {
                var gymClasses = await unitOfWork.GymClassRepository.GetAsync();
                model.GymClasses = gymClasses
                    .Select(g => new GymClassViewModel
                    {
                        Id = g.Id,
                        Name = g.Name,
                        StartDate = g.StartDate,
                        Duration = g.Duration,
                    });
            }

            // ShowHistory show old gymclasses (before todays date)

            // When ShowHistory checkbox is set in index.cshtmp, JavaScript (in site.js) submits

            if (viewModel.ShowHistory)
            {
                var gymClasses = await unitOfWork.GymClassRepository.GetHistory();

                model.GymClasses = gymClasses
                    .Select(g => new GymClassViewModel
                    {
                        Id = g.Id,
                        Name = g.Name,
                        StartDate = g.StartDate,
                        Duration = g.Duration,
                        IsAttending = g.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                    });
            }
            else           
            {
                var gymClasses = await unitOfWork.GymClassRepository.GetWithBookings();

                model.GymClasses = gymClasses
                    .Select(g => new GymClassViewModel
                    {
                        Id = g.Id,
                        Name = g.Name,
                        StartDate = g.StartDate,
                        Duration = g.Duration,
                        IsAttending = g.AttendedMembers.Any(m => m.ApplicationUserId == userId)
                    });
            };
            return View(model);
        }

        [RequiredIdAndModelFilter]
        public async Task<IActionResult> Details(int? id)
        {
            var gymClass = await unitOfWork.GymClassRepository.GetAsync(id);

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

                unitOfWork.GymClassRepository.Add(gymClass);
                await unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [RequiredIdAndModelFilter]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var gymClass = await unitOfWork.GymClassRepository.GetAsync(id);

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
                    unitOfWork.GymClassRepository.Update(gymClass);
                    await unitOfWork.CompleteAsync();
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
            var gymClass = await unitOfWork.GymClassRepository.GetAsync(id);

            return View(gymClass);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await unitOfWork.GymClassRepository.GetAsync(id);
            unitOfWork.GymClassRepository.Remove(gymClass);
            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return unitOfWork.GymClassRepository.Any(id);
        }
    }
}
