using Booking.Data;
using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Repositories
{
    public class GymClassRepository
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
        
    }
}
