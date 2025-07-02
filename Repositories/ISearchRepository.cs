using HotelBooking.Models;

namespace HotelBooking.Repositories
{
    public interface ISearchRepository
    {
        Task<IEnumerable<Room>> SearchRoomsAsync(string city, DateTime startDate, DateTime endDate, int numberOfPeople, bool isLoggedIn);
    }
}
