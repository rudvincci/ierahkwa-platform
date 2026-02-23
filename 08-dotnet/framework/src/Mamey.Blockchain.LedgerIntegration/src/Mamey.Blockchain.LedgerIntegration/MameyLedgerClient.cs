using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Ledger;

namespace Mamey.Blockchain.LedgerIntegration;

/// <summary>
/// Client for LedgerIntegrationService operations
/// </summary>
public class MameyLedgerClient : IDisposable
{
    private readonly LedgerIntegrationService.LedgerIntegrationServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyLedgerClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyLedgerClient
    /// </summary>
    public MameyLedgerClient(IOptions<MameyLedgerClientOptions> options, ILogger<MameyLedgerClient>? logger = null)
    {
        var opts = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(opts.NodeUrl);
        _client = new LedgerIntegrationService.LedgerIntegrationServiceClient(_channel);
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyLedgerClient), opts.NodeUrl);
    }

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public LedgerIntegrationService.LedgerIntegrationServiceClient Client => _client;

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
