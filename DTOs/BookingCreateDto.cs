using System;

namespace HotelBooking.DTO
{
    public class BookingCreateDto
    {
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public string Username { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfPeople { get; set; }
    }
}
