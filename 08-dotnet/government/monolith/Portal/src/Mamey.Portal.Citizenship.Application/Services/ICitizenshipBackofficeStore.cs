using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Requests;

namespace Mamey.Portal.Citizenship.Application.Services;

public interface ICitizenshipBackofficeStore
{
    Task<IReadOnlyList<BackofficeApplicationSummary>> GetRecentApplicationsAsync(string tenantId, int take, CancellationToken ct = default);
    Task<BackofficeApplicationDetails?> GetApplicationAsync(string tenantId, Guid id, CancellationToken ct = default);
    Task UpdateApplicationStatusByNumberAsync(string tenantId, string applicationNumber, string status, DateTimeOffset updatedAt, CancellationToken ct = default);
    Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<ApplicationCertificateSnapshot?> GetApplicationForCertificateAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<ApplicationPassportSnapshot?> GetApplicationForPassportAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<ApplicationIdCardSnapshot?> GetApplicationForIdCardAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<ApplicationVehicleTagSnapshot?> GetApplicationForVehicleTagAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<ApplicationTravelIdSnapshot?> GetApplicationForTravelIdAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<IssuedDocumentSummary?> GetIssuedDocumentAsync(string tenantId, Guid applicationId, string kind, CancellationToken ct = default);
    Task<IssuedDocumentSummary> InsertIssuedDocumentAsync(string tenantId, IssuedDocumentCreate create, CancellationToken ct = default);
    Task UpdateApplicationUpdatedAtAsync(string tenantId, Guid applicationId, DateTimeOffset updatedAt, CancellationToken ct = default);
    Task UpdateIssuedDocumentAsync(Guid issuedDocumentId, string documentNumber, DateTimeOffset? expiresAt, CancellationToken ct = default);
    Task<string?> GetApplicationEmailAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<IReadOnlyList<ApplicationReissueSnapshot>> GetApplicationsForReissueAsync(string tenantId, string email, CancellationToken ct = default);
    Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsForApplicationAsync(Guid applicationId, CancellationToken ct = default);
    Task RemoveIssuedDocumentAsync(Guid issuedDocumentId, CancellationToken ct = default);
    Task<bool> ApplicationExistsAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task UpsertIntakeReviewAsync(string tenantId, SubmitIntakeReviewRequest request, DateTimeOffset now, CancellationToken ct = default);
    Task<ApplicationDecisionSnapshot?> GetApplicationForDecisionAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task UpdateApplicationStatusAsync(
        string tenantId,
        Guid applicationId,
        string status,
        DateTimeOffset updatedAt,
        string? rejectionReason,
        CancellationToken ct = default);
    Task<IssuedDocumentSummary?> GetIssuedDocumentByIdAsync(Guid issuedDocumentId, CancellationToken ct = default);
    Task<ApplicationRenewalSnapshot?> GetApplicationForRenewalAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
}
