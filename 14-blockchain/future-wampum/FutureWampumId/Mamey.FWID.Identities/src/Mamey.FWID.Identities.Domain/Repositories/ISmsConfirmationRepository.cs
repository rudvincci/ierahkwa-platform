using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface ISmsConfirmationRepository
{
    Task AddAsync(SmsConfirmation entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(SmsConfirmation entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(SmsConfirmationId id, CancellationToken cancellationToken = default);
    Task<SmsConfirmation?> GetAsync(SmsConfirmationId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SmsConfirmationId id, CancellationToken cancellationToken = default);
    
    Task<SmsConfirmation?> GetByIdentityAndCodeAsync(IdentityId identityId, string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SmsConfirmation>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SmsConfirmation>> FindAsync(Expression<Func<SmsConfirmation, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<SmsConfirmation, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

