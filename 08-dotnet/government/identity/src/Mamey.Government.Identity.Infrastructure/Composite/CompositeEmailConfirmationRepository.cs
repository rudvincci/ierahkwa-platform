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

internal class CompositeEmailConfirmationRepository : IEmailConfirmationRepository
{
    private readonly EmailConfirmationMongoRepository _mongoRepo;
    private readonly EmailConfirmationRedisRepository _redisRepo;
    private readonly EmailConfirmationPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeEmailConfirmationRepository> _logger;

    public CompositeEmailConfirmationRepository(
        EmailConfirmationMongoRepository mongoRepo,
        EmailConfirmationRedisRepository redisRepo,
        EmailConfirmationPostgresRepository postgresRepo,
        ILogger<CompositeEmailConfirmationRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default)
    {
        // Write to primary store (Postgres), propagate to caches/analytics best-effort
        await _postgresRepo.AddAsync(emailConfirmation, cancellationToken);
        Try(() => _redisRepo.AddAsync(emailConfirmation, cancellationToken));
        Try(() => _mongoRepo.AddAsync(emailConfirmation, cancellationToken));
    }

    public async Task UpdateAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(emailConfirmation, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(emailConfirmation, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(emailConfirmation, cancellationToken));
    }

    public async Task DeleteAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public Task<IReadOnlyList<EmailConfirmation>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<EmailConfirmation> GetAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
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

    public async Task<bool> ExistsAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<EmailConfirmation> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByUserIdAsync(userId, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByUserIdAsync(userId, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<EmailConfirmation> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByEmailAsync(email, cancellationToken);
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

    public async Task<EmailConfirmation> GetByConfirmationCodeAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var fromRedis = await _redisRepo.GetByConfirmationCodeAsync(confirmationCode, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByConfirmationCodeAsync(confirmationCode, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByConfirmationCodeAsync(confirmationCode, cancellationToken);
    }

    public Task<IReadOnlyList<EmailConfirmation>> GetByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public Task<IReadOnlyList<EmailConfirmation>> GetExpiredConfirmationsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetExpiredConfirmationsAsync(cancellationToken);

    public Task<IReadOnlyList<EmailConfirmation>> GetPendingConfirmationsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetPendingConfirmationsAsync(cancellationToken);

    public async Task<bool> HasPendingConfirmationAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _redisRepo.HasPendingConfirmationAsync(userId, cancellationToken)) return true;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Postgres"); }
        
        return await _postgresRepo.HasPendingConfirmationAsync(userId, cancellationToken);
    }

    public Task<bool> HasPendingConfirmationAsync(string email, CancellationToken cancellationToken = default)
        => _postgresRepo.HasPendingConfirmationAsync(email, cancellationToken);

    public Task DeleteExpiredConfirmationsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteExpiredConfirmationsAsync(cancellationToken);

    public Task DeleteConfirmationsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteConfirmationsByUserIdAsync(userId, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.CountByStatusAsync(status, cancellationToken);

    public Task<int> CountExpiredAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.CountExpiredAsync(cancellationToken);

    public Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
        => _postgresRepo.CountExpiredAsync(before, cancellationToken);

    public Task<IReadOnlyList<EmailConfirmation>> GetPendingAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetPendingAsync(cancellationToken);

    public Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetExpiredAsync(cancellationToken);

    public Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
        => _postgresRepo.GetExpiredAsync(before, cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}


