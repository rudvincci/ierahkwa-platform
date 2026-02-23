using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Requests;

namespace Mamey.Portal.Citizenship.Application.Services;

public interface ICitizenshipBackofficeService
{
    Task<IReadOnlyList<BackofficeApplicationSummary>> GetRecentApplicationsAsync(int take = 50, CancellationToken ct = default);

    Task<BackofficeApplicationDetails?> GetApplicationAsync(Guid id, CancellationToken ct = default);

    Task<string> CreateManualApplicationAsync(
        SubmitCitizenshipApplicationRequest request,
        IReadOnlyList<UploadFile> personalDocuments,
        UploadFile passportPhoto,
        UploadFile signatureImage,
        CancellationToken ct = default);

    Task<IReadOnlyList<IssuedDocumentSummary>> GetIssuedDocumentsAsync(Guid applicationId, CancellationToken ct = default);

    Task<IssuedDocumentSummary> GenerateCitizenshipCertificateAsync(Guid applicationId, CancellationToken ct = default);

    Task<IssuedDocumentSummary> IssuePassportAsync(Guid applicationId, CancellationToken ct = default);

    Task<IssuedDocumentSummary> IssueIdCardAsync(Guid applicationId, string variant, CancellationToken ct = default);

    Task<IssuedDocumentSummary> IssueVehicleTagAsync(Guid applicationId, string variant, CancellationToken ct = default);

    Task<IssuedDocumentSummary> IssueTravelIdAsync(Guid applicationId, CancellationToken ct = default);

    /// <summary>
    /// Re-issues all documents for a citizen when their status progresses.
    /// </summary>
    /// <param name="email">The citizen's email</param>
    /// <param name="newStatus">The new citizenship status</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of re-issued document summaries</returns>
    Task<IReadOnlyList<IssuedDocumentSummary>> ReissueDocumentsForStatusProgressionAsync(string email, CitizenshipStatus newStatus, CancellationToken ct = default);

    /// <summary>
    /// Submits an intake review (CIT-001-F) for an application
    /// </summary>
    Task SubmitIntakeReviewAsync(SubmitIntakeReviewRequest request, CancellationToken ct = default);

    /// <summary>
    /// Approves an application (moves status to "Approved" and creates Probationary citizenship status)
    /// </summary>
    Task ApproveApplicationAsync(Guid applicationId, CancellationToken ct = default);

    /// <summary>
    /// Rejects an application (moves status to "Rejected" with reason)
    /// </summary>
    Task RejectApplicationAsync(Guid applicationId, string reason, CancellationToken ct = default);

    /// <summary>
    /// Renews an expired document for a citizen (creates a new document with updated expiration based on current status)
    /// </summary>
    Task<IssuedDocumentSummary> RenewDocumentAsync(Guid expiredDocumentId, CancellationToken ct = default);
}


