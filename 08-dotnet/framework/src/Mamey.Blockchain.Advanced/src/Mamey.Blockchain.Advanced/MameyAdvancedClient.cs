using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Advanced;

namespace Mamey.Blockchain.Advanced;

/// <summary>
/// Client for AdvancedService operations
/// </summary>
public class MameyAdvancedClient : IDisposable
{
    private readonly AdvancedService.AdvancedServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyAdvancedClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyAdvancedClient
    /// </summary>
    public MameyAdvancedClient(IOptions<MameyAdvancedClientOptions> options, ILogger<MameyAdvancedClient>? logger = null)
    {
        var opts = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(opts.NodeUrl);
        _client = new AdvancedService.AdvancedServiceClient(_channel);
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyAdvancedClient), opts.NodeUrl);
    }

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public AdvancedService.AdvancedServiceClient Client => _client;

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
