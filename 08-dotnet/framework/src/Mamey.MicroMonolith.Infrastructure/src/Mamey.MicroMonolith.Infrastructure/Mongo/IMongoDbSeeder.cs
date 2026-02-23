using System.Threading.Tasks;
using MongoDB.Driver;

namespace Mamey.MicroMonolith.Infrastructure.Mongo
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync(IMongoDatabase database);
    }
}