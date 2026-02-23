using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Mamey.FWID.DIDs.Api.Protos;
using Mamey.FWID.DIDs.GrpcClient.Services;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.DTO;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;
using DIDServiceClient = Mamey.FWID.DIDs.GrpcClient.Services.DIDServiceClient;

namespace Mamey.FWID.Identities.Infrastructure.Clients;

/// <summary>
/// Service client for calling the DIDs service via gRPC with certificate authentication.
/// </summary>
internal sealed class DIDsServiceClient : IDIDsServiceClient
{
    private readonly string _didsServiceAddress;
    private readonly ILogger<DIDsServiceClient> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly DIDServiceClient _client;

    public DIDsServiceClient(
        HttpClientOptions options,
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions,
        ILogger<DIDsServiceClient> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _didsServiceAddress = options.Services["dids"];

        var certificate = LoadCertificate(certificatesService, vaultOptions, securityOptions);
        _client = CreateClient(certificate);
    }

    private DIDServiceClient CreateClient(X509Certificate2? certificate)
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
        var channel = GrpcChannel.ForAddress(_didsServiceAddress, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        var grpcClient = new DIDService.DIDServiceClient(channel);
        var grpcLogger = _loggerFactory.CreateLogger<Mamey.FWID.DIDs.GrpcClient.Services.DIDServiceClient>();
        return new DIDServiceClient(grpcClient, grpcLogger);
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

    public async Task<DIDDto?> GetDIDAsync(Guid didId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: DIDs service doesn't have GetDIDById, only GetDIDByIdentity
            // This method requires the IdentityId, not the DIDId
            // For now, we'll return null and log a warning
            _logger.LogWarning("GetDIDAsync requires IdentityId, not DIDId. Use GetDIDByIdentityIdAsync instead.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID: {DIDId}", didId);
            return null;
        }
    }

    public async Task<DIDDto?> GetDIDByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetDIDByIdentityAsync(
                new GetDIDByIdentityRequest { IdentityId = identityId.ToString() },
                cancellationToken: cancellationToken);

            if (response == null || string.IsNullOrEmpty(response.DidId))
            {
                return null;
            }

            return new DIDDto
            {
                DIDId = Guid.Parse(response.DidId),
                IdentityId = Guid.Parse(response.IdentityId),
                DidString = response.DidString,
                DIDMethod = response.DidMethod,
                DocumentSignature = null, // Not available in proto response
                CreatedAt = response.CreatedAt.GetDate(),
                UpdatedAt = null // Not available in proto response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID for identity: {IdentityId}", identityId);
            return null;
        }
    }
}

