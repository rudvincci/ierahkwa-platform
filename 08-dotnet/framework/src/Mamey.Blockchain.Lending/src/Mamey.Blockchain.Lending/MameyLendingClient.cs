using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Lending;

namespace Mamey.Blockchain.Lending;

/// <summary>
/// Client for LendingService operations
/// </summary>
public class MameyLendingClient : IDisposable
{
    private readonly LendingService.LendingServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyLendingClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyLendingClient
    /// </summary>
    public MameyLendingClient(IOptions<MameyLendingClientOptions> options, ILogger<MameyLendingClient>? logger = null)
    {
        var opts = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(opts.NodeUrl);
        _client = new LendingService.LendingServiceClient(_channel);
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyLendingClient), opts.NodeUrl);
    }

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public LendingService.LendingServiceClient Client => _client;

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
