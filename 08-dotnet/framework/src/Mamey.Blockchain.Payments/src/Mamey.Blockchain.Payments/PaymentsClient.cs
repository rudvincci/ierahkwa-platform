using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Payments;

namespace Mamey.Blockchain.Payments;

public class PaymentsClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly PaymentService.PaymentServiceClient _client;
    private readonly ILogger<PaymentsClient>? _logger;
    private readonly PaymentsClientOptions _options;

    public PaymentsClient(PaymentsClientOptions options, ILogger<PaymentsClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new PaymentService.PaymentServiceClient(_channel);

        _logger?.LogInformation("Payments client initialized for {Address}", address);
    }

    public PaymentsClient(IOptions<PaymentsClientOptions> options, ILogger<PaymentsClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public async Task<SendP2PPaymentResult> SendP2PPaymentAsync(Mamey.Blockchain.Payments.SendP2PPaymentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var grpcRequest = new Mamey.Payments.SendP2PPaymentRequest
            {
                FromAccount = request.FromAccount,
                ToAccount = request.ToAccount,
                Amount = request.Amount,
                Currency = request.Currency,
                Memo = request.Memo
            };

            var response = await _client.SendP2PPaymentAsync(grpcRequest, cancellationToken: cancellationToken);

            return new SendP2PPaymentResult
            {
                PaymentId = response.PaymentId,
                TransactionId = response.TransactionId,
                Status = (PaymentStatus)(int)response.Status,
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to send P2P payment");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}




