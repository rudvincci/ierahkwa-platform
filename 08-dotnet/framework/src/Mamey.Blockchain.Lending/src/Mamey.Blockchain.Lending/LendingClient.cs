using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Lending;

namespace Mamey.Blockchain.Lending;

/// <summary>
/// Client for interacting with MameyNode lending operations via gRPC
/// </summary>
public class LendingClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly LendingService.LendingServiceClient _client;
    private readonly ILogger<LendingClient>? _logger;
    private readonly LendingClientOptions _options;

    public LendingClient(LendingClientOptions options, ILogger<LendingClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new LendingService.LendingServiceClient(_channel);

        _logger?.LogInformation("Lending client initialized for {Address}", address);
    }

    public LendingClient(IOptions<LendingClientOptions> options, ILogger<LendingClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    public async Task<CreateLoanResult> CreateLoanAsync(Mamey.Blockchain.Lending.CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var grpcRequest = new Mamey.Lending.CreateLoanRequest
            {
                BorrowerId = request.BorrowerId,
                Amount = request.Amount,
                Currency = request.Currency,
                InterestRate = request.InterestRate,
                TermMonths = request.TermMonths,
                Purpose = request.Purpose
            };
            grpcRequest.Collateral.AddRange(request.Collateral.Select(c => new Mamey.Lending.CollateralInfo
            {
                CollateralId = c.CollateralId,
                CollateralType = c.CollateralType,
                Value = c.Value,
                Currency = c.Currency,
                Description = c.Description
            }));

            var response = await _client.CreateLoanAsync(grpcRequest, cancellationToken: cancellationToken);

            return new CreateLoanResult
            {
                LoanId = response.LoanId,
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create loan");
            throw;
        }
    }

    public async Task<LoanInfo?> GetLoanAsync(string loanId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetLoanRequest { LoanId = loanId };
            var response = await _client.GetLoanAsync(request, cancellationToken: cancellationToken);

            if (!response.Success || response.Loan == null)
            {
                return null;
            }

            var loan = response.Loan;
            return new LoanInfo
            {
                LoanId = loan.LoanId,
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                Currency = loan.Currency,
                InterestRate = loan.InterestRate,
                TermMonths = loan.TermMonths,
                RemainingBalance = loan.RemainingBalance,
                NextPaymentAmount = loan.NextPaymentAmount,
                NextPaymentDate = DateTimeOffset.FromUnixTimeSeconds((long)loan.NextPaymentDate).DateTime,
                Status = (LoanStatus)(int)loan.Status,
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds((long)loan.CreatedAt).DateTime,
                ApprovedAt = loan.ApprovedAt > 0 ? DateTimeOffset.FromUnixTimeSeconds((long)loan.ApprovedAt).DateTime : null
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get loan {LoanId}", loanId);
            throw;
        }
    }

    public async Task<CreditRiskResult> EvaluateCreditRiskAsync(string borrowerId, string loanAmount, string currency, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new EvaluateCreditRiskRequest
            {
                BorrowerId = borrowerId,
                LoanAmount = loanAmount,
                Currency = currency
            };

            var response = await _client.EvaluateCreditRiskAsync(request, cancellationToken: cancellationToken);

            return new CreditRiskResult
            {
                RiskLevel = response.RiskLevel,
                RiskScore = response.RiskScore,
                RiskFactors = response.RiskFactors.ToList(),
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to evaluate credit risk");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}




