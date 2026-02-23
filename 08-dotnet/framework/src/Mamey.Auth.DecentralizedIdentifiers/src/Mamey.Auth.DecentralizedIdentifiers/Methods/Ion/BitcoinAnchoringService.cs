using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;

/// <summary>
/// Service for anchoring ION DIDs to Bitcoin blockchain.
/// </summary>
public interface IBitcoinAnchoringService
{
    /// <summary>
    /// Anchors a DID operation to Bitcoin blockchain.
    /// </summary>
    Task<BitcoinAnchoringResult> AnchorToBitcoinAsync(string operationHash, BitcoinAnchoringOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies if a DID operation is anchored to Bitcoin.
    /// </summary>
    Task<bool> VerifyAnchoringAsync(string operationHash, BitcoinAnchoringOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the Bitcoin transaction ID for a DID operation.
    /// </summary>
    Task<string> GetAnchoringTransactionIdAsync(string operationHash, BitcoinAnchoringOptions options, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of Bitcoin anchoring operation.
/// </summary>
public class BitcoinAnchoringResult
{
    /// <summary>
    /// Whether the anchoring was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Bitcoin transaction ID.
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Block height where the transaction was included.
    /// </summary>
    public int BlockHeight { get; set; }

    /// <summary>
    /// Number of confirmations.
    /// </summary>
    public int Confirmations { get; set; }

    /// <summary>
    /// Error message if anchoring failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the anchoring was completed.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Implementation of Bitcoin anchoring service.
/// </summary>
internal sealed class BitcoinAnchoringService : IBitcoinAnchoringService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BitcoinAnchoringService> _logger;

    public BitcoinAnchoringService(HttpClient httpClient, ILogger<BitcoinAnchoringService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BitcoinAnchoringResult> AnchorToBitcoinAsync(string operationHash, BitcoinAnchoringOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!options.Enabled)
            {
                _logger.LogInformation("Bitcoin anchoring is disabled for operation: {OperationHash}", operationHash);
                return new BitcoinAnchoringResult { Success = true, ErrorMessage = "Bitcoin anchoring disabled" };
            }

            if (string.IsNullOrEmpty(options.BitcoinNodeEndpoint))
            {
                throw new InvalidOperationException("Bitcoin node endpoint is required for anchoring");
            }

            _logger.LogInformation("Anchoring operation to Bitcoin: {OperationHash}", operationHash);

            // Create OP_RETURN transaction with the operation hash
            var transactionId = await CreateOpReturnTransactionAsync(operationHash, options, cancellationToken);

            // Wait for confirmation if required
            if (options.MinConfirmations > 0)
            {
                await WaitForConfirmationsAsync(transactionId, options.MinConfirmations, options, cancellationToken);
            }

            _logger.LogInformation("Successfully anchored operation to Bitcoin: {OperationHash}, TX: {TransactionId}", operationHash, transactionId);

            return new BitcoinAnchoringResult
            {
                Success = true,
                TransactionId = transactionId,
                Confirmations = options.MinConfirmations
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to anchor operation to Bitcoin: {OperationHash}", operationHash);
            return new BitcoinAnchoringResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<bool> VerifyAnchoringAsync(string operationHash, BitcoinAnchoringOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!options.Enabled)
            {
                return true; // If anchoring is disabled, consider it verified
            }

            var transactionId = await GetAnchoringTransactionIdAsync(operationHash, options, cancellationToken);
            return !string.IsNullOrEmpty(transactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Bitcoin anchoring for operation: {OperationHash}", operationHash);
            return false;
        }
    }

    public async Task<string> GetAnchoringTransactionIdAsync(string operationHash, BitcoinAnchoringOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!options.Enabled)
            {
                return string.Empty;
            }

            // Search for OP_RETURN transactions containing the operation hash
            var searchResult = await SearchOpReturnTransactionsAsync(operationHash, options, cancellationToken);
            return searchResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get anchoring transaction ID for operation: {OperationHash}", operationHash);
            return string.Empty;
        }
    }

    private async Task<string> CreateOpReturnTransactionAsync(string operationHash, BitcoinAnchoringOptions options, CancellationToken cancellationToken)
    {
        // Create a Bitcoin transaction with OP_RETURN containing the operation hash
        var request = new
        {
            method = "createrawtransaction",
            @params = new object[]
            {
                new object[] { }, // inputs
                new Dictionary<string, object>
                {
                    { "data", ConvertToHex(operationHash) } // OP_RETURN output
                }
            }
        };

        var response = await CallBitcoinRpcAsync(request, options, cancellationToken);
        
        if (response.TryGetProperty("result", out var result))
        {
            var rawTransaction = result.GetString();
            
            // Sign and broadcast the transaction
            var signRequest = new
            {
                method = "signrawtransactionwithwallet",
                @params = new object[] { rawTransaction }
            };

            var signResponse = await CallBitcoinRpcAsync(signRequest, options, cancellationToken);
            
            if (signResponse.TryGetProperty("result", out var signResult) &&
                signResult.TryGetProperty("hex", out var signedHex))
            {
                var broadcastRequest = new
                {
                    method = "sendrawtransaction",
                    @params = new object[] { signedHex.GetString() }
                };

                var broadcastResponse = await CallBitcoinRpcAsync(broadcastRequest, options, cancellationToken);
                
                if (broadcastResponse.TryGetProperty("result", out var txId))
                {
                    return txId.GetString();
                }
            }
        }

        throw new InvalidOperationException("Failed to create and broadcast Bitcoin transaction");
    }

    private async Task WaitForConfirmationsAsync(string transactionId, int minConfirmations, BitcoinAnchoringOptions options, CancellationToken cancellationToken)
    {
        var maxWaitTime = TimeSpan.FromMinutes(30); // Maximum wait time
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < maxWaitTime)
        {
            var request = new
            {
                method = "gettransaction",
                @params = new object[] { transactionId }
            };

            var response = await CallBitcoinRpcAsync(request, options, cancellationToken);
            
            if (response.TryGetProperty("result", out var result) &&
                result.TryGetProperty("confirmations", out var confirmations))
            {
                var confirmationCount = confirmations.GetInt32();
                if (confirmationCount >= minConfirmations)
                {
                    _logger.LogInformation("Transaction {TransactionId} has {Confirmations} confirmations", transactionId, confirmationCount);
                    return;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }

        throw new TimeoutException($"Transaction {transactionId} did not reach {minConfirmations} confirmations within the timeout period");
    }

    private async Task<string> SearchOpReturnTransactionsAsync(string operationHash, BitcoinAnchoringOptions options, CancellationToken cancellationToken)
    {
        // Search for transactions with OP_RETURN containing the operation hash
        var request = new
        {
            method = "searchrawtransactions",
            @params = new object[] { "data", ConvertToHex(operationHash) }
        };

        var response = await CallBitcoinRpcAsync(request, options, cancellationToken);
        
        if (response.TryGetProperty("result", out var result) &&
            result.ValueKind == JsonValueKind.Array &&
            result.GetArrayLength() > 0)
        {
            var firstTx = result[0];
            if (firstTx.TryGetProperty("txid", out var txId))
            {
                return txId.GetString();
            }
        }

        return string.Empty;
    }

    private async Task<JsonElement> CallBitcoinRpcAsync(object request, BitcoinAnchoringOptions options, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Add basic authentication if credentials are provided
        if (!string.IsNullOrEmpty(options.Credentials.Username) && !string.IsNullOrEmpty(options.Credentials.Password))
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Credentials.Username}:{options.Credentials.Password}"));
            content.Headers.Add("Authorization", $"Basic {credentials}");
        }

        var response = await _httpClient.PostAsync(options.BitcoinNodeEndpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<JsonElement>(responseJson);
    }

    private static string ConvertToHex(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}





