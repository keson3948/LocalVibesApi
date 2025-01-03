namespace CafesRestaurantsAPI;

public class LocalPlacesDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string PlacesCollectionName { get; set; } = null!;
    public string ReviewsCollectionName { get; set; } = null!;
    public string CategoriesCollectionName { get; set; } = null!;
}