using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
        public string Password { get; set; }

        public string Role { get; set; } // "Admin" or "Client"
    }
}
