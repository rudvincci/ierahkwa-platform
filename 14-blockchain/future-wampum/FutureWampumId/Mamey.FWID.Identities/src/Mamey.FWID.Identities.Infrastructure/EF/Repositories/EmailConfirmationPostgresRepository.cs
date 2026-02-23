using System.Collections.Immutable;
using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for EmailConfirmation entities.
/// </summary>
internal class EmailConfirmationPostgresRepository : IEmailConfirmationRepository
{
    private readonly IdentityDbContext _dbContext;

    public EmailConfirmationPostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(EmailConfirmation entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.EmailConfirmations.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EmailConfirmation entity, CancellationToken cancellationToken = default)
    {
        _dbContext.EmailConfirmations.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.EmailConfirmations
            .SingleOrDefaultAsync(ec => ec.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.EmailConfirmations.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<EmailConfirmation?> GetAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .Where(ec => ec.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(EmailConfirmationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .AnyAsync(ec => ec.Id == id, cancellationToken);
    }

    public async Task<EmailConfirmation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailConfirmations
            .Where(ec => ec.Token == token)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.EmailConfirmations
            .Where(ec => ec.IdentityId == identityId)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<IReadOnlyList<EmailConfirmation>> FindAsync(Expression<Func<EmailConfirmation, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var confirmations = await _dbContext.EmailConfirmations
            .Where(predicate)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(confirmations);
    }

    public async Task<int> CountAsync(Expression<Func<EmailConfirmation, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbContext.EmailConfirmations.CountAsync(cancellationToken);
        
        return await _dbContext.EmailConfirmations.CountAsync(predicate, cancellationToken);
    }
}

