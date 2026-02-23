using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Mamey.FWID.AccessControls.Api.Protos;
using Mamey.FWID.AccessControls.GrpcClient.Services;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.DTO;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;

namespace Mamey.FWID.Identities.Infrastructure.Clients;

/// <summary>
/// Service client for calling the AccessControls service via gRPC with certificate authentication.
/// </summary>
internal sealed class AccessControlsServiceClient : IAccessControlsServiceClient
{
    private readonly string _accessControlsServiceAddress;
    private readonly ILogger<AccessControlsServiceClient> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly AccessControlServiceClient _client;

    public AccessControlsServiceClient(
        HttpClientOptions options,
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions,
        ILogger<AccessControlsServiceClient> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _accessControlsServiceAddress = options.Services["accesscontrols"];

        var certificate = LoadCertificate(certificatesService, vaultOptions, securityOptions);
        _client = CreateClient(certificate);
    }

    private AccessControlServiceClient CreateClient(X509Certificate2? certificate)
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
        var channel = GrpcChannel.ForAddress(_accessControlsServiceAddress, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        var grpcClient = new AccessControlService.AccessControlServiceClient(channel);
        var grpcLogger = _loggerFactory.CreateLogger<Mamey.FWID.AccessControls.GrpcClient.Services.AccessControlServiceClient>();
        return new AccessControlServiceClient(grpcClient, grpcLogger);
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

    public async Task<bool> CheckZoneAccessAsync(Guid identityId, Guid zoneId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.CheckZoneAccessAsync(
                new CheckZoneAccessRequest
                {
                    IdentityId = identityId.ToString(),
                    ZoneId = zoneId.ToString(),
                    RequiredPermission = "read" // Default permission
                },
                cancellationToken: cancellationToken);

            return response?.HasAccess ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking zone access for identity: {IdentityId}, zone: {ZoneId}", identityId, zoneId);
            return false;
        }
    }

    public async Task<IReadOnlyList<AccessControlDto>> GetAccessControlsByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetZoneAccessAsync(
                new GetZoneAccessRequest
                {
                    IdentityId = identityId.ToString(),
                    ZoneId = string.Empty // Get all zones
                },
                cancellationToken: cancellationToken);

            if (response == null)
            {
                return Array.Empty<AccessControlDto>();
            }

            // Note: Adjust based on actual AccessControls service response structure
            return new List<AccessControlDto>
            {
                new AccessControlDto
                {
                    AccessControlId = Guid.Parse(response.AccessControlId),
                    IdentityId = identityId,
                    ZoneId = Guid.Parse(response.ZoneId),
                    Permission = response.Permission,
                    GrantedAt = DateTime.UtcNow // Adjust based on actual response
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting access controls for identity: {IdentityId}", identityId);
            return Array.Empty<AccessControlDto>();
        }
    }
}
