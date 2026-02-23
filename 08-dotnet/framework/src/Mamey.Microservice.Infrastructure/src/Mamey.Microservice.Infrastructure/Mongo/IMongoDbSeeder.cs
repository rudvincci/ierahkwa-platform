using MongoDB.Driver;

namespace Mamey.Microservice.Infrastructure.Mongo
{
    internal interface IMongoDbSeeder
    {
        Task SeedAsync(IMongoDatabase database);
    }
}