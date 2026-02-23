using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.CryptoExchange;

namespace Mamey.Blockchain.CryptoExchange;

/// <summary>
/// Client for CryptoExchangeService operations
/// </summary>
public class MameyCryptoExchangeClient : IDisposable
{
    private readonly CryptoExchangeService.CryptoExchangeServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyCryptoExchangeClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyCryptoExchangeClient
    /// </summary>
    public MameyCryptoExchangeClient(IOptions<MameyCryptoExchangeClientOptions> options, ILogger<MameyCryptoExchangeClient>? logger = null)
    {
        var opts = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(opts.NodeUrl);
        _client = new CryptoExchangeService.CryptoExchangeServiceClient(_channel);
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyCryptoExchangeClient), opts.NodeUrl);
    }

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public CryptoExchangeService.CryptoExchangeServiceClient Client => _client;

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
