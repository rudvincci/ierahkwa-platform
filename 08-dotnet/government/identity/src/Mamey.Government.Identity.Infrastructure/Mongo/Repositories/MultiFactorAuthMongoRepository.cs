using System;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;
using Mamey;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class MultiFactorAuthMongoRepository : IMultiFactorAuthRepository
{
    private readonly IMongoRepository<MultiFactorAuthDocument, Guid> _repository;

    public MultiFactorAuthMongoRepository(IMongoRepository<MultiFactorAuthDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new MultiFactorAuthDocument(multiFactorAuth));

    public async Task UpdateAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new MultiFactorAuthDocument(multiFactorAuth));

    public async Task DeleteAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<MultiFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
            .Select(c => c.AsEntity())
            .ToList();

    public async Task<MultiFactorAuth> GetAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
    {
        var mfa = await _repository.GetAsync(id.Value);
        return mfa?.AsEntity();
    }

    public async Task<bool> ExistsAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public async Task<MultiFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var mfas = await _repository.FindAsync(m => m.UserId == userId.Value);
        return mfas.FirstOrDefault()?.AsEntity();
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        var mfas = await _repository.FindAsync(m => m.Status == status.ToString());
        return mfas.Select(m => m.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default)
    {
        var methodString = method.ToString();
        var mfas = await _repository.FindAsync(m => m.EnabledMethods.Contains(methodString));
        return mfas.Select(m => m.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<MultiFactorAuth>> GetActiveMultiFactorAuthsAsync(CancellationToken cancellationToken = default)
    {
        var mfas = await _repository.FindAsync(m => m.Status == MultiFactorAuthStatus.Active.ToString());
        return mfas.Select(m => m.AsEntity()).ToList();
    }

    public async Task<bool> HasActiveMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var mfas = await _repository.FindAsync(m => m.UserId == userId.Value && m.Status == MultiFactorAuthStatus.Active.ToString());
        return mfas.Any();
    }

    public async Task<bool> HasMethodEnabledAsync(UserId userId, MfaMethod method, CancellationToken cancellationToken = default)
    {
        var methodString = method.ToString();
        var mfas = await _repository.FindAsync(m => m.UserId == userId.Value && m.EnabledMethods.Contains(methodString));
        return mfas.Any();
    }

    public async Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var mfas = await _repository.FindAsync(m => m.UserId == userId.Value);
        foreach (var mfa in mfas)
        {
            await _repository.DeleteAsync(mfa.Id);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var mfas = await _repository.FindAsync(_ => true);
        return mfas.Count();
    }

    public async Task<int> CountByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default)
    {
        var mfas = await _repository.FindAsync(m => m.Status == status.ToString());
        return mfas.Count();
    }
}

