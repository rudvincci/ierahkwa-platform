using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Crypto;

namespace Mamey.Blockchain.Crypto;

public class CryptoClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly CryptoService.CryptoServiceClient _client;
    private readonly ILogger<CryptoClient>? _logger;
    private readonly CryptoClientOptions _options;

    public CryptoClient(CryptoClientOptions options, ILogger<CryptoClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new CryptoService.CryptoServiceClient(_channel);

        _logger?.LogInformation("Crypto client initialized for {Address}", address);
    }

    public CryptoClient(IOptions<CryptoClientOptions> options, ILogger<CryptoClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
