using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Compliance;

namespace Mamey.Blockchain.Compliance;

/// <summary>
/// Client for ComplianceService operations
/// </summary>
public class MameyComplianceClient : IDisposable
{
    private readonly ComplianceService.ComplianceServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyComplianceClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyComplianceClient
    /// </summary>
    public MameyComplianceClient(IOptions<MameyComplianceClientOptions> options, ILogger<MameyComplianceClient>? logger = null)
    {
        var opts = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(opts.NodeUrl);
        _client = new ComplianceService.ComplianceServiceClient(_channel);
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyComplianceClient), opts.NodeUrl);
    }

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public ComplianceService.ComplianceServiceClient Client => _client;

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
