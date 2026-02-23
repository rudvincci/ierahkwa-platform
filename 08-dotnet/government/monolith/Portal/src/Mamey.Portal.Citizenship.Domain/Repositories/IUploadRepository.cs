using Mamey.Portal.Citizenship.Domain.Entities;

namespace Mamey.Portal.Citizenship.Domain.Repositories;

public interface IUploadRepository
{
    Task<CitizenshipUpload?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CitizenshipUpload>> GetByApplicationAsync(Guid applicationId, CancellationToken ct = default);
    Task SaveAsync(CitizenshipUpload upload, CancellationToken ct = default);
}
