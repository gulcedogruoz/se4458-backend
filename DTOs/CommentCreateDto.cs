namespace HotelBooking.Models
{
    public class CommentCreateDto
    {
        public int HotelId { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
    }
}
