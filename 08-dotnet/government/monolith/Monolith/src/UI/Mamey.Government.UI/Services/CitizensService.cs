using Mamey.Government.UI.Models;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Citizens service implementation.
/// </summary>
public class CitizensService : ApiServiceBase, ICitizensService
{
    private const string BaseUrl = "/api/citizens";

    public CitizensService(HttpClient httpClient, ILogger<CitizensService> logger, AppOptions appOptions) 
        : base(httpClient, logger, appOptions)
    {
    }

    public async Task<PagedResult<CitizenSummaryModel>> BrowseAsync(
        Guid tenantId,
        string? status = null,
        string? searchTerm = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryString(
            ("tenantId", tenantId),
            ("status", status),
            ("searchTerm", searchTerm),
            ("isActive", isActive),
            ("page", page),
            ("pageSize", pageSize));
        
        var result = await base.GetAsync<PagedResult<CitizenSummaryModel>>(
            $"{BaseUrl}?{query}", 
            tenantId,
            cancellationToken);
        
        return result ?? PagedResult<CitizenSummaryModel>.Empty;
    }

    public async Task<CitizenModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<CitizenModel>($"{BaseUrl}/{id}", cancellationToken);
    }

    public async Task<Guid?> CreateAsync(CreateCitizenRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<CreateCitizenRequest, CreateCitizenResponse>(
            BaseUrl, 
            request, 
            cancellationToken);
        
        return response?.CitizenId;
    }

    public async Task<bool> UpdateAsync(Guid id, CreateCitizenRequest request, CancellationToken cancellationToken = default)
    {
        return await PutAsync($"{BaseUrl}/{id}", request, cancellationToken);
    }

    public async Task<bool> SuspendAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/suspend", new SuspendCitizenRequest { Reason = reason }, cancellationToken);
    }

    public async Task<bool> ReactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/reactivate", cancellationToken);
    }

    private class CreateCitizenResponse
    {
        public Guid CitizenId { get; set; }
    }
}
