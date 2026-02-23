using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface IEmailConfirmationRepository
{
    Task AddAsync(EmailConfirmation entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(EmailConfirmation entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
    Task<EmailConfirmation?> GetAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
    
    Task<EmailConfirmation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailConfirmation>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailConfirmation>> FindAsync(Expression<Func<EmailConfirmation, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<EmailConfirmation, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

