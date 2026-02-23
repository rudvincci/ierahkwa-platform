using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models;

namespace NET10.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IConfigService _configService;

    public AdminController(IConfigService configService)
    {
        _configService = configService;
    }

    /// <summary>
    /// Get platform configuration
    /// </summary>
    [HttpGet("config")]
    public async Task<ActionResult<NET10Config>> GetConfig()
    {
        var config = await _configService.GetConfigAsync();
        return Ok(config);
    }

    /// <summary>
    /// Update platform configuration (admin only)
    /// </summary>
    [HttpPut("config")]
    public async Task<ActionResult<NET10Config>> UpdateConfig([FromBody] NET10Config config)
    {
        var updated = await _configService.UpdateConfigAsync(config);
        return Ok(updated);
    }

    /// <summary>
    /// Get platform statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<AdminStats>> GetStats()
    {
        var stats = await _configService.GetStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Get deployed contracts
    /// </summary>
    [HttpGet("contracts")]
    public async Task<ActionResult<List<DeployedContract>>> GetContracts()
    {
        var contracts = await _configService.GetDeployedContractsAsync();
        return Ok(contracts);
    }

    /// <summary>
    /// Deploy new contract (admin only)
    /// </summary>
    [HttpPost("contracts/deploy")]
    public async Task<ActionResult<DeployedContract>> DeployContract([FromBody] ContractDeployRequest request)
    {
        var contract = await _configService.DeployContractAsync(request);
        return Ok(contract);
    }

    /// <summary>
    /// Get fee settings
    /// </summary>
    [HttpGet("fees")]
    public async Task<ActionResult> GetFees()
    {
        var config = await _configService.GetConfigAsync();
        return Ok(new
        {
            swapFee = config.DefaultSwapFee,
            adminFeePercent = config.AdminFeePercent,
            lpFeePercent = config.LPFeePercent,
            feeRecipient = config.FeeRecipient
        });
    }

    /// <summary>
    /// Update fee settings (admin only)
    /// </summary>
    [HttpPut("fees")]
    public async Task<ActionResult> UpdateFees([FromBody] FeeUpdateRequest request)
    {
        var config = await _configService.GetConfigAsync();
        config.DefaultSwapFee = request.SwapFee ?? config.DefaultSwapFee;
        config.AdminFeePercent = request.AdminFeePercent ?? config.AdminFeePercent;
        config.LPFeePercent = request.LPFeePercent ?? config.LPFeePercent;
        config.FeeRecipient = request.FeeRecipient ?? config.FeeRecipient;
        await _configService.UpdateConfigAsync(config);
        return Ok(new { message = "Fees updated successfully" });
    }

    /// <summary>
    /// Get supported chains
    /// </summary>
    [HttpGet("chains")]
    public async Task<ActionResult> GetSupportedChains()
    {
        var config = await _configService.GetConfigAsync();
        var chains = new[]
        {
            new { chainId = "777777", name = "Ierahkwa Sovereign Blockchain", symbol = "ISB", rpc = "https://node.ierahkwa.gov" },
            new { chainId = "1", name = "Ethereum Mainnet", symbol = "ETH", rpc = "https://mainnet.infura.io" },
            new { chainId = "56", name = "BNB Smart Chain", symbol = "BNB", rpc = "https://bsc-dataseed.binance.org" },
            new { chainId = "137", name = "Polygon", symbol = "MATIC", rpc = "https://polygon-rpc.com" },
            new { chainId = "43114", name = "Avalanche", symbol = "AVAX", rpc = "https://api.avax.network/ext/bc/C/rpc" }
        }.Where(c => config.SupportedChains.Contains(c.chainId));
        return Ok(chains);
    }
}

public class FeeUpdateRequest
{
    public decimal? SwapFee { get; set; }
    public decimal? AdminFeePercent { get; set; }
    public decimal? LPFeePercent { get; set; }
    public string? FeeRecipient { get; set; }
}
