using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class MfaChallengePostgresRepository : EFRepository<MfaChallenge, MfaChallengeId>, IMfaChallengeRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public MfaChallengePostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<MfaChallenge>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var challenges = await _dbContext.MfaChallenges.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(challenges);
    }

    public async Task<MfaChallenge> GetAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaChallenges
            .Where(m => m.Id.Value == id.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaChallenges
            .AnyAsync(m => m.Id.Value == id.Value, cancellationToken);
    }

    public async Task AddAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default)
    {
        await _dbContext.MfaChallenges.AddAsync(mfaChallenge, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default)
    {
        _dbContext.MfaChallenges.Update(mfaChallenge);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
    {
        var challenge = await _dbContext.MfaChallenges
            .SingleAsync(m => m.Id.Value == id.Value, cancellationToken);
        _dbContext.MfaChallenges.Remove(challenge);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // MfaChallenge-specific queries
    public async Task<MfaChallenge> GetByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaChallenges
            .Where(m => m.MultiFactorAuthId.Value == multiFactorAuthId.Value)
            .SingleAsync(cancellationToken);
    }

    public async Task<MfaChallenge> GetByChallengeDataAsync(string challengeData, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaChallenges
            .Where(m => m.ChallengeData == challengeData)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default)
    {
        var challenges = await _dbContext.MfaChallenges
            .Where(m => m.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(challenges);
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default)
    {
        var challenges = await _dbContext.MfaChallenges
            .Where(m => m.Method == method)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(challenges);
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetActiveChallengesAsync(CancellationToken cancellationToken = default)
    {
        var challenges = await _dbContext.MfaChallenges
            .Where(m => m.Status == MfaChallengeStatus.Pending)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(challenges);
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredChallengesAsync(CancellationToken cancellationToken = default)
    {
        var challenges = await _dbContext.MfaChallenges
            .Where(m => m.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(challenges);
    }

    public async Task<bool> HasActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        // This would require a relationship between MfaChallenge and MultiFactorAuth
        // For now, return false - implement when needed
        return false;
    }

    public async Task DeleteExpiredChallengesAsync(CancellationToken cancellationToken = default)
    {
        var expiredChallenges = await _dbContext.MfaChallenges
            .Where(m => m.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        
        _dbContext.MfaChallenges.RemoveRange(expiredChallenges);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteChallengesByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        // This would require a relationship between MfaChallenge and MultiFactorAuth
        // For now, do nothing - implement when needed
    }

    public async Task<MfaChallenge> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // This method doesn't make sense for MfaChallenge since it doesn't have UserId
        // It should be GetActiveByMultiFactorAuthIdAsync instead
        throw new NotImplementedException("GetActiveByUserIdAsync not applicable for MfaChallenge - use GetActiveByMultiFactorAuthIdAsync");
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        var challenges = await _dbContext.MfaChallenges
            .Where(m => m.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(challenges);
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        var challenges = await _dbContext.MfaChallenges
            .Where(m => m.ExpiresAt <= before)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(challenges);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaChallenges.CountAsync(cancellationToken);
    }

    public async Task<int> CountByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MfaChallenges
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
