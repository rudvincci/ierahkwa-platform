using System.Threading.Tasks;
using MongoDB.Driver;

namespace Mamey.MicroMonolith.Infrastructure.Mongo.Factories
{
    internal sealed class MongoSessionFactory : IMongoSessionFactory
    {
        private readonly IMongoClient _client;

        public MongoSessionFactory(IMongoClient client) 
            => _client = client;

        public Task<IClientSessionHandle> CreateAsync()
            => _client.StartSessionAsync();
    }
}