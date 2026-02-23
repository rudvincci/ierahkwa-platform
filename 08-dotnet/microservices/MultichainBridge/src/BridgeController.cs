// ============================================================================
// IERAHKWA SOVEREIGN PLATFORM - BRIDGE API CONTROLLER
// REST endpoints for cross-chain bridge operations
// ============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultichainBridge;

[ApiController]
[Route("api/bridge")]
[Authorize]
public class BridgeController : ControllerBase
{
    private readonly IBridgeService _bridgeService;

    public BridgeController(IBridgeService bridgeService)
    {
        _bridgeService = bridgeService;
    }

    /// <summary>
    /// Get supported tokens for bridging
    /// </summary>
    [HttpGet("tokens")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSupportedTokens()
    {
        var tokens = await _bridgeService.GetSupportedTokensAsync();
        return Ok(new { success = true, data = tokens });
    }

    /// <summary>
    /// Get bridge quote
    /// </summary>
    [HttpPost("quote")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQuote([FromBody] QuoteRequest request)
    {
        try
        {
            var quote = await _bridgeService.GetQuoteAsync(
                request.SourceChain,
                request.DestinationChain,
                request.Token,
                request.Amount
            );

            return Ok(new { success = true, data = quote });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Initiate a bridge transaction
    /// </summary>
    [HttpPost("transfer")]
    public async Task<IActionResult> InitiateTransfer([FromBody] TransferRequest request)
    {
        try
        {
            // Get quote first
            var quote = await _bridgeService.GetQuoteAsync(
                request.SourceChain,
                request.DestinationChain,
                request.Token,
                request.Amount
            );

            // Check if quote is still valid
            if (DateTime.UtcNow > quote.ValidUntil)
            {
                return BadRequest(new { success = false, error = "Quote expired, please request a new one" });
            }

            var transaction = await _bridgeService.InitiateBridgeAsync(
                quote,
                request.SenderAddress,
                request.RecipientAddress
            );

            return Ok(new
            {
                success = true,
                data = new
                {
                    transaction.Id,
                    transaction.SourceChain,
                    transaction.DestinationChain,
                    transaction.TokenSymbol,
                    transaction.Amount,
                    transaction.Fee,
                    transaction.Status,
                    transaction.CreatedAt,
                    depositAddress = GetDepositAddress(transaction.SourceChain, transaction.TokenSymbol),
                    instructions = GetDepositInstructions(transaction)
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Get transaction status
    /// </summary>
    [HttpGet("transaction/{id}")]
    public async Task<IActionResult> GetTransaction(string id)
    {
        var transaction = await _bridgeService.GetTransactionAsync(id);
        
        if (transaction == null)
            return NotFound(new { success = false, error = "Transaction not found" });

        return Ok(new { success = true, data = transaction });
    }

    /// <summary>
    /// Get user's bridge transactions
    /// </summary>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetUserTransactions([FromQuery] string address)
    {
        if (string.IsNullOrEmpty(address))
            return BadRequest(new { success = false, error = "Address is required" });

        var transactions = await _bridgeService.GetUserTransactionsAsync(address);
        return Ok(new { success = true, data = transactions });
    }

    /// <summary>
    /// Validate address for a chain
    /// </summary>
    [HttpGet("validate-address")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateAddress([FromQuery] ChainId chain, [FromQuery] string address)
    {
        var isValid = await _bridgeService.ValidateAddressAsync(chain, address);
        return Ok(new { success = true, data = new { isValid, chain, address } });
    }

    /// <summary>
    /// Get supported chains
    /// </summary>
    [HttpGet("chains")]
    [AllowAnonymous]
    public IActionResult GetSupportedChains()
    {
        var chains = new[]
        {
            new { id = ChainId.Ethereum, name = "Ethereum", symbol = "ETH", explorer = "https://etherscan.io" },
            new { id = ChainId.Polygon, name = "Polygon", symbol = "MATIC", explorer = "https://polygonscan.com" },
            new { id = ChainId.BNBChain, name = "BNB Chain", symbol = "BNB", explorer = "https://bscscan.com" },
            new { id = ChainId.Avalanche, name = "Avalanche", symbol = "AVAX", explorer = "https://snowtrace.io" },
            new { id = ChainId.Arbitrum, name = "Arbitrum", symbol = "ETH", explorer = "https://arbiscan.io" },
            new { id = ChainId.Base, name = "Base", symbol = "ETH", explorer = "https://basescan.org" },
            new { id = ChainId.IerahkwaSovereign, name = "IERAHKWA Sovereign", symbol = "IRHK", explorer = "https://explorer.ierahkwa.gov" }
        };

        return Ok(new { success = true, data = chains });
    }

    /// <summary>
    /// Get bridge fees
    /// </summary>
    [HttpGet("fees")]
    [AllowAnonymous]
    public IActionResult GetBridgeFees()
    {
        var fees = new[]
        {
            new { from = "Ethereum", to = "Polygon", fee = "0.1%" },
            new { from = "Polygon", to = "Ethereum", fee = "0.2%" },
            new { from = "Ethereum", to = "BNB Chain", fee = "0.15%" },
            new { from = "IERAHKWA", to = "Ethereum", fee = "0.05%" },
            new { from = "Ethereum", to = "IERAHKWA", fee = "0.05%" }
        };

        return Ok(new { success = true, data = fees });
    }

    private string GetDepositAddress(ChainId chain, string token)
    {
        // In production, generate unique deposit address per transaction
        return chain switch
        {
            ChainId.Ethereum => "0x1234567890123456789012345678901234567890",
            ChainId.Polygon => "0x2345678901234567890123456789012345678901",
            ChainId.BNBChain => "0x3456789012345678901234567890123456789012",
            ChainId.IerahkwaSovereign => "0x0000000000000000000000000000000000001111",
            _ => "0x0000000000000000000000000000000000000000"
        };
    }

    private object GetDepositInstructions(BridgeTransaction transaction)
    {
        return new
        {
            step1 = $"Send exactly {transaction.Amount} {transaction.TokenSymbol} to the deposit address",
            step2 = "Wait for confirmations on the source chain",
            step3 = "Tokens will be automatically sent to your destination address",
            note = "Do not send from an exchange wallet",
            estimatedTime = $"{GetEstimatedMinutes(transaction.SourceChain)} minutes"
        };
    }

    private int GetEstimatedMinutes(ChainId chain)
    {
        return chain switch
        {
            ChainId.Ethereum => 15,
            ChainId.Polygon => 5,
            ChainId.BNBChain => 3,
            ChainId.IerahkwaSovereign => 1,
            _ => 10
        };
    }
}

// Request DTOs
public class QuoteRequest
{
    public ChainId SourceChain { get; set; }
    public ChainId DestinationChain { get; set; }
    public string Token { get; set; } = "";
    public decimal Amount { get; set; }
}

public class TransferRequest
{
    public ChainId SourceChain { get; set; }
    public ChainId DestinationChain { get; set; }
    public string Token { get; set; } = "";
    public decimal Amount { get; set; }
    public string SenderAddress { get; set; } = "";
    public string RecipientAddress { get; set; } = "";
}
