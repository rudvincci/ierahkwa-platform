using Grpc.Net.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Node;
using Mamey.Rpc;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.Blockchain.Node;

/// <summary>
/// Client for NodeService operations
/// </summary>
public class MameyNodeClient : IMameyNodeClient, IDisposable
{
    private readonly NodeService.NodeServiceClient _client;
    private readonly RpcService.RpcServiceClient _rpcClient;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyNodeClient>? _logger;
    private readonly MameyNodeClientOptions _options;
    private readonly CredentialProvider _defaultCredentialProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyNodeClient
    /// </summary>
    public MameyNodeClient(IOptions<MameyNodeClientOptions> options, ILogger<MameyNodeClient>? logger = null)
    {
        _options = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(
            _options.NodeUrl,
            new GrpcChannelOptions
            {
                HttpHandler = CreateHttpHandler(_options.Tls, _logger)
            });
        _client = new NodeService.NodeServiceClient(_channel);
        _rpcClient = new RpcService.RpcServiceClient(_channel);
        
        // Create default credential provider from options
        _defaultCredentialProvider = new CredentialProvider
        {
            InstitutionId = _options.DefaultInstitutionId,
            CredentialChain = _options.DefaultCredentialChain,
            CorrelationIdGenerator = _options.CorrelationIdGenerator
        };
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyNodeClient), _options.NodeUrl);
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
            // Ensure private key is available to SslStream on all platforms
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

    /// <summary>
    /// Gets the underlying NodeService gRPC client
    /// </summary>
    public NodeService.NodeServiceClient Client => _client;

    /// <summary>
    /// Gets the underlying RpcService gRPC client
    /// </summary>
    public RpcService.RpcServiceClient RpcClient => _rpcClient;

    /// <summary>
    /// Create CallOptions with metadata from credential provider
    /// </summary>
    public CallOptions CreateCallOptions(CredentialProvider? credentialProvider = null, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        var provider = credentialProvider ?? _defaultCredentialProvider;
        var metadata = provider.GetMetadata(correlationId);
        
        var callOptions = new CallOptions(metadata, cancellationToken: cancellationToken);
        
        // Apply timeout if configured
        if (_options.TimeoutSeconds > 0)
        {
            callOptions = callOptions.WithDeadline(DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds));
        }
        
        return callOptions;
    }

    /// <summary>
    /// Get node information (staging smoke test method)
    /// </summary>
    public async Task<GetNodeInfoResponse> GetNodeInfoAsync(
        GetNodeInfoRequest? request = null,
        CredentialProvider? credentialProvider = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        request ??= new GetNodeInfoRequest();
        var callOptions = CreateCallOptions(credentialProvider, correlationId, cancellationToken);
        
        _logger?.LogDebug("Calling NodeService.GetNodeInfo with correlation ID: {CorrelationId}", 
            correlationId ?? "auto-generated");
        
        var response = await _client.GetNodeInfoAsync(request, callOptions);
        
        _logger?.LogInformation("Node info retrieved: Version={Version}, NodeId={NodeId}, BlockCount={BlockCount}", 
            response.Version, response.NodeId, response.BlockCount);
        
        return response;
    }

    /// <summary>
    /// Get RPC version (staging smoke test method)
    /// </summary>
    public async Task<VersionResponse> GetVersionAsync(
        VersionRequest? request = null,
        CredentialProvider? credentialProvider = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        request ??= new VersionRequest();
        var callOptions = CreateCallOptions(credentialProvider, correlationId, cancellationToken);
        
        _logger?.LogDebug("Calling RpcService.Version with correlation ID: {CorrelationId}", 
            correlationId ?? "auto-generated");
        
        var response = await _rpcClient.VersionAsync(request, callOptions);
        
        _logger?.LogInformation("RPC version retrieved: Success={Success}, Version={Version}", 
            response.Success, response.RpcVersion);
        
        return response;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

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
