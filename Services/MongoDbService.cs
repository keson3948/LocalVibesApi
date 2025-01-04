using Microsoft.Extensions.Options;
using CafesRestaurantsAPI.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CafesRestaurantsAPI.Services;

public class MongoDbService
{
    private readonly IMongoCollection<Place> _placesCollection;
    private readonly IMongoCollection<Review> _reviewsCollection;
    private readonly IMongoCollection<Category> _categoriesCollection;

    public MongoDbService(IOptions<LocalPlacesDatabaseSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);

        _placesCollection = database.GetCollection<Place>(settings.Value.PlacesCollectionName);
        _reviewsCollection = database.GetCollection<Review>(settings.Value.ReviewsCollectionName);
        _categoriesCollection = database.GetCollection<Category>(settings.Value.CategoriesCollectionName);
    }
    
    // CRUD methods for Places
    public async Task<List<Place>> GetAllPlacesAsync() =>
        await _placesCollection.Find(_ => true).ToListAsync();

    public async Task<Place?> GetPlaceByIdAsync(string id) =>
        await _placesCollection.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task AddPlaceAsync(Place place) =>
        await _placesCollection.InsertOneAsync(place);

    public async Task UpdatePlaceAsync(string id, Place updatedPlace) =>
        await _placesCollection.ReplaceOneAsync(p => p.Id == id, updatedPlace);

    public async Task DeletePlaceAsync(string id) =>
        await _placesCollection.DeleteOneAsync(p => p.Id == id);
    
    public async Task<List<Place>> SearchPlacesByNameAsync(string name)
    {
        var filter = Builders<Place>.Filter.Regex(p => p.Name, new BsonRegularExpression(name, "i"));
        return await _placesCollection.Find(filter).ToListAsync();
    }

    public async Task<List<Place>> GetPlacesByCategoryAsync(string categoryId)
    {
        var filter = Builders<Place>.Filter.Eq(p => p.CategoryId, categoryId);
        return await _placesCollection.Find(filter).ToListAsync();
    }
    
    public async Task<List<Place>> GetPlacesByCategoryAndNameAsync(string categoryId, string name)
    {
        var filter = Builders<Place>.Filter.And(
            Builders<Place>.Filter.Eq(p => p.CategoryId, categoryId),
            Builders<Place>.Filter.Regex(p => p.Name, new BsonRegularExpression(name, "i"))
        );
        return await _placesCollection.Find(filter).ToListAsync();
    }
    
    public async Task<List<Place>> GetPlacesPaginatedAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than 0.");
        }

        int skip = (pageNumber - 1) * pageSize;

        return await _placesCollection
            .Find(_ => true)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();
    }
    
    public async Task CreateManyPlacesAsync(List<Place> newPlaces) =>
        await _placesCollection.InsertManyAsync(newPlaces);

    public async Task<List<Place>> GetPlacesPaginatedAndSortedByRatingAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("Page number and page size must be greater than 0.");
        }

        int skip = (pageNumber - 1) * pageSize;

        var pipeline = new[]
        {
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "Reviews" },
                { "localField", "_id" },
                { "foreignField", "PlaceId" },
                { "as", "Reviews" }
            }),
            new BsonDocument("$addFields", new BsonDocument
            {
                { "AverageRating", new BsonDocument("$avg", "$Reviews.Rating") }
            }),
            new BsonDocument("$sort", new BsonDocument("AverageRating", -1)),
            new BsonDocument("$skip", skip),
            new BsonDocument("$limit", pageSize),
            new BsonDocument("$project", new BsonDocument
            {
                { "name", 1 },
                { "address", 1 },
                { "opening_hours", 1 },
                { "category_id", 1 },
                { "image_url", 1 },
                { "AverageRating", 1 }
            })
        };

        var result = await _placesCollection.Aggregate<Place>(pipeline).ToListAsync();
        return result;
    }
    
    public async Task<List<Place>> GetAllPlacesWithCategoriesAsync()
    {
        var places = await _placesCollection.Find(_ => true).ToListAsync();
        foreach (var place in places)
        {
            if (!string.IsNullOrEmpty(place.CategoryId))
            {
                var category = await GetCategoryByIdAsync(place.CategoryId);
                if (category != null)
                {
                    place.Category = category;
                }
            }
        }
        return places;
    }
    
    public async Task<Category?> GetCategoryByIdAsync(string id) =>
        await _categoriesCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
    
    // CRUD methods for Reviews
    public async Task<List<Review>> GetReviewsByPlaceIdAsync(string placeId) =>
        await _reviewsCollection.Find(r => r.PlaceId == placeId).ToListAsync();

    public async Task AddReviewAsync(Review review) =>
        await _reviewsCollection.InsertOneAsync(review);

    public async Task<DeleteResult> DeleteReviewAsync(string id)
    {
        return await _reviewsCollection.DeleteOneAsync(r => r.Id == id);
    }
    
    public async Task CreateManyReviewsAsync(List<Review> newReviews) =>
        await _reviewsCollection.InsertManyAsync(newReviews);

    // CRUD methods for Categories
    public async Task<List<Category>> GetAllCategoriesAsync() =>
        await _categoriesCollection.Find(_ => true).ToListAsync();

    public async Task AddCategoryAsync(Category category) =>
        await _categoriesCollection.InsertOneAsync(category);

    public async Task DeleteCategoryAsync(string id) =>
        await _categoriesCollection.DeleteOneAsync(c => c.Id == id);
    
    public async Task CreateManyCategoriesAsync(List<Category> newCategories) =>
            await _categoriesCollection.InsertManyAsync(newCategories);
}