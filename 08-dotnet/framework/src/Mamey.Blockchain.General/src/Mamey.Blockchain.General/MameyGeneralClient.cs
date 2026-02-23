using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.General;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.Blockchain.General;

/// <summary>
/// Client for GeneralService operations
/// </summary>
public class MameyGeneralClient : IDisposable
{
    private readonly GeneralService.GeneralServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyGeneralClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyGeneralClient
    /// </summary>
    public MameyGeneralClient(IOptions<MameyGeneralClientOptions> options, ILogger<MameyGeneralClient>? logger = null)
    {
        var opts = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(
            opts.NodeUrl,
            new GrpcChannelOptions
            {
                HttpHandler = CreateHttpHandler(opts.Tls, _logger)
            });
        _client = new GeneralService.GeneralServiceClient(_channel);
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyGeneralClient), opts.NodeUrl);
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
    /// Gets the underlying gRPC client
    /// </summary>
    public GeneralService.GeneralServiceClient Client => _client;

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
