using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.CryptoExchange;

namespace Mamey.Blockchain.CryptoExchange;

public class CryptoExchangeClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly CryptoExchangeService.CryptoExchangeServiceClient _client;
    private readonly ILogger<CryptoExchangeClient>? _logger;
    private readonly CryptoExchangeClientOptions _options;

    public CryptoExchangeClient(CryptoExchangeClientOptions options, ILogger<CryptoExchangeClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new CryptoExchangeService.CryptoExchangeServiceClient(_channel);

        _logger?.LogInformation("CryptoExchange client initialized for {Address}", address);
    }

    public CryptoExchangeClient(IOptions<CryptoExchangeClientOptions> options, ILogger<CryptoExchangeClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
