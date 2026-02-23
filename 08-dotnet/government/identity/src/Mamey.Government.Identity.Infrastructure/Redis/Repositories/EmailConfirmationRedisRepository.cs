using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class EmailConfirmationRedisRepository : IEmailConfirmationRepository
{
    private readonly ICache _cache;
    private const string EmailConfirmationPrefix = "email-confirmation:";
    private const string CodePrefix = "email-confirmation:code:";
    private const string UserPrefix = "email-confirmation:user:";
    private const string EmailPrefix = "email-confirmation:email:";

    public EmailConfirmationRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default)
    {
        var ttl = emailConfirmation.ExpiresAt - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero) return;

        await _cache.SetAsync($"{EmailConfirmationPrefix}{emailConfirmation.Id.Value}", emailConfirmation, ttl);
        await _cache.SetAsync($"{CodePrefix}{emailConfirmation.ConfirmationCode}", emailConfirmation.Id.Value, ttl);
        await _cache.SetAsync($"{UserPrefix}{emailConfirmation.UserId.Value}", emailConfirmation.Id.Value, ttl);
        await _cache.SetAsync($"{EmailPrefix}{emailConfirmation.Email}", emailConfirmation.Id.Value, ttl);
    }

    public async Task UpdateAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default)
    {
        await AddAsync(emailConfirmation, cancellationToken);
    }

    public async Task DeleteAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        var confirmation = await GetAsync(id, cancellationToken);
        if (confirmation == null) return;

        await _cache.DeleteAsync<EmailConfirmation>($"{EmailConfirmationPrefix}{id.Value}");
        await _cache.DeleteAsync<Guid>($"{CodePrefix}{confirmation.ConfirmationCode}");
        await _cache.DeleteAsync<Guid>($"{UserPrefix}{confirmation.UserId.Value}");
        await _cache.DeleteAsync<Guid>($"{EmailPrefix}{confirmation.Email}");
    }

    public async Task<IReadOnlyList<EmailConfirmation>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis - would need to scan all keys
        return new List<EmailConfirmation>();
    }

    public async Task<EmailConfirmation> GetAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<EmailConfirmation>($"{EmailConfirmationPrefix}{id.Value}");

    public async Task<bool> ExistsAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{EmailConfirmationPrefix}{id.Value}");

    public async Task<EmailConfirmation> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var confirmationId = await _cache.GetAsync<Guid>($"{UserPrefix}{userId.Value}");
        return confirmationId != Guid.Empty ? await GetAsync(new EmailConfirmationId(confirmationId), cancellationToken) : null;
    }

    public async Task<EmailConfirmation> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var confirmationId = await _cache.GetAsync<Guid>($"{EmailPrefix}{email}");
        return confirmationId != Guid.Empty ? await GetAsync(new EmailConfirmationId(confirmationId), cancellationToken) : null;
    }

    public async Task<EmailConfirmation> GetByConfirmationCodeAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        var confirmationId = await _cache.GetAsync<Guid>($"{CodePrefix}{confirmationCode}");
        return confirmationId != Guid.Empty ? await GetAsync(new EmailConfirmationId(confirmationId), cancellationToken) : null;
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis - would need to scan all confirmations
        return new List<EmailConfirmation>();
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<EmailConfirmation>();
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetPendingConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<EmailConfirmation>();
    }

    public async Task<bool> HasPendingConfirmationAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var confirmation = await GetByUserIdAsync(userId, cancellationToken);
        return confirmation != null && confirmation.Status == EmailConfirmationStatus.Pending && !confirmation.IsExpired();
    }

    public async Task<bool> HasPendingConfirmationAsync(string email, CancellationToken cancellationToken = default)
    {
        var confirmation = await GetByEmailAsync(email, cancellationToken);
        return confirmation != null && confirmation.Status == EmailConfirmationStatus.Pending && !confirmation.IsExpired();
    }

    public async Task DeleteExpiredConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis - expired items are automatically removed by TTL
    }

    public async Task DeleteConfirmationsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var confirmation = await GetByUserIdAsync(userId, cancellationToken);
        if (confirmation != null)
        {
            await DeleteAsync(confirmation.Id, cancellationToken);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }

    public async Task<int> CountByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }

    public async Task<int> CountExpiredAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }

    public async Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return 0;
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetPendingAsync(CancellationToken cancellationToken = default)
        => await GetPendingConfirmationsAsync(cancellationToken);

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(CancellationToken cancellationToken = default)
        => await GetExpiredConfirmationsAsync(cancellationToken);

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<EmailConfirmation>();
    }
}

