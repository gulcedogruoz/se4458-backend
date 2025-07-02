using HotelBooking.DTO;

namespace HotelBooking.Services
{
    public interface IAIService
    {
        Task<AIResponseDto> GetAnswerAsync(AIRequestDto request);
    }
}
