using Grpc.Core;
using Mamey.Banking;

namespace Mamey.Blockchain.Banking;

public interface IMameyBankingClient
{
    /// <summary>
    /// Gets the underlying gRPC client
    /// </summary>
    BankingService.BankingServiceClient Client { get; }

    /// <summary>
    /// Create CallOptions with metadata from credential provider
    /// </summary>
    CallOptions CreateCallOptions(CredentialProvider? credentialProvider = null, string? correlationId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get account info with metadata support
    /// </summary>
    Task<GetAccountInfoResponse> GetAccountInfoAsync(
        GetAccountInfoRequest request,
        CredentialProvider? credentialProvider = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create account with metadata support
    /// </summary>
    Task<CreateAccountResponse> CreateAccountAsync(
        CreateAccountRequest request,
        CredentialProvider? credentialProvider = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create payment request with metadata support (capability-gated operation)
    /// </summary>
    Task<CreatePaymentRequestResponse> CreatePaymentRequestAsync(
        CreatePaymentRequestRequest request,
        CredentialProvider? credentialProvider = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payment request with metadata support
    /// </summary>
    Task<GetPaymentRequestResponse> GetPaymentRequestAsync(
        GetPaymentRequestRequest request,
        CredentialProvider? credentialProvider = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);
}









