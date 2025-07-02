using HotelBooking.Data;
using HotelBooking.DTO;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelBooking.Services
{
    public class AuthService : IAuthService
    {
        private readonly HotelDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(HotelDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == loginDto.Username && u.Password == loginDto.Password);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task RegisterAsync(UserRegisterDto registerDto)
        {
            // Username unique mi kontrol et
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                throw new Exception("Username is already taken.");

            var user = new User
            {
                Username = registerDto.Username,
                Password = registerDto.Password, // productionda hashlenir
                Role = registerDto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
