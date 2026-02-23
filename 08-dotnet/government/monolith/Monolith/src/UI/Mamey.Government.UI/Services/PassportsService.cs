using Mamey.Government.UI.Models;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Passports service implementation.
/// </summary>
public class PassportsService : ApiServiceBase, IPassportsService
{
    private const string BaseUrl = "/api/passports";

    public PassportsService(HttpClient httpClient, ILogger<PassportsService> logger, AppOptions appOptions) 
        : base(httpClient, logger, appOptions)
    {
    }

    public async Task<PagedResult<PassportSummaryModel>> BrowseAsync(
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
        
        var result = await base.GetAsync<PagedResult<PassportSummaryModel>>(
            $"{BaseUrl}?{query}", 
            cancellationToken);
        
        return result ?? PagedResult<PassportSummaryModel>.Empty;
    }

    public async Task<PassportModel?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<PassportModel>($"{BaseUrl}/{id}?tenantId={tenantId}", cancellationToken);
    }

    public async Task<PassportModel?> GetByNumberAsync(Guid tenantId, string passportNumber, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<PassportModel>($"{BaseUrl}/by-number/{Uri.EscapeDataString(passportNumber)}?tenantId={tenantId}", cancellationToken);
    }

    public async Task<IReadOnlyList<PassportSummaryModel>> GetByCitizenAsync(Guid citizenId, CancellationToken cancellationToken = default)
    {
        var result = await base.GetAsync<List<PassportSummaryModel>>($"{BaseUrl}/citizen/{citizenId}", cancellationToken);
        return result ?? new List<PassportSummaryModel>();
    }

    public async Task<Guid?> IssueAsync(IssuePassportRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<IssuePassportRequest, IssuePassportResponse>(
            BaseUrl, 
            request, 
            cancellationToken);
        
        return response?.PassportId;
    }

    public async Task<bool> RenewAsync(Guid tenantId, Guid id, int validityYears = 10, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/renew?tenantId={tenantId}", new RenewPassportRequest { ValidityYears = validityYears }, cancellationToken);
    }

    public async Task<bool> RevokeAsync(Guid tenantId, Guid id, string reason, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/revoke?tenantId={tenantId}", new RevokePassportRequest { Reason = reason }, cancellationToken);
    }

    public async Task<PassportValidationResult?> ValidateAsync(string passportNumber, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<PassportValidationResult>($"{BaseUrl}/validate/{Uri.EscapeDataString(passportNumber)}", cancellationToken);
    }

    private class IssuePassportResponse
    {
        public Guid PassportId { get; set; }
    }
}
