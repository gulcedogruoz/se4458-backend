using HotelBooking.DTO;
using HotelBooking.Models;
using HotelBooking.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/v{version}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Comment>>> GetAll(string version)
        {
            var comments = await _commentsService.GetAllAsync();
            return Ok(new
            {
                Version = version,
                Comments = comments
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetById(string version, string id)
        {
            var comment = await _commentsService.GetByIdAsync(id);
            if (comment == null)
                return NotFound();

            return Ok(new
            {
                Version = version,
                Comment = comment
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(string version, [FromBody] CommentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    Version = version,
                    Errors = ModelState
                });

            var comment = new Comment
            {
                HotelId = dto.HotelId,
                Username = dto.Username,
                Text = dto.Text,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow
            };

            await _commentsService.CreateAsync(comment);

            return CreatedAtAction(nameof(GetById),
                new { version = version, id = comment.Id.ToString() },
                new
                {
                    Version = version,
                    Comment = comment
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string version, string id, [FromBody] CommentCreateDto dto)
        {
            var existing = await _commentsService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var objectId = ObjectId.Parse(id);

            var updatedComment = new Comment
            {
                Id = objectId,
                HotelId = dto.HotelId,
                Username = dto.Username,
                Text = dto.Text,
                Rating = dto.Rating,
                CreatedAt = existing.CreatedAt
            };

            await _commentsService.UpdateAsync(id, updatedComment);

            return Ok(new
            {
                Version = version,
                Message = "Comment updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string version, string id)
        {
            var existing = await _commentsService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _commentsService.DeleteAsync(id);

            return Ok(new
            {
                Version = version,
                Message = $"Comment with id {id} deleted successfully."
            });
        }

        [HttpGet("distribution")]
        public async Task<IActionResult> GetCommentsDistribution(string version)
        {
            var comments = await _commentsService.GetAllAsync();

            var distribution = comments
                .GroupBy(c => c.HotelId)
                .Select(g => new
                {
                    HotelId = g.Key,
                    CommentCount = g.Count(),
                    AverageRating = g.Average(c => c.Rating)
                }).ToList();

            return Ok(new
            {
                Version = version,
                Distribution = distribution
            });
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetCommentsByHotelId(string version, string hotelId)
        {
            var comments = await _commentsService.GetByHotelIdAsync(hotelId);

            if (comments == null || !comments.Any())
                return Ok(new { Version = version, Comments = new List<Comment>(), AverageRating = "N/A" });

            var averageRating = comments.Average(c => c.Rating).ToString("0.0");

            return Ok(new
            {
                Version = version,
                Comments = comments,
                AverageRating = averageRating
            });
        }
    }
}
