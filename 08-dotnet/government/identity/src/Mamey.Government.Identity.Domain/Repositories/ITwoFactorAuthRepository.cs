using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface ITwoFactorAuthRepository
{
    Task AddAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default);
    Task UpdateAsync(TwoFactorAuth twoFactorAuth, CancellationToken cancellationToken = default);
    Task DeleteAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TwoFactorAuth>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<TwoFactorAuth> GetAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TwoFactorAuthId id, CancellationToken cancellationToken = default);
    
    // Two-factor auth-specific queries
    Task<TwoFactorAuth> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TwoFactorAuth>> GetByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TwoFactorAuth>> GetActiveTwoFactorAuthsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasActiveTwoFactorAuthAsync(UserId userId, CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // Statistics and counting methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(TwoFactorAuthStatus status, CancellationToken cancellationToken = default);
}
