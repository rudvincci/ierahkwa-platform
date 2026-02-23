using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface IMfaChallengeRepository
{
    Task AddAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default);
    Task UpdateAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default);
    Task DeleteAsync(MfaChallengeId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaChallenge>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<MfaChallenge> GetAsync(MfaChallengeId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(MfaChallengeId id, CancellationToken cancellationToken = default);
    
    // MFA challenge-specific queries
    Task<MfaChallenge> GetByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaChallenge>> GetByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaChallenge>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaChallenge>> GetActiveChallengesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaChallenge>> GetExpiredChallengesAsync(CancellationToken cancellationToken = default);
    Task<bool> HasActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default);
    Task DeleteExpiredChallengesAsync(CancellationToken cancellationToken = default);
    Task DeleteChallengesByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default);
    
    // Statistics and counting methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default);
    
    // Additional query methods
    Task<MfaChallenge> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default);
}
