using Grpc.Net.Client;
using Mamey.FWID.Identities.Application.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Clients;

/// <summary>
/// gRPC client implementation for MameyNode Banking Service
/// </summary>
internal sealed class MameyNodeBankingClient : IMameyNodeBankingClient
{
    private readonly ILogger<MameyNodeBankingClient> _logger;
    private readonly string _grpcEndpoint;
    private readonly bool _enabled;
    private readonly bool _createAccountOnIdentityCreation;

    public MameyNodeBankingClient(
        ILogger<MameyNodeBankingClient> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _enabled = configuration.GetValue<bool>("mameyNode:enabled", false);
        _grpcEndpoint = configuration.GetValue<string>("mameyNode:grpc:endpoint") ?? "http://localhost:7076";
        _createAccountOnIdentityCreation = configuration.GetValue<bool>("mameyNode:banking:createAccountOnIdentityCreation", true);
    }

    public async Task<string?> CreateAccountAsync(string accountId, string currency = "USD", CancellationToken cancellationToken = default)
    {
        if (!_enabled)
        {
            _logger.LogDebug("MameyNode integration is disabled, skipping account creation for {AccountId}", accountId);
            return null;
        }

        try
        {
            _logger.LogDebug("Creating MameyNode account for Identity: {AccountId}, Currency: {Currency}", accountId, currency);

            // TODO: Replace with actual gRPC call once proto files are generated
            // var channel = GrpcChannel.ForAddress(_grpcEndpoint);
            // var client = new BankingService.BankingServiceClient(channel);
            // var request = new CreateAccountRequest { AccountId = accountId, Currency = currency };
            // var response = await client.CreateAccountAsync(request, cancellationToken: cancellationToken);
            // return response.Success ? response.BlockchainAccount : null;

            _logger.LogWarning(
                "MameyNode gRPC client not fully implemented. Proto files need to be generated. AccountId: {AccountId}",
                accountId);

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create MameyNode account for Identity: {AccountId}, Currency: {Currency}",
                accountId,
                currency);
            return null;
        }
    }

    public async Task<AccountBalance?> GetBalanceAsync(string accountId, CancellationToken cancellationToken = default)
    {
        if (!_enabled)
        {
            _logger.LogDebug("MameyNode integration is disabled, skipping balance check for {AccountId}", accountId);
            return null;
        }

        try
        {
            _logger.LogDebug("Getting MameyNode balance for account: {AccountId}", accountId);

            // TODO: Replace with actual gRPC call once proto files are generated
            _logger.LogWarning(
                "MameyNode gRPC client not fully implemented. Proto files need to be generated. AccountId: {AccountId}",
                accountId);

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get MameyNode balance for account: {AccountId}", accountId);
            return null;
        }
    }

    public async Task<AccountInfo?> GetAccountInfoAsync(string accountId, CancellationToken cancellationToken = default)
    {
        if (!_enabled)
        {
            _logger.LogDebug("MameyNode integration is disabled, skipping account info for {AccountId}", accountId);
            return null;
        }

        try
        {
            _logger.LogDebug("Getting MameyNode account info for: {AccountId}", accountId);

            // TODO: Replace with actual gRPC call once proto files are generated
            _logger.LogWarning(
                "MameyNode gRPC client not fully implemented. Proto files need to be generated. AccountId: {AccountId}",
                accountId);

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get MameyNode account info for: {AccountId}", accountId);
            return null;
        }
    }
}




