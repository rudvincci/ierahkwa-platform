using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.General;

namespace Mamey.Blockchain.General;

public class GeneralClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly GeneralService.GeneralServiceClient _client;
    private readonly ILogger<GeneralClient>? _logger;
    private readonly GeneralClientOptions _options;

    public GeneralClient(GeneralClientOptions options, ILogger<GeneralClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new GeneralService.GeneralServiceClient(_channel);

        _logger?.LogInformation("General client initialized for {Address}", address);
    }

    public GeneralClient(IOptions<GeneralClientOptions> options, ILogger<GeneralClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
