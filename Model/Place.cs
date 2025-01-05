using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CafesRestaurantsAPI.Model;

public class Place
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Address { get; set; }
    
    public string OpeningHours { get; set; }
    
    public string? CategoryId { get; set; }
    
    public string? ImageUrl { get; set; }
    
    [BsonIgnore]
    public double AverageRating { get; set; }
    
    [BsonIgnore]
    public Category? Category { get; set; }
}