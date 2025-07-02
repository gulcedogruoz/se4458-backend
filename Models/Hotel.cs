using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string City { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        // Navigation property
        public List<Room> Rooms { get; set; }
    }
}
