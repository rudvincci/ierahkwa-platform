using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Bridge;

namespace Mamey.Blockchain.Bridge;

/// <summary>
/// Client for BridgeService operations
/// </summary>
public class MameyBridgeClient : IDisposable
{
    private readonly BridgeService.BridgeServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyBridgeClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyBridgeClient
    /// </summary>
    public MameyBridgeClient(IOptions<MameyBridgeClientOptions> options, ILogger<MameyBridgeClient>? logger = null)
    {
        var opts = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(opts.NodeUrl);
        _client = new BridgeService.BridgeServiceClient(_channel);
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyBridgeClient), opts.NodeUrl);
    }

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public BridgeService.BridgeServiceClient Client => _client;

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
