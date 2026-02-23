using Mamey.Government.UI.Models;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Travel identities service implementation.
/// </summary>
public class TravelIdentitiesService : ApiServiceBase, ITravelIdentitiesService
{
    private const string BaseUrl = "/api/travel-identities";

    public TravelIdentitiesService(HttpClient httpClient, ILogger<TravelIdentitiesService> logger, AppOptions appOptions) 
        : base(httpClient, logger, appOptions)
    {
    }

    public async Task<PagedResult<TravelIdentitySummaryModel>> BrowseAsync(
        Guid tenantId,
        string? status = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryString(
            ("tenantId", tenantId),
            ("status", status),
            ("searchTerm", searchTerm),
            ("page", page),
            ("pageSize", pageSize));
        
        var result = await base.GetAsync<PagedResult<TravelIdentitySummaryModel>>(
            $"{BaseUrl}?{query}", 
            cancellationToken);
        
        return result ?? PagedResult<TravelIdentitySummaryModel>.Empty;
    }

    public async Task<TravelIdentityModel?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<TravelIdentityModel>($"{BaseUrl}/{id}?tenantId={tenantId}", cancellationToken);
    }

    public async Task<TravelIdentityModel?> GetByNumberAsync(Guid tenantId, string idNumber, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<TravelIdentityModel>($"{BaseUrl}/by-number/{Uri.EscapeDataString(idNumber)}?tenantId={tenantId}", cancellationToken);
    }

    public async Task<IReadOnlyList<TravelIdentitySummaryModel>> GetByCitizenAsync(Guid citizenId, CancellationToken cancellationToken = default)
    {
        var result = await base.GetAsync<List<TravelIdentitySummaryModel>>($"{BaseUrl}/citizen/{citizenId}", cancellationToken);
        return result ?? new List<TravelIdentitySummaryModel>();
    }

    public async Task<Guid?> IssueAsync(IssueTravelIdentityRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<IssueTravelIdentityRequest, IssueTravelIdentityResponse>(
            BaseUrl, 
            request, 
            cancellationToken);
        
        return response?.TravelIdentityId;
    }

    public async Task<bool> RenewAsync(Guid tenantId, Guid id, int validityYears = 8, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/renew?tenantId={tenantId}", new RenewTravelIdentityRequest { ValidityYears = validityYears }, cancellationToken);
    }

    public async Task<bool> RevokeAsync(Guid tenantId, Guid id, string reason, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/revoke?tenantId={tenantId}", new RevokeTravelIdentityRequest { Reason = reason }, cancellationToken);
    }

    public async Task<TravelIdentityValidationResult?> ValidateAsync(string idNumber, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<TravelIdentityValidationResult>($"{BaseUrl}/validate/{Uri.EscapeDataString(idNumber)}", cancellationToken);
    }

    private class IssueTravelIdentityResponse
    {
        public Guid TravelIdentityId { get; set; }
    }
}
