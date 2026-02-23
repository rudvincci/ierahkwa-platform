using Grpc.Net.Client;
using Mamey.DID;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.Blockchain.DID;

/// <summary>
/// Client implementation for DID operations on MameyNode blockchain.
/// Implements W3C DID Core specification for did:futurewampum method.
/// </summary>
public class DIDClient : IDIDClient, IDisposable
{
    private readonly Mamey.DID.DIDService.DIDServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly DIDClientOptions _options;
    private readonly ILogger<DIDClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the DIDClient.
    /// </summary>
    public DIDClient(IOptions<DIDClientOptions> options, ILogger<DIDClient>? logger = null)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        _channel = GrpcChannel.ForAddress(
            _options.NodeUrl,
            new GrpcChannelOptions
            {
                HttpHandler = CreateHttpHandler(_options.Tls, _logger)
            });

        _client = new Mamey.DID.DIDService.DIDServiceClient(_channel);

        _logger?.LogInformation(
            "Initialized {Client} connecting to {Url}",
            nameof(DIDClient), _options.NodeUrl);
    }

    private static SocketsHttpHandler CreateHttpHandler(GrpcTlsOptions? tls, ILogger? logger)
    {
        var handler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };

        if (tls is null)
        {
            return handler;
        }

        handler.SslOptions = new SslClientAuthenticationOptions();

        if (!string.IsNullOrWhiteSpace(tls.ClientCertificatePath))
        {
            if (string.IsNullOrWhiteSpace(tls.ClientKeyPath))
            {
                throw new InvalidOperationException("TLS ClientKeyPath is required when ClientCertificatePath is set.");
            }

            var clientCert = X509Certificate2.CreateFromPemFile(tls.ClientCertificatePath, tls.ClientKeyPath);
            clientCert = new X509Certificate2(clientCert.Export(X509ContentType.Pkcs12));
            handler.SslOptions.ClientCertificates = new X509CertificateCollection { clientCert };
            logger?.LogInformation("Configured gRPC mTLS client certificate from {Path}", tls.ClientCertificatePath);
        }

        if (tls.SkipServerCertificateValidation)
        {
            handler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            logger?.LogWarning("TLS server certificate validation is DISABLED (dev-only).");
            return handler;
        }

        if (!string.IsNullOrWhiteSpace(tls.CaCertificatePath))
        {
            var ca = new X509Certificate2(tls.CaCertificatePath);
            handler.SslOptions.RemoteCertificateValidationCallback = (_, certificate, _, _) =>
            {
                if (certificate is null)
                {
                    return false;
                }

                var serverCert = certificate as X509Certificate2 ?? new X509Certificate2(certificate);
                using var chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                chain.ChainPolicy.CustomTrustStore.Add(ca);
                return chain.Build(serverCert);
            };

            logger?.LogInformation("Configured gRPC server CA trust from {Path}", tls.CaCertificatePath);
        }

        return handler;
    }

    /// <inheritdoc />
    public async Task<IssueDIDResult> IssueDIDAsync(
        string controller,
        IReadOnlyList<DIDVerificationMethod>? verificationMethods = null,
        IReadOnlyList<DIDService>? services = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Issuing new DID for controller: {Controller}", controller);

        try
        {
            var request = new IssueDIDRequest
            {
                Controller = controller,
                Method = _options.DefaultMethod
            };

            if (verificationMethods != null)
            {
                foreach (var vm in verificationMethods)
                {
                    request.VerificationMethods.Add(new VerificationMethod
                    {
                        Id = vm.Id,
                        Type = vm.Type,
                        Controller = vm.Controller,
                        PublicKeyMultibase = vm.PublicKeyMultibase ?? "",
                        PublicKeyJwk = vm.PublicKeyJwk ?? ""
                    });
                }
            }

            if (services != null)
            {
                foreach (var svc in services)
                {
                    request.Services.Add(new Service
                    {
                        Id = svc.Id,
                        Type = svc.Type,
                        ServiceEndpoint = svc.ServiceEndpoint
                    });
                }
            }

            if (metadata != null)
            {
                foreach (var (key, value) in metadata)
                {
                    request.Metadata.Add(key, value);
                }
            }

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.IssueDIDAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            if (response.Success)
            {
                _logger?.LogInformation("Successfully issued DID: {DID}", response.Did);
            }
            else
            {
                _logger?.LogWarning("Failed to issue DID: {Error}", response.ErrorMessage);
            }

            return new IssueDIDResult(
                Success: response.Success,
                DID: response.Did,
                DIDDocument: response.DidDocument,
                TransactionHash: response.TransactionHash,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error issuing DID for controller: {Controller}", controller);
            return new IssueDIDResult(false, null, null, null, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<ResolveDIDResult> ResolveDIDAsync(
        string did,
        ulong version = 0,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Resolving DID: {DID}", did);

        try
        {
            var request = new ResolveDIDRequest
            {
                Did = did,
                IncludeMetadata = _options.IncludeMetadataInResolution,
                Version = version
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.ResolveDIDAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            DIDDocumentMetadata? docMetadata = null;
            if (response.Metadata != null)
            {
                docMetadata = new DIDDocumentMetadata(
                    Created: response.Metadata.Created > 0 
                        ? DateTimeOffset.FromUnixTimeSeconds((long)response.Metadata.Created).UtcDateTime 
                        : null,
                    Updated: response.Metadata.Updated > 0 
                        ? DateTimeOffset.FromUnixTimeSeconds((long)response.Metadata.Updated).UtcDateTime 
                        : null,
                    VersionId: response.Metadata.VersionId,
                    Deactivated: response.Metadata.Deactivated,
                    CanonicalId: string.IsNullOrEmpty(response.Metadata.CanonicalId) ? null : response.Metadata.CanonicalId);
            }

            DIDResolutionMetadata? resMetadata = null;
            if (response.ResolutionMetadata != null)
            {
                resMetadata = new DIDResolutionMetadata(
                    ContentType: response.ResolutionMetadata.ContentType,
                    Error: string.IsNullOrEmpty(response.ResolutionMetadata.Error) ? null : response.ResolutionMetadata.Error,
                    ErrorMessage: string.IsNullOrEmpty(response.ResolutionMetadata.ErrorMessage) ? null : response.ResolutionMetadata.ErrorMessage,
                    Duration: (long)response.ResolutionMetadata.Duration);
            }

            return new ResolveDIDResult(
                Success: response.Success,
                DIDDocument: response.DidDocument,
                Metadata: docMetadata,
                ResolutionMetadata: resMetadata,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error resolving DID: {DID}", did);
            return new ResolveDIDResult(false, null, null, null, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<UpdateDIDResult> UpdateDIDDocumentAsync(
        string did,
        string didDocument,
        string proof,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Updating DID Document: {DID}", did);

        try
        {
            var request = new UpdateDIDDocumentRequest
            {
                Did = did,
                DidDocument = didDocument,
                Proof = proof,
                Reason = reason ?? ""
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.UpdateDIDDocumentAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            if (response.Success)
            {
                _logger?.LogInformation("Successfully updated DID Document: {DID}, Version: {Version}", did, response.NewVersion);
            }
            else
            {
                _logger?.LogWarning("Failed to update DID Document: {Error}", response.ErrorMessage);
            }

            return new UpdateDIDResult(
                Success: response.Success,
                TransactionHash: response.TransactionHash,
                NewVersion: response.NewVersion,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating DID Document: {DID}", did);
            return new UpdateDIDResult(false, null, null, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<RevokeDIDResult> RevokeDIDAsync(
        string did,
        string proof,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Revoking DID: {DID}", did);

        try
        {
            var request = new RevokeDIDRequest
            {
                Did = did,
                Proof = proof,
                Reason = reason ?? ""
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.RevokeDIDAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            if (response.Success)
            {
                _logger?.LogInformation("Successfully revoked DID: {DID}", did);
            }
            else
            {
                _logger?.LogWarning("Failed to revoke DID: {Error}", response.ErrorMessage);
            }

            return new RevokeDIDResult(
                Success: response.Success,
                TransactionHash: response.TransactionHash,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error revoking DID: {DID}", did);
            return new RevokeDIDResult(false, null, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<DIDHistoryResult> GetDIDHistoryAsync(
        string did,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting DID history: {DID}", did);

        try
        {
            var request = new GetDIDHistoryRequest
            {
                Did = did,
                Limit = (uint)limit,
                Offset = (uint)offset
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.GetDIDHistoryAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            if (response.Success)
            {
                var entries = response.Entries.Select(e => new DIDHistoryEntry(
                    Version: e.Version,
                    DIDDocument: e.DidDocument,
                    Action: e.Action,
                    Actor: e.Actor,
                    Timestamp: DateTimeOffset.FromUnixTimeSeconds((long)e.Timestamp).UtcDateTime,
                    TransactionHash: e.TransactionHash,
                    Reason: string.IsNullOrEmpty(e.Reason) ? null : e.Reason)).ToList();

                return new DIDHistoryResult(
                    Success: true,
                    Entries: entries,
                    TotalCount: (int)response.TotalCount,
                    ErrorMessage: null);
            }

            return new DIDHistoryResult(
                Success: false,
                Entries: Array.Empty<DIDHistoryEntry>(),
                TotalCount: 0,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting DID history: {DID}", did);
            return new DIDHistoryResult(false, Array.Empty<DIDHistoryEntry>(), 0, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<VerifyOwnershipResult> VerifyDIDOwnershipAsync(
        string did,
        string challenge,
        string signature,
        string? verificationMethodId = null,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Verifying DID ownership: {DID}", did);

        try
        {
            var request = new VerifyDIDOwnershipRequest
            {
                Did = did,
                Challenge = challenge,
                Signature = signature,
                VerificationMethodId = verificationMethodId ?? ""
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.VerifyDIDOwnershipAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            return new VerifyOwnershipResult(
                Success: response.Success,
                Verified: response.Verified,
                VerificationMethodUsed: string.IsNullOrEmpty(response.VerificationMethodUsed) ? null : response.VerificationMethodUsed,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error verifying DID ownership: {DID}", did);
            return new VerifyOwnershipResult(false, false, null, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<ListDIDsResult> ListDIDsAsync(
        string controller,
        bool includeDeactivated = false,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Listing DIDs for controller: {Controller}", controller);

        try
        {
            var request = new ListDIDsRequest
            {
                Controller = controller,
                IncludeDeactivated = includeDeactivated,
                Limit = (uint)limit,
                Offset = (uint)offset
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.ListDIDsAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            if (response.Success)
            {
                var dids = response.Dids.Select(d => new DIDInfo(
                    DID: d.Did,
                    Status: (DIDStatus)d.Status,
                    Created: DateTimeOffset.FromUnixTimeSeconds((long)d.Created).UtcDateTime,
                    Updated: DateTimeOffset.FromUnixTimeSeconds((long)d.Updated).UtcDateTime,
                    Version: d.Version)).ToList();

                return new ListDIDsResult(
                    Success: true,
                    DIDs: dids,
                    TotalCount: (int)response.TotalCount,
                    ErrorMessage: null);
            }

            return new ListDIDsResult(
                Success: false,
                DIDs: Array.Empty<DIDInfo>(),
                TotalCount: 0,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error listing DIDs for controller: {Controller}", controller);
            return new ListDIDsResult(false, Array.Empty<DIDInfo>(), 0, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<ModifyDIDResult> AddVerificationMethodAsync(
        string did,
        DIDVerificationMethod verificationMethod,
        string proof,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Adding verification method to DID: {DID}", did);

        try
        {
            var request = new AddVerificationMethodRequest
            {
                Did = did,
                VerificationMethod = new VerificationMethod
                {
                    Id = verificationMethod.Id,
                    Type = verificationMethod.Type,
                    Controller = verificationMethod.Controller,
                    PublicKeyMultibase = verificationMethod.PublicKeyMultibase ?? "",
                    PublicKeyJwk = verificationMethod.PublicKeyJwk ?? ""
                },
                Proof = proof
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.AddVerificationMethodAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            return new ModifyDIDResult(
                Success: response.Success,
                TransactionHash: response.TransactionHash,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding verification method to DID: {DID}", did);
            return new ModifyDIDResult(false, null, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<ModifyDIDResult> RemoveVerificationMethodAsync(
        string did,
        string verificationMethodId,
        string proof,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Removing verification method from DID: {DID}, Method: {MethodId}", did, verificationMethodId);

        try
        {
            var request = new RemoveVerificationMethodRequest
            {
                Did = did,
                VerificationMethodId = verificationMethodId,
                Proof = proof
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.RemoveVerificationMethodAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            return new ModifyDIDResult(
                Success: response.Success,
                TransactionHash: response.TransactionHash,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error removing verification method from DID: {DID}", did);
            return new ModifyDIDResult(false, null, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<ModifyDIDResult> AddServiceAsync(
        string did,
        DIDService service,
        string proof,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Adding service to DID: {DID}", did);

        try
        {
            var request = new AddServiceRequest
            {
                Did = did,
                Service = new Service
                {
                    Id = service.Id,
                    Type = service.Type,
                    ServiceEndpoint = service.ServiceEndpoint
                },
                Proof = proof
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.AddServiceAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            return new ModifyDIDResult(
                Success: response.Success,
                TransactionHash: response.TransactionHash,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding service to DID: {DID}", did);
            return new ModifyDIDResult(false, null, ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<ModifyDIDResult> RemoveServiceAsync(
        string did,
        string serviceId,
        string proof,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Removing service from DID: {DID}, Service: {ServiceId}", did, serviceId);

        try
        {
            var request = new RemoveServiceRequest
            {
                Did = did,
                ServiceId = serviceId,
                Proof = proof
            };

            var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
            var response = await _client.RemoveServiceAsync(request, deadline: deadline, cancellationToken: cancellationToken);

            return new ModifyDIDResult(
                Success: response.Success,
                TransactionHash: response.TransactionHash,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error removing service from DID: {DID}", did);
            return new ModifyDIDResult(false, null, ex.Message);
        }
    }

    /// <summary>
    /// Disposes the client and its resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the client and its resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _channel.Dispose();
            }
            _disposed = true;
        }
    }
}
