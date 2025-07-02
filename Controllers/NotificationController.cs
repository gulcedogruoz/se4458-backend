using HotelBooking.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/v{version}/notification")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("check-capacities")]
        public async Task<IActionResult> CheckCapacities(string version)
        {
            await _notificationService.CheckHotelCapacitiesAndNotifyAdmins();

            return Ok(new
            {
                Version = version,
                Message = "Capacity check completed and notifications sent if needed."
            });
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification(string version, [FromBody] NotificationRequestDto dto)
        {
            await _notificationService.NotifyIfRoomCapacityLow(dto.RoomId);

            return Ok(new
            {
                Version = version,
                Message = $"Notification checked for room {dto.RoomId}."
            });
        }
    }

    public class NotificationRequestDto
    {
        public int RoomId { get; set; }
    }
}
