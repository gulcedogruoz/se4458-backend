using HotelBooking.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICommentsService
{
    Task<List<Comment>> GetAllAsync();
    Task<Comment> GetByIdAsync(string id);
    Task<List<Comment>> GetByHotelIdAsync(string hotelId); // BUNU EKLEDİK
    Task CreateAsync(Comment comment);
    Task UpdateAsync(string id, Comment comment);
    Task DeleteAsync(string id);
}
