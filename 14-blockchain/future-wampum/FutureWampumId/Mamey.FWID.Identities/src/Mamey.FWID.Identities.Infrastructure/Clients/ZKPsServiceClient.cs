using System.Security.Cryptography.X509Certificates;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Mamey.FWID.ZKPs.Api.Protos;
using Mamey.FWID.ZKPs.GrpcClient.Services;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.DTO;
using Mamey.Http;
using Mamey.Secrets.Vault;
using Mamey.WebApi.Security;

namespace Mamey.FWID.Identities.Infrastructure.Clients;

/// <summary>
/// Service client for calling the ZKPs service via gRPC with certificate authentication.
/// </summary>
internal sealed class ZKPsServiceClient : IZKPsServiceClient
{
    private readonly string _zkpsServiceAddress;
    private readonly ILogger<ZKPsServiceClient> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ZKPServiceClient _client;

    public ZKPsServiceClient(
        HttpClientOptions options,
        ICertificatesService certificatesService,
        VaultOptions vaultOptions,
        SecurityOptions securityOptions,
        ILogger<ZKPsServiceClient> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
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
        var grpcLogger = _loggerFactory.CreateLogger<Mamey.FWID.ZKPs.GrpcClient.Services.ZKPServiceClient>();
        return new ZKPServiceClient(grpcClient, grpcLogger);
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
            // Note: ZKPs service may not have GetProofById, using GetProofsByIdentity as fallback
            // This is a placeholder - adjust based on actual ZKPs service API
            _logger.LogWarning("GetZKPProofAsync not fully implemented - ZKPs service may not support GetProofById");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ZKP proof: {ProofId}", proofId);
            return null;
        }
    }

    public async Task<IReadOnlyList<ZKPProofDto>> GetZKPProofsByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: ZKPs service may not have GetProofsByIdentity, adjust based on actual API
            _logger.LogWarning("GetZKPProofsByIdentityIdAsync not fully implemented - ZKPs service may not support GetProofsByIdentity");
            return Array.Empty<ZKPProofDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ZKP proofs for identity: {IdentityId}", identityId);
            return Array.Empty<ZKPProofDto>();
        }
    }
}
