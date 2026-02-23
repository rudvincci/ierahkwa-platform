using Mamey.Government.UI.Models;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Service for travel identity operations.
/// </summary>
public interface ITravelIdentitiesService
{
    Task<PagedResult<TravelIdentitySummaryModel>> BrowseAsync(
        Guid tenantId,
        string? status = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<TravelIdentityModel?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);

    Task<TravelIdentityModel?> GetByNumberAsync(Guid tenantId, string idNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TravelIdentitySummaryModel>> GetByCitizenAsync(Guid citizenId, CancellationToken cancellationToken = default);

    Task<Guid?> IssueAsync(IssueTravelIdentityRequest request, CancellationToken cancellationToken = default);

    Task<bool> RenewAsync(Guid tenantId, Guid id, int validityYears = 8, CancellationToken cancellationToken = default);

    Task<bool> RevokeAsync(Guid tenantId, Guid id, string reason, CancellationToken cancellationToken = default);

    Task<TravelIdentityValidationResult?> ValidateAsync(string idNumber, CancellationToken cancellationToken = default);
}
