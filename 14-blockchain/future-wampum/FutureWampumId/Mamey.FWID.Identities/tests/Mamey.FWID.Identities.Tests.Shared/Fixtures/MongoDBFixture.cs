using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.FWID.Identities.Infrastructure.Mongo.Documents;
using Mamey.Persistence.MongoDB;
using Mamey.Persistence.MongoDB.Repositories;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Shared.Fixtures;

/// <summary>
/// Test fixture for MongoDB integration tests using Testcontainers.NET.
/// </summary>
public class MongoDBFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public string ConnectionString { get; private set; } = string.Empty;
    public IMongoClient MongoClient { get; private set; } = null!;
    public IMongoDatabase Database { get; private set; } = null!;
    internal IMongoRepository<IdentityDocument, Guid> Repository { get; private set; } = null!;

    public MongoDBFixture()
    {
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:7")
            .WithPortBinding(27017, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start the MongoDB container
        await _mongoContainer.StartAsync();
        ConnectionString = _mongoContainer.GetConnectionString();

        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add MongoDB client
        MongoClient = new MongoClient(ConnectionString);
        Database = MongoClient.GetDatabase("fwid_identities_test");

        services.AddSingleton(MongoClient);
        services.AddSingleton(Database);

        // Add MongoDB repository for IdentityDocument
        services.AddTransient<IMongoRepository<IdentityDocument, Guid>>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return new TestMongoRepository(database.GetCollection<IdentityDocument>("identities"));
        });

        ServiceProvider = services.BuildServiceProvider();

        // Get repository
        Repository = ServiceProvider.GetRequiredService<IMongoRepository<IdentityDocument, Guid>>();
    }

    public async Task DisposeAsync()
    {
        // Clean up database
        if (Database != null)
        {
            await MongoClient.DropDatabaseAsync("fwid_identities_test");
        }

        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        await _mongoContainer.DisposeAsync();
    }
}

/// <summary>
/// Test wrapper for IMongoRepository since MongoRepository is internal.
/// </summary>
internal class TestMongoRepository : IMongoRepository<IdentityDocument, Guid>
{
    private readonly IMongoCollection<IdentityDocument> _collection;

    public TestMongoRepository(IMongoCollection<IdentityDocument> collection)
    {
        _collection = collection;
    }

    public IMongoCollection<IdentityDocument> Collection => _collection;

    public Task<IdentityDocument> GetAsync(Guid id) => 
        _collection.Find(d => d.Id == id).SingleOrDefaultAsync();

    public Task<IdentityDocument> GetAsync(Expression<Func<IdentityDocument, bool>> predicate) => 
        _collection.Find(predicate).SingleOrDefaultAsync();

    public async Task<IReadOnlyList<IdentityDocument>> FindAsync(Expression<Func<IdentityDocument, bool>> predicate) => 
        await _collection.Find(predicate).ToListAsync();

    public async Task<PagedResult<IdentityDocument>> BrowseAsync<TQuery>(
        Expression<Func<IdentityDocument, bool>> predicate,
        TQuery query) where TQuery : IPagedQuery
    {
        var totalCount = await _collection.Find(predicate).CountDocumentsAsync();
        var items = await _collection.Find(predicate)
            .Skip((query.Page - 1) * query.ResultsPerPage)
            .Limit(query.ResultsPerPage)
            .ToListAsync();
        
        return PagedResult<IdentityDocument>.Create(
            items,
            query.Page,
            query.ResultsPerPage,
            (int)Math.Ceiling((double)totalCount / query.ResultsPerPage),
            (int)totalCount);
    }

    public Task AddAsync(IdentityDocument entity) => 
        _collection.InsertOneAsync(entity);

    public Task UpdateAsync(IdentityDocument entity) => 
        _collection.ReplaceOneAsync(d => d.Id == entity.Id, entity);

    public Task UpdateAsync(IdentityDocument entity, Expression<Func<IdentityDocument, bool>> predicate) => 
        _collection.ReplaceOneAsync(predicate, entity);

    public Task DeleteAsync(Guid id) => 
        _collection.DeleteOneAsync(d => d.Id == id);

    public Task DeleteAsync(Expression<Func<IdentityDocument, bool>> predicate) => 
        _collection.DeleteOneAsync(predicate);

    public Task<bool> ExistsAsync(Expression<Func<IdentityDocument, bool>> predicate) => 
        _collection.Find(predicate).AnyAsync();
}
