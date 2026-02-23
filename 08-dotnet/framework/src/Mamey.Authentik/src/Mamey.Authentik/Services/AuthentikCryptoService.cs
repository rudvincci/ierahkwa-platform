using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service implementation for Authentik Crypto API operations.
/// </summary>
public class AuthentikCryptoService : IAuthentikCryptoService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikCryptoService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikCryptoService"/> class.
    /// </summary>
    public AuthentikCryptoService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikCryptoService> logger,
        IAuthentikCache? cache = null)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Gets the HTTP client for making requests.
    /// </summary>
    protected HttpClient GetHttpClient() => _httpClientFactory.CreateClient("Authentik");

    /// <summary>
    /// GET /crypto/certificatekeypairs/
    /// </summary>
    public async Task<PaginatedResult<object>> CertificatekeypairsListAsync(bool? has_key = null, bool? include_details = null, string? managed = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_50f30757 = $"api/v3/crypto/certificatekeypairs/";
        var queryParams = new List<string>();
        if (has_key.HasValue) queryParams.Add($"has_key={has_key.Value.ToString().ToLower()}");
        if (include_details.HasValue) queryParams.Add($"include_details={include_details.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (queryParams.Any()) url_50f30757 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_50f30757, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_50f30757 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_50f30757 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /crypto/certificatekeypairs/
    /// </summary>
    public async Task<object?> CertificatekeypairsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d997e09b = $"api/v3/crypto/certificatekeypairs/";
        var response = await client.PostAsJsonAsync(url_d997e09b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d997e09b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d997e09b;
    }

    /// <summary>
    /// GET /crypto/certificatekeypairs/{kp_uuid}/
    /// </summary>
    public async Task<object?> CertificatekeypairsRetrieveAsync(string kp_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8a9f1abd = $"api/v3/crypto/certificatekeypairs/{kp_uuid}/";
        var response = await client.GetAsync(url_8a9f1abd, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8a9f1abd = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8a9f1abd;
    }

    /// <summary>
    /// PUT /crypto/certificatekeypairs/{kp_uuid}/
    /// </summary>
    public async Task<object?> CertificatekeypairsUpdateAsync(string kp_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2b050072 = $"api/v3/crypto/certificatekeypairs/{kp_uuid}/";
        var response = await client.PutAsJsonAsync(url_2b050072, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2b050072 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2b050072;
    }

    /// <summary>
    /// PATCH /crypto/certificatekeypairs/{kp_uuid}/
    /// </summary>
    public async Task<object?> CertificatekeypairsPartialUpdateAsync(string kp_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6882eedc = $"api/v3/crypto/certificatekeypairs/{kp_uuid}/";
        var response = await client.PatchAsJsonAsync(url_6882eedc, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6882eedc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6882eedc;
    }

    /// <summary>
    /// DELETE /crypto/certificatekeypairs/{kp_uuid}/
    /// </summary>
    public async Task<object?> CertificatekeypairsDestroyAsync(string kp_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_28f3d8e2 = $"api/v3/crypto/certificatekeypairs/{kp_uuid}/";
        var response = await client.DeleteAsync(url_28f3d8e2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_28f3d8e2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_28f3d8e2;
    }

    /// <summary>
    /// GET /crypto/certificatekeypairs/{kp_uuid}/used_by/
    /// </summary>
    public async Task<object?> CertificatekeypairsUsedByListAsync(string kp_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fb906fa0 = $"api/v3/crypto/certificatekeypairs/{kp_uuid}/used_by/";
        var response = await client.GetAsync(url_fb906fa0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fb906fa0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fb906fa0;
    }

    /// <summary>
    /// GET /crypto/certificatekeypairs/{kp_uuid}/view_certificate/
    /// </summary>
    public async Task<object?> CertificatekeypairsViewCertificateRetrieveAsync(string kp_uuid, bool? download = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4f17616f = $"api/v3/crypto/certificatekeypairs/{kp_uuid}/view_certificate/";
        var queryParams = new List<string>();
        if (download.HasValue) queryParams.Add($"download={download.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_4f17616f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_4f17616f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4f17616f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4f17616f;
    }

    /// <summary>
    /// GET /crypto/certificatekeypairs/{kp_uuid}/view_private_key/
    /// </summary>
    public async Task<object?> CertificatekeypairsViewPrivateKeyRetrieveAsync(string kp_uuid, bool? download = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_af599759 = $"api/v3/crypto/certificatekeypairs/{kp_uuid}/view_private_key/";
        var queryParams = new List<string>();
        if (download.HasValue) queryParams.Add($"download={download.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_af599759 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_af599759, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_af599759 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_af599759;
    }

    /// <summary>
    /// POST /crypto/certificatekeypairs/generate/
    /// </summary>
    public async Task<object?> CertificatekeypairsGenerateCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8d1cad53 = $"api/v3/crypto/certificatekeypairs/generate/";
        var response = await client.PostAsJsonAsync(url_8d1cad53, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8d1cad53 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8d1cad53;
    }

}
