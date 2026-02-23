using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.DTO;
using Mamey.FWID.ZKPs.Api.Protos;
using Mamey.FWID.ZKPs.GrpcClient.Services;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Infrastructure.Clients;

/// <summary>
/// Service client for calling the ZKPs service via gRPC with certificate authentication.
/// </summary>
internal sealed class ZKPsServiceClient : IZKPsServiceClient
{
    private readonly string _zkpsServiceAddress;
    private readonly ILogger<ZKPsServiceClient> _logger;
    private readonly ZKPServiceClient _client;

    public ZKPsServiceClient(
        HttpClientOptions options,
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions,
        ILogger<ZKPsServiceClient> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _zkpsServiceAddress = options.Services["zkps"];

        var certificate = LoadCertificate(certificatesService, vaultOptions, securityOptions);
        _client = CreateClient(certificate);
    }

    private ZKPServiceClient CreateClient(X509Certificate2? certificate)
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
        var channel = GrpcChannel.ForAddress(_zkpsServiceAddress, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        var grpcClient = new ZKPService.ZKPServiceClient(channel);
        return new ZKPServiceClient(grpcClient, _logger);
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

    public async Task<ZKPProofDto?> GetZKPProofAsync(Guid proofId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: ZKPs service may not have GetZKPProof method yet
            // This is a placeholder implementation
            _logger.LogWarning("GetZKPProofAsync not yet implemented in ZKPs service. ProofId: {ProofId}", proofId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ZKP proof: {ProofId}", proofId);
            return null;
        }
    }
}







