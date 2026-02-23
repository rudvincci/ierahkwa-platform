using Grpc.Net.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Government;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.Blockchain.Government;

/// <summary>
/// Client for GovernmentService operations
/// </summary>
public class MameyGovernmentClient : IMameyGovernmentClient, IDisposable
{
    private readonly GovernmentService.GovernmentServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyGovernmentClient>? _logger;
    private readonly MameyGovernmentClientOptions _options;
    private readonly CredentialProvider _defaultCredentialProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyGovernmentClient
    /// </summary>
    public MameyGovernmentClient(IOptions<MameyGovernmentClientOptions> options, ILogger<MameyGovernmentClient>? logger = null)
    {
        _options = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(
            _options.NodeUrl,
            new GrpcChannelOptions
            {
                HttpHandler = CreateHttpHandler(_options.Tls, _logger)
            });
        _client = new GovernmentService.GovernmentServiceClient(_channel);
        
        // Create default credential provider from options
        _defaultCredentialProvider = new CredentialProvider
        {
            InstitutionId = _options.DefaultInstitutionId,
            CredentialChain = _options.DefaultCredentialChain,
            CorrelationIdGenerator = _options.CorrelationIdGenerator
        };
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyGovernmentClient), _options.NodeUrl);
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

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public GovernmentService.GovernmentServiceClient Client => _client;

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
