using Mamey.Government.UI.Models;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Service for passport operations.
/// </summary>
public interface IPassportsService
{
    Task<PagedResult<PassportSummaryModel>> BrowseAsync(
        Guid tenantId,
        string? status = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<PassportModel?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);

    Task<PassportModel?> GetByNumberAsync(Guid tenantId, string passportNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PassportSummaryModel>> GetByCitizenAsync(Guid citizenId, CancellationToken cancellationToken = default);

    Task<Guid?> IssueAsync(IssuePassportRequest request, CancellationToken cancellationToken = default);

    Task<bool> RenewAsync(Guid tenantId, Guid id, int validityYears = 10, CancellationToken cancellationToken = default);

    Task<bool> RevokeAsync(Guid tenantId, Guid id, string reason, CancellationToken cancellationToken = default);

    Task<PassportValidationResult?> ValidateAsync(string passportNumber, CancellationToken cancellationToken = default);
}
