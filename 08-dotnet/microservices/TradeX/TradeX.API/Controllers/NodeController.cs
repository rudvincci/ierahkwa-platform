using Microsoft.AspNetCore.Mvc;
using TradeX.Core.Interfaces;

namespace TradeX.API.Controllers;

/// <summary>
/// Ierahkwa Node API
/// Direct access to Ierahkwa Futurehead Mamey Node
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NodeController : ControllerBase
{
    private readonly IIerahkwaNodeService _nodeService;
    private readonly ILogger<NodeController> _logger;
    
    public NodeController(IIerahkwaNodeService nodeService, ILogger<NodeController> logger)
    {
        _nodeService = nodeService;
        _logger = logger;
    }
    
    /// <summary>
    /// Check node health
    /// </summary>
    [HttpGet("health")]
    public async Task<ActionResult> GetHealth()
    {
        var isConnected = await _nodeService.IsConnectedAsync();
        return Ok(new 
        { 
            status = isConnected ? "connected" : "disconnected",
            node = "Ierahkwa Futurehead Mamey Node",
            network = "ierahkwa-mainnet",
            chainId = 777777,
            timestamp = DateTime.UtcNow
        });
    }
    
    /// <summary>
    /// Get asset balance
    /// </summary>
    [HttpGet("balance/{address}/{asset}")]
    public async Task<ActionResult> GetBalance(string address, string asset)
    {
        var balance = await _nodeService.GetBalanceAsync(address, asset);
        return Ok(new { address, asset, balance });
    }
    
    /// <summary>
    /// Get asset price
    /// </summary>
    [HttpGet("price/{asset}")]
    public async Task<ActionResult> GetPrice(string asset)
    {
        var price = await _nodeService.GetAssetPriceAsync(asset);
        return Ok(new { asset, price, currency = "USD" });
    }
    
    /// <summary>
    /// Validate address
    /// </summary>
    [HttpGet("validate/{address}")]
    public async Task<ActionResult> ValidateAddress(string address)
    {
        var isValid = await _nodeService.ValidateAddressAsync(address);
        return Ok(new { address, isValid });
    }
    
    /// <summary>
    /// Get transaction status
    /// </summary>
    [HttpGet("tx/{txHash}")]
    public async Task<ActionResult> GetTransactionStatus(string txHash)
    {
        var status = await _nodeService.GetTransactionStatusAsync(txHash);
        return Ok(new { txHash, status });
    }
    
    /// <summary>
    /// Generate new wallet
    /// </summary>
    [HttpPost("wallet/generate")]
    public async Task<ActionResult> GenerateWallet()
    {
        var address = await _nodeService.GenerateWalletAsync();
        return Ok(new { address, network = "ierahkwa-mainnet" });
    }
    
    /// <summary>
    /// Transfer assets
    /// </summary>
    [HttpPost("transfer")]
    public async Task<ActionResult> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            var txHash = await _nodeService.TransferAsync(
                request.FromAddress, 
                request.ToAddress, 
                request.Amount, 
                request.Asset);
            
            _logger.LogInformation("Transfer: {Amount} {Asset} from {From} to {To}, TxHash: {TxHash}",
                request.Amount, request.Asset, request.FromAddress, request.ToAddress, txHash);
            
            return Ok(new { txHash, status = "pending" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record TransferRequest(
    string FromAddress,
    string ToAddress,
    decimal Amount,
    string Asset
);
