using System.Text.Json;
using Mamey;
using Microsoft.Extensions.Logging;
using Pupitre.Users.Domain.Entities;
using Pupitre.Users.Domain.Repositories;
using StackExchange.Redis;

namespace Pupitre.Users.Infrastructure.Redis.Repositories;

internal class UserRedisRepository : IUserRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<UserRedisRepository> _logger;
    private const string KeyPrefix = "user:";
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public UserRedisRepository(IConnectionMultiplexer redis, ILogger<UserRedisRepository> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task<User> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        var json = await _database.StringGetAsync(key);
        if (json.IsNullOrEmpty) return null!;
        return JsonSerializer.Deserialize<User>(json!)!;
    }

    public async Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        var key = GetKey(entity.Id.Value);
        var json = JsonSerializer.Serialize(entity);
        await _database.StringSetAsync(key, json, DefaultExpiry);
    }

    public async Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var key = GetKey(id.Value);
        return await _database.KeyExistsAsync(key);
    }

    public Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<User>>(Array.Empty<User>());

    private static string GetKey(Guid id) => $"{KeyPrefix}{id}";
}
