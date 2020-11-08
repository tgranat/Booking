using Booking.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Repositories
{
    public interface IApplicationUserRepository
    {
        void Add(ApplicationUserGymClass attending);
        ApplicationUserGymClass GetAttending(string userId, int? id);
        Task<IEnumerable<ApplicationUserGymClass>> GetBookings(string userId);
        void Remove(ApplicationUserGymClass attending);
    }
}