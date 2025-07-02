using HotelBooking.DTO;

namespace HotelBooking.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(UserLoginDto loginDto);
        Task RegisterAsync(UserRegisterDto registerDto);
    }
}
