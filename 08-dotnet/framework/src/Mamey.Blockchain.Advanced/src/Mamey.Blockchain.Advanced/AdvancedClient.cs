using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Advanced;

namespace Mamey.Blockchain.Advanced;

public class AdvancedClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly AdvancedService.AdvancedServiceClient _client;
    private readonly ILogger<AdvancedClient>? _logger;
    private readonly AdvancedClientOptions _options;

    public AdvancedClient(AdvancedClientOptions options, ILogger<AdvancedClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new AdvancedService.AdvancedServiceClient(_channel);

        _logger?.LogInformation("Advanced client initialized for {Address}", address);
    }

    public AdvancedClient(IOptions<AdvancedClientOptions> options, ILogger<AdvancedClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public async Task<TokenizeAssetResult> TokenizeAssetAsync(Mamey.Blockchain.Advanced.TokenizeAssetRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var grpcRequest = new Mamey.Advanced.TokenizeAssetRequest
            {
                AssetId = request.AssetId,
                AssetType = request.AssetType,
                OwnerId = request.OwnerId,
                TotalSupply = request.TotalSupply,
                Metadata = request.Metadata
            };
            grpcRequest.Properties.Add((IDictionary<string, string>)request.Properties);

            var response = await _client.TokenizeAssetAsync(grpcRequest, cancellationToken: cancellationToken);

            return new TokenizeAssetResult
            {
                TokenId = response.TokenId,
                ContractAddress = response.ContractAddress,
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to tokenize asset");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}




