using Grpc.Net.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Upg;

namespace Mamey.Blockchain.UniversalProtocolGateway;

/// <summary>
/// Client for UniversalProtocolGatewayService operations
/// </summary>
public class MameyUPGClient : IDisposable
{
    private readonly UniversalProtocolGatewayService.UniversalProtocolGatewayServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyUPGClient>? _logger;
    private readonly MameyUPGClientOptions _options;
    private readonly CredentialProvider _defaultCredentialProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyUPGClient
    /// </summary>
    public MameyUPGClient(IOptions<MameyUPGClientOptions> options, ILogger<MameyUPGClient>? logger = null)
    {
        _options = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(_options.NodeUrl);
        _client = new UniversalProtocolGatewayService.UniversalProtocolGatewayServiceClient(_channel);
        
        // Create default credential provider from options
        _defaultCredentialProvider = new CredentialProvider
        {
            InstitutionId = _options.DefaultInstitutionId,
            CredentialChain = _options.DefaultCredentialChain,
            CorrelationIdGenerator = _options.CorrelationIdGenerator
        };
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyUPGClient), _options.NodeUrl);
    }

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public UniversalProtocolGatewayService.UniversalProtocolGatewayServiceClient Client => _client;

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
