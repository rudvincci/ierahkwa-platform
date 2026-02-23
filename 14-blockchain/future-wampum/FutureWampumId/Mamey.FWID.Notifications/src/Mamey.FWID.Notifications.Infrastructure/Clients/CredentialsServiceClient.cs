using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Mamey.FWID.Credentials.Api.Protos;
using Mamey.FWID.Credentials.GrpcClient.Services;
using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.DTO;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Infrastructure.Clients;

/// <summary>
/// Service client for calling the Credentials service via gRPC with certificate authentication.
/// </summary>
internal sealed class CredentialsServiceClient : ICredentialsServiceClient
{
    private readonly string _credentialsServiceAddress;
    private readonly ILogger<CredentialsServiceClient> _logger;
    private readonly CredentialServiceClient _client;

    public CredentialsServiceClient(
        HttpClientOptions options,
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions,
        ILogger<CredentialsServiceClient> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _credentialsServiceAddress = options.Services["credentials"];

        var certificate = LoadCertificate(certificatesService, vaultOptions, securityOptions);
        _client = CreateClient(certificate);
    }

    private CredentialServiceClient CreateClient(X509Certificate2? certificate)
    {
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        if (certificate != null)
        {
            httpClientHandler.ClientCertificates.Add(certificate);
        }

        var httpClient = new HttpClient(httpClientHandler);
        var channel = GrpcChannel.ForAddress(_credentialsServiceAddress, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        var grpcClient = new CredentialService.CredentialServiceClient(channel);
        return new CredentialServiceClient(grpcClient, _logger);
    }

    private X509Certificate2? LoadCertificate(
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions)
    {
        if (!vaultOptions.Enabled || vaultOptions.Pki?.Enabled != true)
        {
            return null;
        }

        var certificate = certificatesService.Get(vaultOptions.Pki.RoleName);
        return certificate;
    }

    public async Task<CredentialDto?> GetCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetCredentialAsync(
                new GetCredentialRequest { CredentialId = credentialId.ToString() },
                cancellationToken: cancellationToken);

            if (response == null || string.IsNullOrEmpty(response.CredentialId))
            {
                return null;
            }

            return new CredentialDto
            {
                CredentialId = Guid.Parse(response.CredentialId),
                IdentityId = Guid.Parse(response.IdentityId),
                CredentialType = response.CredentialType,
                IssuedAt = response.IssuedAt.ToDateTime()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credential: {CredentialId}", credentialId);
            return null;
        }
    }
}







