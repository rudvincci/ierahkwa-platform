using System;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;
using Mamey;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class MfaChallengeMongoRepository : IMfaChallengeRepository
{
    private readonly IMongoRepository<MfaChallengeDocument, Guid> _repository;

    public MfaChallengeMongoRepository(IMongoRepository<MfaChallengeDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new MfaChallengeDocument(mfaChallenge));

    public async Task UpdateAsync(MfaChallenge mfaChallenge, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new MfaChallengeDocument(mfaChallenge));

    public async Task DeleteAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<MfaChallenge>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
            .Select(c => c.AsEntity())
            .ToList();

    public async Task<MfaChallenge> GetAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
    {
        var challenge = await _repository.GetAsync(id.Value);
        return challenge?.AsEntity();
    }

    public async Task<bool> ExistsAsync(MfaChallengeId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public async Task<MfaChallenge> GetByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        var challenges = await _repository.FindAsync(c => c.MultiFactorAuthId == multiFactorAuthId.Value);
        return challenges.FirstOrDefault()?.AsEntity();
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default)
    {
        var challenges = await _repository.FindAsync(c => c.Status == status.ToString());
        return challenges.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default)
    {
        var methodString = method.ToString();
        var challenges = await _repository.FindAsync(c => c.Method == methodString);
        return challenges.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetActiveChallengesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var challenges = await _repository.FindAsync(c => c.Status == MfaChallengeStatus.Pending.ToString() && c.ExpiresAt > now);
        return challenges.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredChallengesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var challenges = await _repository.FindAsync(c => c.ExpiresAt < now || c.Status == MfaChallengeStatus.Expired.ToString());
        return challenges.Select(c => c.AsEntity()).ToList();
    }

    public async Task<bool> HasActiveChallengeAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var challenges = await _repository.FindAsync(c => c.MultiFactorAuthId == multiFactorAuthId.Value && 
                                                          c.Status == MfaChallengeStatus.Pending.ToString() && 
                                                          c.ExpiresAt > now);
        return challenges.Any();
    }

    public async Task DeleteExpiredChallengesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var expired = await _repository.FindAsync(c => c.ExpiresAt < now || c.Status == MfaChallengeStatus.Expired.ToString());
        foreach (var challenge in expired)
        {
            await _repository.DeleteAsync(challenge.Id);
        }
    }

    public async Task DeleteChallengesByMultiFactorAuthIdAsync(MultiFactorAuthId multiFactorAuthId, CancellationToken cancellationToken = default)
    {
        var challenges = await _repository.FindAsync(c => c.MultiFactorAuthId == multiFactorAuthId.Value);
        foreach (var challenge in challenges)
        {
            await _repository.DeleteAsync(challenge.Id);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var challenges = await _repository.FindAsync(_ => true);
        return challenges.Count();
    }

    public async Task<int> CountByStatusAsync(MfaChallengeStatus status, CancellationToken cancellationToken = default)
    {
        var challenges = await _repository.FindAsync(c => c.Status == status.ToString());
        return challenges.Count();
    }

    public async Task<MfaChallenge> GetActiveByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        // This method requires joining with MultiFactorAuth, which is not ideal in MongoDB
        // For now, return null - this should be handled by a query handler that combines both repositories
        return null;
    }

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(CancellationToken cancellationToken = default)
        => await GetExpiredChallengesAsync(cancellationToken);

    public async Task<IReadOnlyList<MfaChallenge>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        var beforeTimestamp = before.ToUnixTimeMilliseconds();
        var challenges = await _repository.FindAsync(c => c.ExpiresAt < beforeTimestamp || c.Status == MfaChallengeStatus.Expired.ToString());
        return challenges.Select(c => c.AsEntity()).ToList();
    }
}

