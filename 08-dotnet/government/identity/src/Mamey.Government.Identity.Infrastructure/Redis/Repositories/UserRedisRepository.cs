using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class UserRedisRepository : IUserRepository
{
    private readonly ICache _cache;
    private const string UserPrefix = "user:";
    private const string UsernamePrefix = "user:username:";
    private const string EmailPrefix = "user:email:";
    private const string SubjectPrefix = "user:subject:";

    public UserRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var ttl = TimeSpan.FromHours(24);
        await _cache.SetAsync($"{UserPrefix}{user.Id.Value}", user, ttl);
        await _cache.SetAsync($"{UsernamePrefix}{user.Username}", user.Id.Value, ttl);
        await _cache.SetAsync($"{EmailPrefix}{user.Email.Value}", user.Id.Value, ttl);
        await _cache.SetAsync($"{SubjectPrefix}{user.SubjectId.Value}", user.Id.Value, ttl);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await AddAsync(user, cancellationToken);
    }

    public async Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        var user = await GetAsync(id, cancellationToken);
        if (user == null) return;

        await _cache.DeleteAsync<User>($"{UserPrefix}{id.Value}");
        await _cache.DeleteAsync<Guid>($"{UsernamePrefix}{user.Username}");
        await _cache.DeleteAsync<Guid>($"{EmailPrefix}{user.Email.Value}");
        await _cache.DeleteAsync<Guid>($"{SubjectPrefix}{user.SubjectId.Value}");
    }

    public async Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return new List<User>();
    }

    public async Task<User> GetAsync(UserId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<User>($"{UserPrefix}{id.Value}");

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{UserPrefix}{id.Value}");

    public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var userId = await _cache.GetAsync<Guid>($"{UsernamePrefix}{username}");
        return userId != Guid.Empty ? await GetAsync(new UserId(userId), cancellationToken) : null;
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var userId = await _cache.GetAsync<Guid>($"{EmailPrefix}{email}");
        return userId != Guid.Empty ? await GetAsync(new UserId(userId), cancellationToken) : null;
    }

    public async Task<User> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        var userId = await _cache.GetAsync<Guid>($"{SubjectPrefix}{subjectId.Value}");
        return userId != Guid.Empty ? await GetAsync(new UserId(userId), cancellationToken) : null;
    }

    public async Task<IReadOnlyList<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return new List<User>();
    }

    public async Task<IReadOnlyList<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
    {
        return new List<User>();
    }

    public async Task<IReadOnlyList<User>> GetUsersWithFailedAttemptsAsync(int minAttempts, CancellationToken cancellationToken = default)
    {
        return new List<User>();
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{UsernamePrefix}{username}");

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{EmailPrefix}{email}");

    public async Task<bool> SubjectIdExistsAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{SubjectPrefix}{subjectId.Value}");

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> CountByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> CountLockedAsync(CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> CountWithTwoFactorAsync(CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<int> CountWithMultiFactorAsync(CancellationToken cancellationToken = default)
    {
        return 0;
    }

    public async Task<IReadOnlyList<User>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        return new List<User>();
    }

    public async Task<IReadOnlyList<User>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return new List<User>();
    }
}

