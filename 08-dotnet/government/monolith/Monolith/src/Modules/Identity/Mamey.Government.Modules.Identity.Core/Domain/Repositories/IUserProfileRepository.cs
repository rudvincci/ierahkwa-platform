using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Domain.Repositories;

internal interface IUserProfileRepository
{
    Task<UserProfile?> GetAsync(UserId id, CancellationToken cancellationToken = default);
    Task AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserProfile>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup by Authentik identifiers
    Task<UserProfile?> GetByAuthenticatorAsync(string issuer, string subject, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
