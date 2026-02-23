using System.Collections.Immutable;
using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for SmsConfirmation entities.
/// </summary>
internal class SmsConfirmationPostgresRepository : ISmsConfirmationRepository
{
    private readonly IdentityDbContext _dbContext;

    public SmsConfirmationPostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(SmsConfirmation entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.SmsConfirmations.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SmsConfirmation entity, CancellationToken cancellationToken = default)
    {
        _dbContext.SmsConfirmations.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SmsConfirmationId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.SmsConfirmations
            .SingleOrDefaultAsync(sc => sc.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.SmsConfirmations.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<SmsConfirmation?> GetAsync(SmsConfirmationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SmsConfirmations
            .Where(sc => sc.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(SmsConfirmationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SmsConfirmations
            .AnyAsync(sc => sc.Id == id, cancellationToken);
    }

    public async Task<SmsConfirmation?> GetByIdentityAndCodeAsync(IdentityId identityId, string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SmsConfirmations
            .Where(sc => sc.IdentityId == identityId && sc.Code == code)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SmsConfirmation>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.SmsConfirmations
            .Where(sc => sc.IdentityId == identityId)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<IReadOnlyList<SmsConfirmation>> FindAsync(Expression<Func<SmsConfirmation, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.SmsConfirmations
            .Where(predicate)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<int> CountAsync(Expression<Func<SmsConfirmation, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbContext.SmsConfirmations.CountAsync(cancellationToken);
        
        return await _dbContext.SmsConfirmations.CountAsync(predicate, cancellationToken);
    }
}

