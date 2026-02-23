using Mamey.Government.UI.Models;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Certificates service implementation.
/// </summary>
public class CertificatesService : ApiServiceBase, ICertificatesService
{
    private const string BaseUrl = "/api/certificates";

    public CertificatesService(HttpClient httpClient, ILogger<CertificatesService> logger, AppOptions appOptions) 
        : base(httpClient, logger, appOptions)
    {
    }

    public async Task<PagedResult<CertificateSummaryModel>> BrowseAsync(
        Guid tenantId,
        string? certificateType = null,
        string? status = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQueryString(
            ("tenantId", tenantId),
            ("certificateType", certificateType),
            ("status", status),
            ("searchTerm", searchTerm),
            ("page", page),
            ("pageSize", pageSize));
        
        var result = await base.GetAsync<PagedResult<CertificateSummaryModel>>(
            $"{BaseUrl}?{query}", 
            cancellationToken);
        
        return result ?? PagedResult<CertificateSummaryModel>.Empty;
    }

    public async Task<CertificateModel?> GetAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<CertificateModel>($"{BaseUrl}/{id}?tenantId={tenantId}", cancellationToken);
    }

    public async Task<CertificateModel?> GetByNumberAsync(Guid tenantId, string certificateNumber, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<CertificateModel>($"{BaseUrl}/by-number/{Uri.EscapeDataString(certificateNumber)}?tenantId={tenantId}", cancellationToken);
    }

    public async Task<IReadOnlyList<CertificateSummaryModel>> GetByCitizenAsync(Guid citizenId, CancellationToken cancellationToken = default)
    {
        var result = await base.GetAsync<List<CertificateSummaryModel>>($"{BaseUrl}/citizen/{citizenId}", cancellationToken);
        return result ?? new List<CertificateSummaryModel>();
    }

    public async Task<Guid?> IssueAsync(IssueCertificateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await PostAsync<IssueCertificateRequest, IssueCertificateResponse>(
            BaseUrl, 
            request, 
            cancellationToken);
        
        return response?.CertificateId;
    }

    public async Task<bool> RevokeAsync(Guid tenantId, Guid id, string reason, CancellationToken cancellationToken = default)
    {
        return await PostAsync($"{BaseUrl}/{id}/revoke?tenantId={tenantId}", new RevokeCertificateRequest { Reason = reason }, cancellationToken);
    }

    public async Task<CertificateValidationResult?> ValidateAsync(string certificateNumber, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<CertificateValidationResult>($"{BaseUrl}/validate/{Uri.EscapeDataString(certificateNumber)}", cancellationToken);
    }

    private class IssueCertificateResponse
    {
        public Guid CertificateId { get; set; }
    }
}
