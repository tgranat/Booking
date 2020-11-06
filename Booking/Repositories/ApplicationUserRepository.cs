using Booking.Data;
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

    }
}
