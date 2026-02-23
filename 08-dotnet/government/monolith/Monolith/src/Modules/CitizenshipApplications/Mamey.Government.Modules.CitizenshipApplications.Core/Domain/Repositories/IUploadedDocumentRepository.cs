using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;

internal interface IUploadedDocumentRepository
{
    Task<UploadedDocument?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(UploadedDocument document, CancellationToken cancellationToken = default);
    Task UpdateAsync(UploadedDocument document, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Lookup methods
    Task<IReadOnlyList<UploadedDocument>> GetByApplicationIdAsync(AppId applicationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UploadedDocument>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UploadedDocument>> GetByApplicationIdAndTypeAsync(AppId applicationId, string documentType, CancellationToken cancellationToken = default);
}
