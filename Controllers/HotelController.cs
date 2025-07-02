using HotelBooking.Models;
using HotelBooking.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/v{version}/hotel")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetAllHotels(string version)
        {
            var hotels = await _hotelRepository.GetAllHotelsAsync();

            return Ok(new
            {
                Version = version,
                Hotels = hotels
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotelById(string version, int id)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(id);
            if (hotel == null)
                return NotFound();

            return Ok(new
            {
                Version = version,
                Hotel = hotel
            });
        }

        [HttpPost]
        public async Task<ActionResult<Hotel>> AddHotel(string version, Hotel hotel)
        {
            var createdHotel = await _hotelRepository.AddHotelAsync(hotel);

            return CreatedAtAction(nameof(GetHotelById),
                new { version = version, id = createdHotel.Id },
                new
                {
                    Version = version,
                    Hotel = createdHotel
                });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Hotel>> UpdateHotel(string version, int id, Hotel hotel)
        {
            if (id != hotel.Id)
                return BadRequest(new
                {
                    Version = version,
                    Error = "ID mismatch."
                });

            var updatedHotel = await _hotelRepository.UpdateHotelAsync(hotel);

            return Ok(new
            {
                Version = version,
                Hotel = updatedHotel
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(string version, int id)
        {
            await _hotelRepository.DeleteHotelAsync(id);

            return Ok(new
            {
                Version = version,
                Message = $"Hotel with id {id} deleted successfully."
            });
        }
    }
}
