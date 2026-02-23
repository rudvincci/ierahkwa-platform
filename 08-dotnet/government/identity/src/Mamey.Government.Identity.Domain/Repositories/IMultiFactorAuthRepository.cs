using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface IMultiFactorAuthRepository
{
    Task AddAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default);
    Task UpdateAsync(MultiFactorAuth multiFactorAuth, CancellationToken cancellationToken = default);
    Task DeleteAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MultiFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<MultiFactorAuth> GetAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(MultiFactorAuthId id, CancellationToken cancellationToken = default);
    
    // Multi-factor auth-specific queries
    Task<MultiFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MultiFactorAuth>> GetByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MultiFactorAuth>> GetByMethodAsync(MfaMethod method, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MultiFactorAuth>> GetActiveMultiFactorAuthsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasActiveMultiFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> HasMethodEnabledAsync(UserId userId, MfaMethod method, CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // Statistics and counting methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(MultiFactorAuthStatus status, CancellationToken cancellationToken = default);
}
