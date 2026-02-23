using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

public interface ICitizenPortalService
{
    Task<CitizenProfileDto?> GetProfileAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CitizenApplicationDto>> GetApplicationsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CitizenDocumentDto>> GetDocumentsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CitizenUploadDto>> GetUploadsAsync(CancellationToken ct = default);
}


