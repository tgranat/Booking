using Booking.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Booking.Repositories
{
    public interface IGymClassRepository
    {
        void Add(GymClass gymClass);
        bool Any(int? id);
        Task<IEnumerable<GymClass>> GetAsync();
        Task<GymClass> GetAsync(int? id);
        Task<IEnumerable<GymClass>> GetHistory();
        Task<IEnumerable<GymClass>> GetWithBookings();
        void Remove(GymClass gymClass);
        void Update(GymClass gymClass);
    }
}