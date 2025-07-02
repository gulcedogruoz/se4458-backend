using HotelBooking.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HotelBooking.Services
{
    public class CommentsService : ICommentsService
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentsService(IOptions<MongoDBSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _comments = database.GetCollection<Comment>(mongoSettings.Value.CommentsCollectionName);
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _comments.Find(c => true).ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _comments.Find(c => c.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task<List<Comment>> GetByHotelIdAsync(string hotelId)
        {
            int id = int.Parse(hotelId);
            return await _comments.Find(c => c.HotelId == id).ToListAsync();
        }


        public async Task CreateAsync(Comment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            await _comments.InsertOneAsync(comment);
        }

        public async Task UpdateAsync(string id, Comment comment)
        {
            var objectId = ObjectId.Parse(id);
            comment.Id = objectId;
            await _comments.ReplaceOneAsync(c => c.Id == objectId, comment);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            await _comments.DeleteOneAsync(c => c.Id == objectId);
        }
    }
}
