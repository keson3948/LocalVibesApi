using CafesRestaurantsAPI.Model;
using CafesRestaurantsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CafesRestaurantsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaceController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;
    
    public PlaceController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    
    // GET: api/Places
    [HttpGet]
    public async Task<IActionResult> GetAllPlaces()
    {
        var places = await _mongoDbService.GetAllPlacesWithCategoriesAsync();
        return Ok(places);
    }

    // GET: api/Places/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlaceById(string id)
    {
        var place = await _mongoDbService.GetPlaceByIdAsync(id);
        if (place == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(place.CategoryId))
        {
            var category = await _mongoDbService.GetCategoryByIdAsync(place.CategoryId);
            if (category != null)
            {
                place.Category = category;
            }
        }

        return Ok(place);
    }

    // POST: api/Places
    [HttpPost]
    public async Task<ActionResult<Place>> Post(Place newPlace)
    {
        await _mongoDbService.AddPlaceAsync(newPlace);
        return CreatedAtAction(nameof(GetPlaceById), new { id = newPlace.Id }, newPlace);
    }

    // PUT: api/Places/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlace(string id, [FromBody] Place updatedPlace)
    {
        var existingPlace = await _mongoDbService.GetPlaceByIdAsync(id);
        if (existingPlace == null)
        {
            return NotFound();
        }

        updatedPlace.Id = id;
        await _mongoDbService.UpdatePlaceAsync(id, updatedPlace);
        return NoContent();
    }

    // DELETE: api/Places/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlace(string id)
    {
        var existingPlace = await _mongoDbService.GetPlaceByIdAsync(id);
        if (existingPlace == null)
        {
            return NotFound();
        }

        await _mongoDbService.DeletePlaceAsync(id);
        return NoContent();
    }

    // GET: api/Places/search-by-name?name={name}
    [HttpGet("search-by-name")]
    public async Task<IActionResult> SearchPlacesByName([FromQuery] string name)
    {
        var results = await _mongoDbService.SearchPlacesByNameAsync(name);
        return Ok(results);
    }
    
    [HttpPost("createMany")]
    public async Task<IActionResult> PostManyPlaces([FromBody] List<Place> newPlaces)
    {
        if (newPlaces == null || newPlaces.Count == 0)
        {
            return BadRequest("The list of places cannot be empty.");
        }

        await _mongoDbService.CreateManyPlacesAsync(newPlaces);

        return Ok(newPlaces);
    }
    
    [HttpGet("byCategory/{categoryId}")]
    public async Task<IActionResult> GetPlacesByCategory(string categoryId)
    {
        var places = await _mongoDbService.GetPlacesByCategoryAsync(categoryId);
        return Ok(places);
    }

    
    [HttpGet("byCategoryAndName")]
    public async Task<IActionResult> GetPlacesByCategoryAndName([FromQuery] string categoryId, [FromQuery] string name)
    {
        var places = await _mongoDbService.GetPlacesByCategoryAndNameAsync(categoryId, name);
        return Ok(places);
    }


}