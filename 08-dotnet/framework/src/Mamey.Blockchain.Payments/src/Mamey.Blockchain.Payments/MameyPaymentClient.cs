using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Payments;

namespace Mamey.Blockchain.Payments;

/// <summary>
/// Client for PaymentService operations
/// </summary>
public class MameyPaymentClient : IDisposable
{
    private readonly PaymentService.PaymentServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<MameyPaymentClient>? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the MameyPaymentClient
    /// </summary>
    public MameyPaymentClient(IOptions<MameyPaymentClientOptions> options, ILogger<MameyPaymentClient>? logger = null)
    {
        var opts = options.Value;
        _logger = logger;
        
        _channel = GrpcChannel.ForAddress(opts.NodeUrl);
        _client = new PaymentService.PaymentServiceClient(_channel);
        
        _logger?.LogInformation("Initialized {Client} connecting to {Url}", nameof(MameyPaymentClient), opts.NodeUrl);
    }

    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    public PaymentService.PaymentServiceClient Client => _client;

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
