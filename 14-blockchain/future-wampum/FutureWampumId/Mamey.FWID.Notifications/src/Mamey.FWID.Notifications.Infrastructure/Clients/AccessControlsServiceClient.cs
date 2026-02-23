using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Mamey.FWID.AccessControls.Api.Protos;
using Mamey.FWID.AccessControls.GrpcClient.Services;
using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.DTO;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Infrastructure.Clients;

/// <summary>
/// Service client for calling the AccessControls service via gRPC with certificate authentication.
/// </summary>
internal sealed class AccessControlsServiceClient : IAccessControlsServiceClient
{
    private readonly string _accessControlsServiceAddress;
    private readonly ILogger<AccessControlsServiceClient> _logger;
    private readonly AccessControlServiceClient _client;

    public AccessControlsServiceClient(
        HttpClientOptions options,
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions,
        ILogger<AccessControlsServiceClient> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        return new AccessControlServiceClient(grpcClient, _logger);
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

    public async Task<AccessControlDto?> GetAccessControlAsync(Guid accessControlId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: AccessControls service may not have GetAccessControl method yet
            // This is a placeholder implementation
            _logger.LogWarning("GetAccessControlAsync not yet implemented in AccessControls service. AccessControlId: {AccessControlId}", accessControlId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting access control: {AccessControlId}", accessControlId);
            return null;
        }
    }
}







