using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.RequestResponses;
using Mamey.Government.UI.Models;
using StartApplicationRequest = Mamey.Government.UI.Models.StartApplicationRequest;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Service for citizenship application operations.
/// </summary>
public interface IApplicationsService
{
    Task<PagedResult<ApplicationSummaryModel>> BrowseAsync(
        Guid tenantId,
        string? status = null,
        string? searchTerm = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<ApplicationModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ApplicationModel?> GetByNumberAsync(string applicationNumber, CancellationToken cancellationToken = default);

    Task<Guid?> CreateAsync(CreateApplicationRequest request, CancellationToken cancellationToken = default);

    Task<bool> StartAsync(StartApplicationRequest request, CancellationToken cancellationToken = default);

    Task<ResumeApplicationResponse?> ResumeApplicationAsync(string token, string email, CancellationToken cancellationToken = default);

    Task<bool> ResumeAsync(ResumeApplicationRequest request, CancellationToken cancellationToken = default);

    Task<bool> SubmitAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ApproveAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> RejectAsync(Guid id, string reason, CancellationToken cancellationToken = default);
}
