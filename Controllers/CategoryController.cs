using CafesRestaurantsAPI.Model;
using CafesRestaurantsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CafesRestaurantsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public CategoryController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    // GET: api/Category
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _mongoDbService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // POST: api/Category
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] Category newCategory)
    {
        if (newCategory == null || string.IsNullOrEmpty(newCategory.Name))
        {
            return BadRequest("Category name cannot be null or empty.");
        }

        await _mongoDbService.AddCategoryAsync(newCategory);
        return CreatedAtAction(nameof(GetAllCategories), new { id = newCategory.Id }, newCategory);
    }

    // DELETE: api/Category/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var category = await _mongoDbService.GetAllCategoriesAsync();
        if (category == null || category.All(c => c.Id != id))
        {
            return NotFound("Category not found.");
        }

        await _mongoDbService.DeleteCategoryAsync(id);
        return NoContent();
    }

    [HttpPost("createMany")]
    public async Task<IActionResult> CreateMany([FromBody] List<Category> newCategories)
    {
        if (newCategories == null || newCategories.Count == 0)
        {
            return BadRequest("The list of places cannot be empty.");
        }

        await _mongoDbService.CreateManyCategoriesAsync(newCategories);

        return Ok(newCategories);
    }
}