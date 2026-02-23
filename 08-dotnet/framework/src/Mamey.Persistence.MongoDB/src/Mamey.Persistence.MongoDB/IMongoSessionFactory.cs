using MongoDB.Driver;

namespace Mamey.Persistence.MongoDB;

public interface IMongoSessionFactory
{
    Task<IClientSessionHandle> CreateAsync();
}