using Microsoft.AspNetCore.Mvc;
using TradeX.Core.Interfaces;

namespace TradeX.API.Controllers;

/// <summary>
/// Crypto Wallet API - IERAHKWA TradeX
/// CodeCanyon-style: Bitcoin, Ethereum, ERC20, BEP20, Matic. Non-custodial. Exchange, fiat on-ramp, admin fee.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CryptoWalletController : ControllerBase
{
    private readonly ICryptoWalletConfigService _config;
    private readonly ILogger<CryptoWalletController> _log;

    public CryptoWalletController(ICryptoWalletConfigService config, ILogger<CryptoWalletController> log)
    {
        _config = config;
        _log = log;
    }

    /// <summary>Wallet UI settings: theme, registration, exchange-only mode, admin fee %.</summary>
    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        return Ok(_config.GetSettings());
    }

    /// <summary>Fiat on-ramp: Visa/MC via Transak or itez. SupportedAssets, Provider, BaseUrl.</summary>
    [HttpGet("fiat-on-ramp")]
    public IActionResult GetFiatOnRamp()
    {
        return Ok(_config.GetFiatOnRampConfig());
    }

    /// <summary>Tokens: ETH, BSC, POLYGON, BITCOIN, IERAHKWA. From Etherscan/BscScan or custom. ?network=ETH</summary>
    [HttpGet("tokens")]
    public IActionResult GetTokens([FromQuery] string? network = null)
    {
        return Ok(_config.GetTokens(network));
    }

    /// <summary>Swap quote with base + admin fee. fromSymbol, toSymbol, fromAmount.</summary>
    [HttpGet("quote")]
    public IActionResult GetQuote([FromQuery] string fromSymbol, [FromQuery] string toSymbol, [FromQuery] decimal fromAmount)
    {
        if (fromAmount <= 0) return BadRequest(new { error = "fromAmount must be positive" });
        var q = _config.GetSwapQuote(fromSymbol, toSymbol, fromAmount);
        return Ok(q);
    }
}
