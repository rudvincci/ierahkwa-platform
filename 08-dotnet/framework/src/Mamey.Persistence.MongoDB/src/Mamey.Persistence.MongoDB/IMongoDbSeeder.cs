using MongoDB.Driver;

namespace Mamey.Persistence.MongoDB;

public interface IMongoDbSeeder
{
    Task SeedAsync(IMongoDatabase database);
}