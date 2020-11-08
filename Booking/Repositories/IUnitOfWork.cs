using System.Threading.Tasks;

namespace Booking.Repositories
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository AppUserRepository { get; }
        IGymClassRepository GymClassRepository { get; }

        Task CompleteAsync();
    }
}