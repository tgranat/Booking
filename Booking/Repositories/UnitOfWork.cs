using Booking.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Repositories
{
    public class UnitOfWork
    {
        private ApplicationDbContext db;
        public GymClassRepository GymClassRepository { get; private set; }
        public ApplicationUserRepository AppUserRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            GymClassRepository = new GymClassRepository(db);
            AppUserRepository = new ApplicationUserRepository(db);
        }

        public async Task CompleteAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
