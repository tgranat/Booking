using Booking.Data;
using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Repositories
{
    public class ApplicationUserRepository
    {
        private ApplicationDbContext db;

        public ApplicationUserRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<ApplicationUserGymClass>> GetBookings(string userId)
        {
            return await db.ApplicationUserGymClasses
                                           .IgnoreQueryFilters()
                                           .Include(g => g.GymClass)
                                           .Where(u => u.ApplicationUserId == userId)
                                           .ToListAsync();
        }

        public ApplicationUserGymClass GetAttending(int? id, string userId)
        {
            return db.ApplicationUserGymClasses.Find(userId, id);
        }

        public void Add(ApplicationUserGymClass attending)
        {
            db.Add(attending);
        }

    }
}
