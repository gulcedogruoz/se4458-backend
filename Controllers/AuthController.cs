using HotelBooking.DTO;
using HotelBooking.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/v{version}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            string version, // Route parametresi olarak version
            UserLoginDto loginDto)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto);

                // Version parametresi response'a eklenebilir, test için
                return Ok(new
                {
                    Version = version,
                    Token = token
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            string version, // Route parametresi
            UserRegisterDto registerDto)
        {
            try
            {
                await _authService.RegisterAsync(registerDto);
                return Ok(new
                {
                    Version = version,
                    Message = "User registered successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
