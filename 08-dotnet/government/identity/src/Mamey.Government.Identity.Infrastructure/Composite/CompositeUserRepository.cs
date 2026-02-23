using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Composite;

internal class CompositeUserRepository : IUserRepository
{
    private readonly UserMongoRepository _mongoRepo;
    private readonly UserRedisRepository _redisRepo;
    private readonly UserPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeUserRepository> _logger;

    public CompositeUserRepository(
        UserMongoRepository mongoRepo,
        UserRedisRepository redisRepo,
        UserPostgresRepository postgresRepo,
        ILogger<CompositeUserRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(user, cancellationToken);
        Try(() => _redisRepo.AddAsync(user, cancellationToken));
        Try(() => _mongoRepo.AddAsync(user, cancellationToken));
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(user, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(user, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(user, cancellationToken));
    }

    public async Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public async Task<User> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
        // Try Redis → Mongo → Postgres
        try
        {
            var fromRedis = await _redisRepo.GetAsync(id, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetAsync(id, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public Task<IReadOnlyList<User>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByUsernameAsync(username, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByUsernameAsync(username, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByUsernameAsync(username, cancellationToken);
    }

    public async Task<User> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByEmailAsync(email.Value, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByEmailAsync(email, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByEmailAsync(email, cancellationToken);
    }

    public async Task<User> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetBySubjectIdAsync(subjectId, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetBySubjectIdAsync(subjectId, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetBySubjectIdAsync(subjectId, cancellationToken);
    }

    public Task<IReadOnlyList<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public Task<IReadOnlyList<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(UserStatus.Active, cancellationToken);

    public Task<IReadOnlyList<User>> GetRecentlyAuthenticatedAsync(DateTime since, CancellationToken cancellationToken = default)
        => _postgresRepo.GetRecentlyAuthenticatedAsync(since, cancellationToken);

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.UsernameExistsAsync(username, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis check failed, trying Postgres"); }
        
        return await _postgresRepo.UsernameExistsAsync(username, cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string username, UserId excludeId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Method not implemented in PostgreSQL repository");

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.EmailExistsAsync(email.Value, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis check failed, trying Postgres"); }
        
        return await _postgresRepo.EmailExistsAsync(email, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(Email email, UserId excludeId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Method not implemented in PostgreSQL repository");

    public async Task<bool> SubjectIdExistsAsync(SubjectId subjectId, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.SubjectIdExistsAsync(subjectId, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis check failed, trying Postgres"); }
        
        return await _postgresRepo.SubjectIdExistsAsync(subjectId, cancellationToken);
    }

    public Task<bool> SubjectIdExistsAsync(SubjectId subjectId, UserId excludeId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Method not implemented in PostgreSQL repository");

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(status, cancellationToken);

    public Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(UserStatus.Active, cancellationToken);

    // Additional methods required by IUserRepository interface
    public Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByEmailAsync(email, cancellationToken);

    public Task<IReadOnlyList<User>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetLockedUsersAsync(cancellationToken);

    public Task<IReadOnlyList<User>> GetUsersWithFailedAttemptsAsync(int minAttempts, CancellationToken cancellationToken = default)
        => _postgresRepo.GetUsersWithFailedAttemptsAsync(minAttempts, cancellationToken);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.EmailExistsAsync(email, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis check failed, trying Postgres"); }
        
        return await _postgresRepo.EmailExistsAsync(email, cancellationToken);
    }

    public Task<int> CountLockedAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountLockedAsync(cancellationToken);

    public Task<int> CountWithTwoFactorAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountWithTwoFactorAsync(cancellationToken);

    public Task<int> CountWithMultiFactorAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountWithMultiFactorAsync(cancellationToken);

    public Task<IReadOnlyList<User>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByRoleIdAsync(roleId, cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}
