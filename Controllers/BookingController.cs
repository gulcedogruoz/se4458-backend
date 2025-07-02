using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/v{version}/booking")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings(string version)
        {
            var bookings = await _bookingService.GetAllBookingsAsync();

            return Ok(new
            {
                Version = version,
                Bookings = bookings
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(string version, int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return Ok(new
            {
                Version = version,
                Booking = booking
            });
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> AddBooking(string version, Booking booking)
        {
            try
            {
                var createdBooking = await _bookingService.AddBookingAsync(booking);

                return CreatedAtAction(nameof(GetBookingById),
                    new { version = version, id = createdBooking.Id },
                    new
                    {
                        Version = version,
                        Booking = createdBooking
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Version = version,
                    Error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Booking>> UpdateBooking(string version, int id, Booking booking)
        {
            if (id != booking.Id)
                return BadRequest(new
                {
                    Version = version,
                    Error = "ID mismatch."
                });

            var updatedBooking = await _bookingService.UpdateBookingAsync(booking);

            return Ok(new
            {
                Version = version,
                Booking = updatedBooking
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(string version, int id)
        {
            await _bookingService.DeleteBookingAsync(id);

            return Ok(new
            {
                Version = version,
                Message = $"Booking with id {id} deleted successfully."
            });
        }
    }
}
