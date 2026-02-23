using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Ledger;

namespace Mamey.Blockchain.LedgerIntegration;

public class LedgerIntegrationClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly LedgerIntegrationService.LedgerIntegrationServiceClient _client;
    private readonly ILogger<LedgerIntegrationClient>? _logger;
    private readonly LedgerIntegrationClientOptions _options;

    public LedgerIntegrationClient(LedgerIntegrationClientOptions options, ILogger<LedgerIntegrationClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new LedgerIntegrationService.LedgerIntegrationServiceClient(_channel);

        _logger?.LogInformation("LedgerIntegration client initialized for {Address}", address);
    }

    public LedgerIntegrationClient(IOptions<LedgerIntegrationClientOptions> options, ILogger<LedgerIntegrationClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
