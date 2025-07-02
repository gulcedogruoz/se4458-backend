using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.Repositories;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/v{version}/search")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepository _searchRepository;

        public SearchController(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        [HttpGet]
        public async Task<IActionResult> SearchRooms(
            string version, // Route parametresi olarak version
            string city,
            DateTime startDate,
            DateTime endDate,
            int numberOfPeople,
            bool isLoggedIn)
        {
            var rooms = await _searchRepository.SearchRoomsAsync(city, startDate, endDate, numberOfPeople, isLoggedIn);

            // Version parametresini de response'a ekliyoruz (test ve kontrol için)
            return Ok(new
            {
                Version = version,
                Rooms = rooms
            });
        }
    }
}
