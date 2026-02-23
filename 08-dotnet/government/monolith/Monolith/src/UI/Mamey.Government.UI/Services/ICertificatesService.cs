using Mamey.Government.UI.Models;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Service for certificate operations.
/// </summary>
public interface ICertificatesService
{
    Task<PagedResult<CertificateSummaryModel>> BrowseAsync(
        Guid tenantId,
        string? certificateType = null,
        string? status = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<CertificateModel?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);

    Task<CertificateModel?> GetByNumberAsync(Guid tenantId, string certificateNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CertificateSummaryModel>> GetByCitizenAsync(Guid citizenId, CancellationToken cancellationToken = default);

    Task<Guid?> IssueAsync(IssueCertificateRequest request, CancellationToken cancellationToken = default);

    Task<bool> RevokeAsync(Guid tenantId, Guid id, string reason, CancellationToken cancellationToken = default);

    Task<CertificateValidationResult?> ValidateAsync(string certificateNumber, CancellationToken cancellationToken = default);
}
