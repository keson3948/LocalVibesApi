using CafesRestaurantsAPI.Model;
using CafesRestaurantsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CafesRestaurantsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public ReviewController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    
    // GET: api/Reviews
    [HttpGet]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _mongoDbService.GetAllReviewsAsync();
        return Ok(reviews);
    }
    
    // GET: api/Reviews/place/{placeId}
    [HttpGet("place/{placeId}")]
    public async Task<IActionResult> GetReviewsByPlaceId(string placeId)
    {
        var reviews = await _mongoDbService.GetReviewsByPlaceIdAsync(placeId);
        return Ok(reviews);
    }
    
    // PUT: api/Review/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(string id, [FromBody] Review updatedReview)
    {
        var existingReview = await _mongoDbService.GetReviewByIdAsync(id);
        if (existingReview == null)
        {
            return NotFound();
        }

        updatedReview.Id = id;
        updatedReview.CreatedAt = existingReview.CreatedAt; // Preserve the original CreatedAt value
        await _mongoDbService.UpdateReviewAsync(id, updatedReview);
        return NoContent();
    }
    
    // POST: api/Reviews
    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] Review newReview)
    {
        newReview.CreatedAt = DateTime.Now; 
        await _mongoDbService.AddReviewAsync(newReview);
        return CreatedAtAction(nameof(GetReviewsByPlaceId), new { placeId = newReview.PlaceId }, newReview);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        var result = await _mongoDbService.DeleteReviewAsync(id);
        if (result.DeletedCount == 0)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpPost("createMany")]
    public async Task<IActionResult> PostManyReviews([FromBody] List<Review> newReviews)
    {
        if (newReviews == null || newReviews.Count == 0)
        {
            return BadRequest("The list of reviews cannot be empty.");
        }
    
        await _mongoDbService.CreateManyReviewsAsync(newReviews);
    
        return Ok(newReviews);
    }
}