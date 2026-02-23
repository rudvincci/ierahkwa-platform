using MongoDB.Driver;

namespace Mamey.Microservice.Infrastructure.Mongo
{
    internal interface IMongoSessionFactory
    {
        Task<IClientSessionHandle> CreateAsync();
    }
}