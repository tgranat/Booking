using Booking.Data;
using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Repositories
{
    public class GymClassRepository : IGymClassRepository
    {
        private ApplicationDbContext db;

        public GymClassRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<GymClass>> GetAsync()
        {
            return await db.GymClasses.ToListAsync();
        }
        public async Task<GymClass> GetAsync(int? id)
        {
            return await db.GymClasses
                .Include(c => c.AttendedMembers)
                .ThenInclude(a => a.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<GymClass>> GetHistory()
        {
            return await db.GymClasses
                        .IgnoreQueryFilters()
                        .Include(g => g.AttendedMembers)
                        .Where(g => g.StartDate < DateTime.Now).ToListAsync();
        }

        public async Task<IEnumerable<GymClass>> GetWithBookings()
        {
            return await db.GymClasses.Include(g => g.AttendedMembers).ToListAsync();
        }

        public void Add(GymClass gymClass)
        {
            db.Add(gymClass);
        }

        public void Update(GymClass gymClass)
        {
            db.Update(gymClass);
        }

        public void Remove(GymClass gymClass)
        {
            db.Remove(gymClass);
        }

        public bool Any(int? id)
        {
            return db.GymClasses.Any(e => e.Id == id);
        }
    }
}
