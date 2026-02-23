using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class MultiFactorAuthPostgresRepository : EFRepository<MultiFactorAuth, MultiFactorAuthId>, IMultiFactorAuthRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public MultiFactorAuthPostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var multiFactorAuths = await _dbContext.MultiFactorAuths.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(multiFactorAuths);
    }

    public async Task<MultiFactorAuth> GetAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MultiFactorAuths
            .Where(m => m.Id.Value == id.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MultiFactorAuths
            .AnyAsync(m => m.Id.Value == id.Value, cancellationToken);
    }

    public async Task AddAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default)
    {
        await _dbContext.MultiFactorAuths.AddAsync(multiFactorAuth, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default)
    {
        _dbContext.MultiFactorAuths.Update(multiFactorAuth);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _dbContext.MultiFactorAuths
            .SingleAsync(m => m.Id == id, cancellationToken);
        _dbContext.MultiFactorAuths.Remove(multiFactorAuth);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // MultiFactorAuth-specific queries
    public async Task<MultiFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MultiFactorAuths
            .Where(m => m.UserId == userId)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        var multiFactorAuths = await _dbContext.MultiFactorAuths
            .Where(m => m.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(multiFactorAuths);
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default)
    {
        // Query JSONB column for enabled methods containing the specified method
        var multiFactorAuths = await _dbContext.MultiFactorAuths
            .Where(m => m.EnabledMethods.Contains(method))
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(multiFactorAuths);
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetActiveMultiFactorAuthsAsync(CancellationToken cancellationToken = default)
    {
        var multiFactorAuths = await _dbContext.MultiFactorAuths
            .Where(m => m.Status == MultiFactorAuthStatus.Active)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(multiFactorAuths);
    }

    public async Task<bool> HasActiveMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MultiFactorAuths
            .AnyAsync(m => m.UserId.Value == userId.Value && m.Status == MultiFactorAuthStatus.Active, cancellationToken);
    }

    public async Task<bool> HasMethodEnabledAsync(UserId userId, MfaMethod method, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _dbContext.MultiFactorAuths
            .Where(m => m.UserId.Value == userId.Value)
            .FirstOrDefaultAsync(cancellationToken);
        
        return multiFactorAuth?.EnabledMethods.Contains(method) ?? false;
    }

    public async Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userMultiFactorAuths = await _dbContext.MultiFactorAuths
            .Where(m => m.UserId.Value == userId.Value)
            .ToListAsync(cancellationToken);
        
        _dbContext.MultiFactorAuths.RemoveRange(userMultiFactorAuths);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.MultiFactorAuths.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MultiFactorAuths
            .CountAsync(m => m.Status == status, cancellationToken);
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
