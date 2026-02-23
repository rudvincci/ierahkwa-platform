using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public interface ICitizenPortalStore
{
    Task<CitizenPortalProfileSnapshot?> GetLatestProfileAsync(string tenantId, string email, CancellationToken ct = default);
    Task<IReadOnlyList<CitizenApplicationDto>> GetApplicationsAsync(string tenantId, string email, CancellationToken ct = default);
    Task<IReadOnlyList<CitizenPortalDocumentSnapshot>> GetDocumentsAsync(string tenantId, string email, CancellationToken ct = default);
    Task<IReadOnlyList<CitizenUploadDto>> GetUploadsAsync(string tenantId, string email, CancellationToken ct = default);
}
