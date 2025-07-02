using HotelBooking.Models;
using HotelBooking.Repositories;
using System.Net.Http.Json;

namespace HotelBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;

        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllBookingsAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await _bookingRepository.GetBookingByIdAsync(id);
        }

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            
            if (booking.StartDate >= booking.EndDate)
                throw new Exception("StartDate must be earlier than EndDate");

            
            var room = await _roomRepository.GetRoomByIdAsync(booking.RoomId);
            if (room == null)
                throw new Exception("Room does not exist.");

            // Capacity kontrolü
            if (booking.NumberOfPeople > room.Capacity)
                throw new Exception($"Not enough capacity. Room capacity: {room.Capacity}");

            // Kapasite azaltma
            int originalCapacity = room.Capacity;
            room.Capacity -= booking.NumberOfPeople;
            await _roomRepository.UpdateRoomAsync(room);

            // Eğer kapasite %20 altına düştüyse notification servisine POST at
            if (room.Capacity < originalCapacity * 0.2)
            {
                using (var httpClient = new HttpClient())
                {
                    var notificationPayload = new
                    {
                        RoomId = room.Id
                    };

                    Console.WriteLine("⚠️ Capacity below 20%, sending notification POST request...");

                    var response = await httpClient.PostAsJsonAsync(
                        "http://localhost:5177/api/v1/notification/send"
,
                        notificationPayload
                    );

                    Console.WriteLine($"Notification POST status: {response.StatusCode}");

                    response.EnsureSuccessStatusCode();
                }
            }


            return await _bookingRepository.AddBookingAsync(booking);
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            return await _bookingRepository.UpdateBookingAsync(booking);
        }

        public async Task DeleteBookingAsync(int id)
        {
            await _bookingRepository.DeleteBookingAsync(id);
        }
    }
}
