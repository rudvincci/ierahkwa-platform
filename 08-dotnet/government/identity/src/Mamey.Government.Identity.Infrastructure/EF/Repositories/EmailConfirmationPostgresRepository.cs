using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class EmailConfirmationPostgresRepository : EFRepository<EmailConfirmation, EmailConfirmationId>, IEmailConfirmationRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public EmailConfirmationPostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<EmailConfirmation>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.EmailConfirmations.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<EmailConfirmation> GetAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .Where(e => e.Id.Value == id.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .AnyAsync(e => e.Id.Value == id.Value, cancellationToken);
    }

    public async Task AddAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default)
    {
        await _dbContext.EmailConfirmations.AddAsync(emailConfirmation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default)
    {
        _dbContext.EmailConfirmations.Update(emailConfirmation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        var confirmation = await _dbContext.EmailConfirmations
            .SingleAsync(e => e.Id.Value == id.Value, cancellationToken);
        _dbContext.EmailConfirmations.Remove(confirmation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Email confirmation-specific queries
    public async Task<EmailConfirmation> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .Where(e => e.UserId.Value == userId.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<EmailConfirmation> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .Where(e => e.Email == email)
            .SingleAsync(cancellationToken);
    }

    public async Task<EmailConfirmation> GetByConfirmationCodeAsync(string confirmationCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .Where(e => e.ConfirmationCode == confirmationCode)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.EmailConfirmations
            .Where(e => e.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.EmailConfirmations
            .Where(e => e.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredConfirmationsAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.EmailConfirmations
            .Where(e => e.ExpiresAt <= before)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetPendingConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.EmailConfirmations
            .Where(e => e.Status == EmailConfirmationStatus.Pending)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await GetPendingConfirmationsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        return await GetExpiredConfirmationsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        return await GetExpiredConfirmationsAsync(before, cancellationToken);
    }

    public async Task<bool> HasPendingConfirmationAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .AnyAsync(e => e.UserId.Value == userId.Value && e.Status == EmailConfirmationStatus.Pending, cancellationToken);
    }

    public async Task<bool> HasPendingConfirmationAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .AnyAsync(e => e.Email == email && e.Status == EmailConfirmationStatus.Pending, cancellationToken);
    }

    public async Task DeleteExpiredConfirmationsAsync(CancellationToken cancellationToken = default)
    {
        var expiredConfirmations = await _dbContext.EmailConfirmations
            .Where(e => e.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        
        _dbContext.EmailConfirmations.RemoveRange(expiredConfirmations);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteConfirmationsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userConfirmations = await _dbContext.EmailConfirmations
            .Where(e => e.UserId.Value == userId.Value)
            .ToListAsync(cancellationToken);
        
        _dbContext.EmailConfirmations.RemoveRange(userConfirmations);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .CountAsync(e => e.Status == status, cancellationToken);
    }

    public async Task<int> CountExpiredAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .CountAsync(e => e.ExpiresAt <= DateTime.UtcNow, cancellationToken);
    }

    public async Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .CountAsync(e => e.ExpiresAt <= before, cancellationToken);
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
