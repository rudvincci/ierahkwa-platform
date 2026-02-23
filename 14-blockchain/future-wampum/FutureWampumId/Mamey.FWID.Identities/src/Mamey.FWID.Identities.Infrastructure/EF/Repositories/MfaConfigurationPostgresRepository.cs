using System.Collections.Immutable;
using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for MfaConfiguration entities.
/// </summary>
internal class MfaConfigurationPostgresRepository : IMfaConfigurationRepository
{
    private readonly IdentityDbContext _dbContext;

    public MfaConfigurationPostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(MfaConfiguration entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.MfaConfigurations.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MfaConfiguration entity, CancellationToken cancellationToken = default)
    {
        _dbContext.MfaConfigurations.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MfaConfigurationId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.MfaConfigurations
            .SingleOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.MfaConfigurations.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<MfaConfiguration?> GetAsync(MfaConfigurationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaConfigurations
            .Where(m => m.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(MfaConfigurationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaConfigurations
            .AnyAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<MfaConfiguration?> GetByIdentityAndMethodAsync(IdentityId identityId, MfaMethod method, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaConfigurations
            .Where(m => m.IdentityId == identityId && m.Method == method)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MfaConfiguration>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var configs = await _dbContext.MfaConfigurations
            .Where(m => m.IdentityId == identityId)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(configs);
    }

    public async Task<IReadOnlyList<MfaConfiguration>> FindAsync(Expression<Func<MfaConfiguration, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var configs = await _dbContext.MfaConfigurations
            .Where(predicate)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(configs);
    }
}

