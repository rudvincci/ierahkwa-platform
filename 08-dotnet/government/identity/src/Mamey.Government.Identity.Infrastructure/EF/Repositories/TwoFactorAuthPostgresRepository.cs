using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class TwoFactorAuthPostgresRepository : EFRepository<TwoFactorAuth, TwoFactorAuthId>, ITwoFactorAuthRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public TwoFactorAuthPostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TwoFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var twoFactorAuths = await _dbContext.TwoFactorAuths.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(twoFactorAuths);
    }

    public async Task<TwoFactorAuth> GetAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TwoFactorAuths
            .Where(t => t.Id.Value == id.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TwoFactorAuths
            .AnyAsync(t => t.Id.Value == id.Value, cancellationToken);
    }

    public async Task AddAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default)
    {
        await _dbContext.TwoFactorAuths.AddAsync(twoFactorAuth, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default)
    {
        _dbContext.TwoFactorAuths.Update(twoFactorAuth);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _dbContext.TwoFactorAuths
            .SingleAsync(t => t.Id.Value == id.Value, cancellationToken);
        _dbContext.TwoFactorAuths.Remove(twoFactorAuth);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // TwoFactorAuth-specific queries
    public async Task<TwoFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TwoFactorAuths
            .Where(t => t.UserId.Value == userId.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TwoFactorAuth>> GetByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        var twoFactorAuths = await _dbContext.TwoFactorAuths
            .Where(t => t.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(twoFactorAuths);
    }

    public async Task<IReadOnlyList<TwoFactorAuth>> GetActiveTwoFactorAuthsAsync(CancellationToken cancellationToken = default)
    {
        var twoFactorAuths = await _dbContext.TwoFactorAuths
            .Where(t => t.Status == TwoFactorAuthStatus.Active)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(twoFactorAuths);
    }

    public async Task<bool> HasActiveTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TwoFactorAuths
            .AnyAsync(t => t.UserId.Value == userId.Value && t.Status == TwoFactorAuthStatus.Active, cancellationToken);
    }

    public async Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userTwoFactorAuths = await _dbContext.TwoFactorAuths
            .Where(t => t.UserId.Value == userId.Value)
            .ToListAsync(cancellationToken);
        
        _dbContext.TwoFactorAuths.RemoveRange(userTwoFactorAuths);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.TwoFactorAuths.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TwoFactorAuths
            .CountAsync(t => t.Status == status, cancellationToken);
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
