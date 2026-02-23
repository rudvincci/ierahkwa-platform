using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface IEmailConfirmationRepository
{
    Task AddAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default);
    Task UpdateAsync(EmailConfirmation emailConfirmation, CancellationToken cancellationToken = default);
    Task DeleteAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailConfirmation>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<EmailConfirmation> GetAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
    
    // Email confirmation-specific queries
    Task<EmailConfirmation> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<EmailConfirmation> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<EmailConfirmation> GetByConfirmationCodeAsync(string confirmationCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailConfirmation>> GetByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailConfirmation>> GetExpiredConfirmationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailConfirmation>> GetPendingConfirmationsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasPendingConfirmationAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<bool> HasPendingConfirmationAsync(string email, CancellationToken cancellationToken = default);
    Task DeleteExpiredConfirmationsAsync(CancellationToken cancellationToken = default);
    Task DeleteConfirmationsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // Statistics and counting methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(EmailConfirmationStatus status, CancellationToken cancellationToken = default);
    Task<int> CountExpiredAsync(CancellationToken cancellationToken = default);
    Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default);
    
    // Additional query methods
    Task<IReadOnlyList<EmailConfirmation>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailConfirmation>> GetExpiredAsync(DateTime before, CancellationToken cancellationToken = default);
}
