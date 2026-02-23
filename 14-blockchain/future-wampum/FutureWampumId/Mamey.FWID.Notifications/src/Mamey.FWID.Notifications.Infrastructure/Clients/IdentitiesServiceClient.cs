using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.DTO;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;

namespace Mamey.FWID.Notifications.Infrastructure.Clients;

/// <summary>
/// Service client for calling the Identities service via HTTP with certificate authentication.
/// </summary>
internal sealed class IdentitiesServiceClient : IIdentitiesServiceClient
{
    private readonly IHttpClient _httpClient;
    private readonly string _url;

    public IdentitiesServiceClient(
        IHttpClient httpClient,
        HttpClientOptions options,
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions)
    {
        _httpClient = httpClient;
        _url = options.Services["identities"];

        if (!vaultOptions.Enabled || vaultOptions.Pki?.Enabled != true)
        {
            return;
        }

        var certificate = certificatesService.Get(vaultOptions.Pki.RoleName);
        if (certificate is null)
        {
            return;
        }

        var header = securityOptions.Certificate.GetHeaderName();
        var certificateData = certificate.GetRawCertDataString();
        _httpClient.SetHeaders(h => h.Add(header, certificateData));
    }

    public async Task<IdentityDto?> GetIdentityAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetApiResponseAsync<IdentityDto>($"{_url}/api/identities/{identityId}");
        return response.Value;
    }
}







