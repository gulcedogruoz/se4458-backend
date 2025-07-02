using HotelBooking.Data;
using HotelBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/v{version}/room")]
    public class RoomController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public RoomController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomsByHotelId(
            string version,
            int hotelId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Rooms.Where(r => r.HotelId == hotelId);

            var totalRecords = await query.CountAsync();

            var rooms = await query
                .OrderBy(r => r.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Version = version,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Rooms = rooms
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoomById(string version, int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            return Ok(new
            {
                Version = version,
                Room = room
            });
        }

        [HttpPost]
        public async Task<ActionResult<Room>> AddRoom(string version, Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoomById),
                new { version = version, id = room.Id },
                new
                {
                    Version = version,
                    Room = room
                });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Room>> UpdateRoom(string version, int id, Room room)
        {
            if (id != room.Id)
                return BadRequest(new
                {
                    Version = version,
                    Error = "ID mismatch."
                });

            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Version = version,
                Room = room
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(string version, int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Version = version,
                Message = $"Room with id {id} deleted successfully."
            });
        }
    }
}
