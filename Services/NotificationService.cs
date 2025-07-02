using HotelBooking.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services
{
    public class NotificationService
    {
        private readonly HotelDbContext _context;
        private readonly EmailService _emailService;

        public NotificationService(HotelDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task CheckHotelCapacitiesAndNotifyAdmins()
        {
            var hotels = await _context.Hotels.Include(h => h.Rooms).ToListAsync();

            foreach (var hotel in hotels)
            {
                double totalRemainingCapacity = hotel.Rooms.Sum(r => r.Capacity);
                double totalRoomCapacity = hotel.Rooms.Count * 5; // varsayılan max 5 kişi/oda
                double capacityRatio = totalRemainingCapacity / totalRoomCapacity;

                if (capacityRatio < 0.2)
                {
                    string subject = $"⚠️ ALERT: {hotel.Name} capacity below 20%";
                    string body = $"Hotel '{hotel.Name}' capacity is at {capacityRatio * 100:F1}%.";

                    _emailService.SendEmail("youremail@gmail.com", subject, body);

                    Console.WriteLine($"[Email Sent] {subject}");
                }
            }
        }

        public async Task NotifyIfRoomCapacityLow(int roomId)
        {
            var room = await _context.Rooms.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null) return;

            double capacityRatio = room.Capacity / 5.0; // varsayılan max 5 kişi/oda

            if (capacityRatio < 0.2)
            {
                string subject = $"⚠️ ALERT: {room.Hotel.Name} Room {room.Type} capacity below 20%";
                string body = $"Room '{room.Type}' in hotel '{room.Hotel.Name}' has only {room.Capacity} remaining capacity.";

                _emailService.SendEmail("youremail@gmail.com", subject, body);

                Console.WriteLine($"[Email Sent] {subject}");
            }
        }
    }
}
