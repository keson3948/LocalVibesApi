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
    
    // PUT: api/Places/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlace(string id, [FromBody] Place updatedPlace)
    {
        // Nejprve ověř, zda místo s daným ID existuje
        var existingPlace = await _mongoDbService.GetPlaceByIdAsync(id);
        if (existingPlace == null)
        {
            return NotFound(); // Pokud místo neexistuje, vrátí 404 Not Found
        }

        // Aktualizuj místo
        updatedPlace.Id = id; // Nastav ID, aby odpovídalo aktualizovanému záznamu
        await _mongoDbService.UpdatePlaceAsync(id, updatedPlace);

        return NoContent(); // Vrací 204 No Content po úspěšné aktualizaci
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