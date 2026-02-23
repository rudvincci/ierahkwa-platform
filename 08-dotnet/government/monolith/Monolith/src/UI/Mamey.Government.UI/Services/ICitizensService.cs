using Mamey.Government.UI.Models;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Service for citizen management operations.
/// </summary>
public interface ICitizensService
{
    Task<PagedResult<CitizenSummaryModel>> BrowseAsync(
        Guid tenantId,
        string? status = null,
        string? searchTerm = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<CitizenModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid?> CreateAsync(CreateCitizenRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, CreateCitizenRequest request, CancellationToken cancellationToken = default);

    Task<bool> SuspendAsync(Guid id, string reason, CancellationToken cancellationToken = default);

    Task<bool> ReactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
