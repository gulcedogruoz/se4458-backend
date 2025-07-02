using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }
        public double Price { get; set; }
        public int Capacity { get; set; }

        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }

        [ForeignKey("Hotel")]
        public int HotelId { get; set; }

        // Nullable navigation property olarak güncellendi:
        public Hotel? Hotel { get; set; }
    }
}
