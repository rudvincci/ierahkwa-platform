using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Bookstore.Domain.Entities;
using Pupitre.Bookstore.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Bookstore.Infrastructure.Redis.Repositories;

internal class BookRedisRepository : IBookRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<BookRedisRepository> _logger;
    private const string KeyPrefix = "book:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public BookRedisRepository(IConnectionMultiplexer redis, ILogger<BookRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(Book entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<Book> GetAsync(BookId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<Book>(json!)!;
    }

    public async Task UpdateAsync(Book entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(BookId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(BookId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<Book>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Book>>(Array.Empty<Book>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
