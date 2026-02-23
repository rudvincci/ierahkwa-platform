using System;
using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Types;
using Mamey;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Repositories;

internal class CredentialMongoRepository : ICredentialRepository
{
    private readonly IMongoRepository<CredentialDocument, Guid> _repository;

    public CredentialMongoRepository(IMongoRepository<CredentialDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Credential credential, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new CredentialDocument(credential));

    public async Task UpdateAsync(Credential credential, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new CredentialDocument(credential));

    public async Task DeleteAsync(CredentialId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<Credential>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
            .Select(c => c.AsEntity())
            .ToList();

    public async Task<Credential> GetAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        var credential = await _repository.GetAsync(id.Value);
        return credential?.AsEntity();
    }

    public async Task<bool> ExistsAsync(CredentialId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

    public async Task<IReadOnlyList<Credential>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentials = await _repository.FindAsync(c => c.UserId == userId.Value);
        return credentials.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetByTypeAsync(CredentialType type, CancellationToken cancellationToken = default)
    {
        var typeString = type.ToString();
        var credentials = await _repository.FindAsync(c => c.Type == typeString);
        return credentials.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetByStatusAsync(CredentialStatus status, CancellationToken cancellationToken = default)
    {
        var statusString = status.ToString();
        var credentials = await _repository.FindAsync(c => c.Status == statusString);
        return credentials.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var typeString = type.ToString();
        var credentials = await _repository.FindAsync(c => c.UserId == userId.Value && c.Type == typeString);
        return credentials.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetActiveCredentialsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentials = await _repository.FindAsync(c => c.UserId == userId.Value && 
                                                          c.Status == CredentialStatus.Active.ToString() &&
                                                          (c.ExpiresAt == null || c.ExpiresAt.Value > DateTime.UtcNow.ToUnixTimeMilliseconds()));
        return credentials.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetExpiredCredentialsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var credentials = await _repository.FindAsync(c => (c.ExpiresAt.HasValue && c.ExpiresAt.Value < now) || 
                                                          c.Status == CredentialStatus.Expired.ToString());
        return credentials.Select(c => c.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetCredentialsExpiringSoonAsync(DateTime expirationThreshold, CancellationToken cancellationToken = default)
    {
        var thresholdTimestamp = expirationThreshold.ToUnixTimeMilliseconds();
        var credentials = await _repository.FindAsync(c => c.ExpiresAt.HasValue && 
                                                          c.ExpiresAt.Value <= thresholdTimestamp &&
                                                          c.ExpiresAt.Value > DateTime.UtcNow.ToUnixTimeMilliseconds() &&
                                                          c.Status == CredentialStatus.Active.ToString());
        return credentials.Select(c => c.AsEntity()).ToList();
    }

    public async Task<bool> HasActiveCredentialOfTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var typeString = type.ToString();
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var credentials = await _repository.FindAsync(c => c.UserId == userId.Value && 
                                                          c.Type == typeString &&
                                                          c.Status == CredentialStatus.Active.ToString() &&
                                                          (c.ExpiresAt == null || c.ExpiresAt.Value > now));
        return credentials.Any();
    }

    public async Task<int> GetActiveCredentialCountAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var credentials = await _repository.FindAsync(c => c.UserId == userId.Value && 
                                                          c.Status == CredentialStatus.Active.ToString() &&
                                                          (c.ExpiresAt == null || c.ExpiresAt.Value > now));
        return credentials.Count();
    }

    public async Task DeleteExpiredCredentialsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow.ToUnixTimeMilliseconds();
        var expired = await _repository.FindAsync(c => (c.ExpiresAt.HasValue && c.ExpiresAt.Value < now) || 
                                                       c.Status == CredentialStatus.Expired.ToString());
        foreach (var credential in expired)
        {
            await _repository.DeleteAsync(credential.Id);
        }
    }

    public async Task DeleteCredentialsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentials = await _repository.FindAsync(c => c.UserId == userId.Value);
        foreach (var credential in credentials)
        {
            await _repository.DeleteAsync(credential.Id);
        }
    }

    public async Task DeleteCredentialsByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var typeString = type.ToString();
        var credentials = await _repository.FindAsync(c => c.UserId == userId.Value && c.Type == typeString);
        foreach (var credential in credentials)
        {
            await _repository.DeleteAsync(credential.Id);
        }
    }
}

