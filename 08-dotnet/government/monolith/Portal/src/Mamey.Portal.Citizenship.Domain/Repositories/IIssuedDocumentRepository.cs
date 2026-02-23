using Mamey.Portal.Citizenship.Domain.Entities;

namespace Mamey.Portal.Citizenship.Domain.Repositories;

public interface IIssuedDocumentRepository
{
    Task<IssuedDocument?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<IssuedDocument>> GetByApplicationAsync(Guid applicationId, CancellationToken ct = default);
    Task SaveAsync(IssuedDocument document, CancellationToken ct = default);
}
