using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Compliance;

namespace Mamey.Blockchain.Compliance;

public class ComplianceClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly ComplianceService.ComplianceServiceClient _client;
    private readonly ILogger<ComplianceClient>? _logger;
    private readonly ComplianceClientOptions _options;

    public ComplianceClient(ComplianceClientOptions options, ILogger<ComplianceClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new ComplianceService.ComplianceServiceClient(_channel);

        _logger?.LogInformation("Compliance client initialized for {Address}", address);
    }

    public ComplianceClient(IOptions<ComplianceClientOptions> options, ILogger<ComplianceClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public async Task<CheckAMLResult> CheckAMLAsync(string accountId, string transactionId, string amount, string currency, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new CheckAMLRequest
            {
                AccountId = accountId,
                TransactionId = transactionId,
                Amount = amount,
                Currency = currency
            };

            var response = await _client.CheckAMLAsync(request, cancellationToken: cancellationToken);

            return new CheckAMLResult
            {
                Flagged = response.Flagged,
                RiskLevel = response.RiskLevel,
                RiskFactors = response.RiskFactors.ToList(),
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to check AML");
            throw;
        }
    }

    public async Task<VerifyKYCResult> VerifyKYCAsync(string accountId, string verificationType, Dictionary<string, string> verificationData, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new VerifyKYCRequest
            {
                AccountId = accountId,
                VerificationType = verificationType
            };
            foreach (var kvp in verificationData)
            {
                request.VerificationData[kvp.Key] = kvp.Value;
            }

            var response = await _client.VerifyKYCAsync(request, cancellationToken: cancellationToken);

            return new VerifyKYCResult
            {
                Verified = response.Verified,
                KycLevel = response.KycLevel,
                VerifiedAttributes = response.VerifiedAttributes.ToList(),
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to verify KYC");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}

