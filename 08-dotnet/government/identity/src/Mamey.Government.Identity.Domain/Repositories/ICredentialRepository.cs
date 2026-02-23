using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface ICredentialRepository
{
    Task AddAsync(Credential credential, CancellationToken cancellationToken = default);
    Task UpdateAsync(Credential credential, CancellationToken cancellationToken = default);
    Task DeleteAsync(CredentialId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Credential> GetAsync(CredentialId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(CredentialId id, CancellationToken cancellationToken = default);
    
    // Credential-specific queries
    Task<IReadOnlyList<Credential>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetByTypeAsync(CredentialType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetByStatusAsync(CredentialStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetActiveCredentialsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetExpiredCredentialsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Credential>> GetCredentialsExpiringSoonAsync(DateTime expirationThreshold, CancellationToken cancellationToken = default);
    Task<bool> HasActiveCredentialOfTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default);
    Task<int> GetActiveCredentialCountAsync(UserId userId, CancellationToken cancellationToken = default);
    Task DeleteExpiredCredentialsAsync(CancellationToken cancellationToken = default);
    Task DeleteCredentialsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task DeleteCredentialsByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default);
}
