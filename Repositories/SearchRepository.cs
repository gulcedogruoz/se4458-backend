using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly HotelDbContext _context;

        public SearchRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> SearchRoomsAsync(string city, DateTime startDate, DateTime endDate, int numberOfPeople, bool isLoggedIn)
        {
            var rooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Where(r => r.Hotel.City == city
                    && r.AvailableFrom <= startDate
                    && r.AvailableTo >= endDate
                    && r.Capacity >= numberOfPeople)
                .ToListAsync();

            if (isLoggedIn)
            {
                // %15 indirim uygula
                foreach (var room in rooms)
                {
                    room.Price = room.Price * 0.85;
                }
            }

            return rooms;
        }
    }
}
