using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Mamey.FWID.Credentials.Api.Protos;
using Mamey.FWID.Credentials.GrpcClient.Services;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.DTO;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;

namespace Mamey.FWID.Identities.Infrastructure.Clients;

/// <summary>
/// Service client for calling the Credentials service via gRPC with certificate authentication.
/// </summary>
internal sealed class CredentialsServiceClient : ICredentialsServiceClient
{
    private readonly string _credentialsServiceAddress;
    private readonly ILogger<CredentialsServiceClient> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly CredentialServiceClient _client;

    public CredentialsServiceClient(
        HttpClientOptions options,
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions,
        ILogger<CredentialsServiceClient> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
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
        var grpcLogger = _loggerFactory.CreateLogger<Mamey.FWID.Credentials.GrpcClient.Services.CredentialServiceClient>();
        return new CredentialServiceClient(grpcClient, grpcLogger);
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
                IdentityId = new IdentityId(Guid.Parse(response.IdentityId)),
                CredentialType = response.CredentialType,
                Claims = new Dictionary<string, object>(), // Claims not available in GetCredentialResponse
                IssuerId = Guid.Parse(response.IssuerId),
                Status = response.Status,
                IssuedAt = response.IssuedAt.GetDate(),
                ExpiresAt = response.ExpiresAt.GetDate()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credential: {CredentialId}", credentialId);
            return null;
        }
    }

    public async Task<IReadOnlyList<CredentialDto>> GetCredentialsByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetCredentialsByIdentityAsync(
                new GetCredentialsByIdentityRequest { IdentityId = identityId.ToString() },
                cancellationToken: cancellationToken);

            if (response == null || response.Credentials == null)
            {
                return Array.Empty<CredentialDto>();
            }

            return response.Credentials.Select(c => new CredentialDto
            {
                CredentialId = Guid.Parse(c.CredentialId),
                IdentityId = new IdentityId(identityId), // Use the identityId parameter since it's not in CredentialInfo
                CredentialType = c.CredentialType,
                Claims = new Dictionary<string, object>(), // Claims not available in CredentialInfo
                IssuerId = Guid.Parse(c.IssuerId),
                Status = c.Status,
                IssuedAt = c.IssuedAt.GetDate(),
                ExpiresAt = c.ExpiresAt.GetDate()
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credentials for identity: {IdentityId}", identityId);
            return Array.Empty<CredentialDto>();
        }
    }
}
