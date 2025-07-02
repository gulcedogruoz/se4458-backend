namespace HotelBooking.DTO
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Admin" veya "Client"
    }
}
