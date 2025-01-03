using CafesRestaurantsAPI;
using CafesRestaurantsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure CafeRestaurantDatabaseSettings
builder.Services.Configure<LocalPlacesDatabaseSettings>(
    builder.Configuration.GetSection("LocalPlacesDatabase"));

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
