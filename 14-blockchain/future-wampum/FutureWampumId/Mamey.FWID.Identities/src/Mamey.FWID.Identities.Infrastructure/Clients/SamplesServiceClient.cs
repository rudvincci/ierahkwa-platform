using System;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.Types;

namespace Mamey.FWID.Identities.Infrastructure.Clients
{
    internal sealed class SamplesServiceClient :  ISamplesServiceClient
    {
        private readonly IHttpClient _httpClient;
        private readonly string _url;

        public SamplesServiceClient(IHttpClient httpClient, HttpClientOptions options
            ,ICertificatesService certificatesService, VaultOptions vaultOptions, SecurityOptions securityOptions)
        {
            _httpClient = httpClient;
            _url = options.Services["samples"];

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
            return;
        }

        public async Task<SampleDto?> GetAsync(Guid id)
        {
            var response = await _httpClient.GetApiResponseAsync<SampleDto>($"{_url}/samples/{id}");
            return response.Value;
        }
    }
}

