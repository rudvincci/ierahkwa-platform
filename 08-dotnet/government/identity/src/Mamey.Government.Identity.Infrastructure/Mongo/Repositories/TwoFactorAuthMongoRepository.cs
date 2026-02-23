using System;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;
using Mamey;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class TwoFactorAuthMongoRepository : ITwoFactorAuthRepository
{
    private readonly IMongoRepository<TwoFactorAuthDocument, Guid> _repository;

    public TwoFactorAuthMongoRepository(IMongoRepository<TwoFactorAuthDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new TwoFactorAuthDocument(twoFactorAuth));

    public async Task UpdateAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new TwoFactorAuthDocument(twoFactorAuth));

    public async Task DeleteAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<TwoFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
            .Select(c => c.AsEntity())
            .ToList();

    public async Task<TwoFactorAuth> GetAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _repository.GetAsync(id.Value);
        return twoFactorAuth?.AsEntity();
    }

    public async Task<bool> ExistsAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public async Task<TwoFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var auths = await _repository.FindAsync(a => a.UserId == userId.Value);
        return auths.FirstOrDefault()?.AsEntity();
    }

    public async Task<IReadOnlyList<TwoFactorAuth>> GetByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        var auths = await _repository.FindAsync(a => a.Status == status.ToString());
        return auths.Select(a => a.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<TwoFactorAuth>> GetActiveTwoFactorAuthsAsync(CancellationToken cancellationToken = default)
    {
        var auths = await _repository.FindAsync(a => a.Status == TwoFactorAuthStatus.Active.ToString());
        return auths.Select(a => a.AsEntity()).ToList();
    }

    public async Task<bool> HasActiveTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var auths = await _repository.FindAsync(a => a.UserId == userId.Value && a.Status == TwoFactorAuthStatus.Active.ToString());
        return auths.Any();
    }

    public async Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var auths = await _repository.FindAsync(a => a.UserId == userId.Value);
        foreach (var auth in auths)
        {
            await _repository.DeleteAsync(auth.Id);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var auths = await _repository.FindAsync(_ => true);
        return auths.Count();
    }

    public async Task<int> CountByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        var auths = await _repository.FindAsync(a => a.Status == status.ToString());
        return auths.Count();
    }
}

