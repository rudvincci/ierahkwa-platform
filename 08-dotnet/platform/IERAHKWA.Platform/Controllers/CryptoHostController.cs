using Microsoft.AspNetCore.Mvc;
using IERAHKWA.Platform.Services;

namespace IERAHKWA.Platform.Controllers;

[ApiController]
[Route("api/cryptohost")]
public class CryptoHostController : ControllerBase
{
    private readonly ICryptoHostService _crypto;

    public CryptoHostController(ICryptoHostService crypto) => _crypto = crypto;

    [HttpGet("assets")]
    public async Task<IActionResult> GetAssets() =>
        Ok(new { success = true, data = await _crypto.GetAllAssetsAsync() });

    [HttpGet("assets/{symbol}")]
    public async Task<IActionResult> GetAsset(string symbol)
    {
        var asset = await _crypto.GetAssetAsync(symbol);
        return asset != null ? Ok(new { success = true, data = asset }) : NotFound(new { success = false });
    }

    [HttpGet("trending")]
    public async Task<IActionResult> GetTrending([FromQuery] int limit = 10) =>
        Ok(new { success = true, data = await _crypto.GetTrendingAsync(limit) });

    [HttpGet("market")]
    public async Task<IActionResult> GetMarketOverview() =>
        Ok(new { success = true, data = await _crypto.GetMarketOverviewAsync() });

    [HttpPost("quote")]
    public async Task<IActionResult> GetQuote([FromBody] QuoteRequest req)
    {
        try
        {
            var quote = await _crypto.GetSwapQuoteAsync(req.From, req.To, req.Amount);
            return Ok(new { success = true, data = quote });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("swap")]
    public async Task<IActionResult> ExecuteSwap([FromBody] SwapRequest req)
    {
        var quote = await _crypto.GetSwapQuoteAsync(req.From, req.To, req.Amount);
        var result = await _crypto.ExecuteSwapAsync(req.UserId, quote);
        return result.Success ? Ok(new { success = true, data = result }) : BadRequest(new { success = false, error = result.Error });
    }

    [HttpGet("portfolio/{userId}")]
    public async Task<IActionResult> GetPortfolio(string userId) =>
        Ok(new { success = true, data = await _crypto.GetPortfolioAsync(userId) });

    [HttpGet("portfolio/{userId}/analytics")]
    public async Task<IActionResult> GetPortfolioAnalytics(string userId) =>
        Ok(new { success = true, data = await _crypto.GetPortfolioAnalyticsAsync(userId) });

    [HttpGet("history/{userId}")]
    public async Task<IActionResult> GetSwapHistory(string userId) =>
        Ok(new { success = true, data = await _crypto.GetSwapHistoryAsync(userId) });

    [HttpGet("ai/insights")]
    public async Task<IActionResult> GetInsights([FromQuery] string? symbol = null) =>
        Ok(new { success = true, data = await _crypto.GetAIInsightsAsync(symbol) });

    [HttpGet("ai/predict/{symbol}")]
    public async Task<IActionResult> GetPrediction(string symbol, [FromQuery] string timeframe = "24h") =>
        Ok(new { success = true, data = await _crypto.GetPricePredictionAsync(symbol, timeframe) });

    [HttpPost("ai/ask")]
    public async Task<IActionResult> AskAI([FromBody] AIQuestionRequest req) =>
        Ok(new { success = true, data = new { response = await _crypto.AskCryptoAIAsync(req.UserId, req.Question) } });

    [HttpGet("staking/pools")]
    public async Task<IActionResult> GetStakingPools() =>
        Ok(new { success = true, data = await _crypto.GetStakingPoolsAsync() });

    [HttpPost("staking/stake")]
    public async Task<IActionResult> Stake([FromBody] StakeRequest req)
    {
        var result = await _crypto.StakeAsync(req.UserId, req.PoolId, req.Amount);
        return result.Success ? Ok(new { success = true, data = result }) : BadRequest(new { success = false, error = result.Error });
    }

    [HttpPost("staking/unstake")]
    public async Task<IActionResult> Unstake([FromBody] UnstakeRequest req)
    {
        var result = await _crypto.UnstakeAsync(req.UserId, req.StakeId);
        return result.Success ? Ok(new { success = true, data = result }) : BadRequest(new { success = false, error = result.Error });
    }
}

public class QuoteRequest { public string From { get; set; } = ""; public string To { get; set; } = ""; public decimal Amount { get; set; } }
public class SwapRequest { public string UserId { get; set; } = ""; public string From { get; set; } = ""; public string To { get; set; } = ""; public decimal Amount { get; set; } }
public class AIQuestionRequest { public string UserId { get; set; } = ""; public string Question { get; set; } = ""; }
public class StakeRequest { public string UserId { get; set; } = ""; public string PoolId { get; set; } = ""; public decimal Amount { get; set; } }
public class UnstakeRequest { public string UserId { get; set; } = ""; public string StakeId { get; set; } = ""; }
