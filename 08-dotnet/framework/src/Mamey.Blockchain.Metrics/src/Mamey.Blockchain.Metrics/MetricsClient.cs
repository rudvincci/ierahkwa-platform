using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Metrics;

namespace Mamey.Blockchain.Metrics;

public class MetricsClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly MetricsService.MetricsServiceClient _client;
    private readonly ILogger<MetricsClient>? _logger;
    private readonly MetricsClientOptions _options;

    public MetricsClient(MetricsClientOptions options, ILogger<MetricsClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new MetricsService.MetricsServiceClient(_channel);

        _logger?.LogInformation("Metrics client initialized for {Address}", address);
    }

    public MetricsClient(IOptions<MetricsClientOptions> options, ILogger<MetricsClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
