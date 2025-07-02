using HotelBooking.DTO;
using HotelBooking.Repositories;
using HotelBooking.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/v{version}/ai")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly IHotelRepository _hotelRepository;

        public AIController(IAIService aiService, IHotelRepository hotelRepository)
        {
            _aiService = aiService;
            _hotelRepository = hotelRepository;
        }

        [HttpPost("ask")]
        public async Task<ActionResult> AskQuestion(string version, [FromBody] AIRequestDto request)
        {
            var response = await _aiService.GetAnswerAsync(request);

            // Claude AI yanıtını parse et
            var parsed = JsonSerializer.Deserialize<JsonElement>(response.Answer);
            var intent = parsed.GetProperty("intent").GetString();

            if (intent == "SearchHotels")
            {
                var parameters = parsed.GetProperty("parameters");

                // parametreleri al
                var city = parameters.GetProperty("city").GetString();
                var numberOfPeople = parameters.GetProperty("numberOfPeople").GetInt32();
                var startDate = parameters.GetProperty("startDate").GetString();
                var endDate = parameters.GetProperty("endDate").GetString();

                // Otel araması yap
                var hotels = await _hotelRepository.GetAllHotelsAsync();
                var filteredHotels = hotels
                    .Where(h =>
                        string.Equals(NormalizeTurkish(h.City), NormalizeTurkish(city), StringComparison.OrdinalIgnoreCase)
                        && h.Rooms.Any(r =>
                            r.AvailableFrom <= DateTime.Parse(startDate)
                            && r.AvailableTo >= DateTime.Parse(endDate)
                            && r.Capacity >= numberOfPeople)
                    )
                    .ToList();

                return Ok(new
                {
                    Version = version,
                    AIResponse = response.Answer,
                    Hotels = filteredHotels
                });
            }

            // Eğer SearchHotels intent değilse, sadece AI yanıtını dön
            return Ok(new
            {
                Version = version,
                AIResponse = response.Answer
            });
        }

        private string NormalizeTurkish(string text)
        {
            return text
                .Replace("İ", "I")
                .Replace("ı", "i")
                .ToLowerInvariant();
        }
    }
}
