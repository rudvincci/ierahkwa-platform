using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models;

namespace NET10.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SwapController : ControllerBase
{
    private readonly ISwapService _swapService;

    public SwapController(ISwapService swapService)
    {
        _swapService = swapService;
    }

    /// <summary>
    /// Get swap quote for exact input
    /// </summary>
    [HttpGet("quote")]
    public async Task<ActionResult<SwapQuote>> GetQuote(
        [FromQuery] string tokenIn, 
        [FromQuery] string tokenOut, 
        [FromQuery] decimal amount)
    {
        try
        {
            var quote = await _swapService.GetQuoteAsync(tokenIn, tokenOut, amount);
            return Ok(quote);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get swap quote for exact output
    /// </summary>
    [HttpGet("quote-exact-out")]
    public async Task<ActionResult<SwapQuote>> GetQuoteExactOut(
        [FromQuery] string tokenIn, 
        [FromQuery] string tokenOut, 
        [FromQuery] decimal amountOut)
    {
        try
        {
            var quote = await _swapService.GetQuoteExactOutAsync(tokenIn, tokenOut, amountOut);
            return Ok(quote);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Execute a swap
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SwapTransaction>> ExecuteSwap([FromBody] SwapRequest request)
    {
        try
        {
            var transaction = await _swapService.ExecuteSwapAsync(request);
            return Ok(transaction);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get user swap history
    /// </summary>
    [HttpGet("history/{userId}")]
    public async Task<ActionResult<List<SwapTransaction>>> GetUserHistory(string userId, [FromQuery] int limit = 50)
    {
        var history = await _swapService.GetUserSwapHistoryAsync(userId, limit);
        return Ok(history);
    }

    /// <summary>
    /// Get recent swaps (all users)
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<List<SwapTransaction>>> GetRecentSwaps([FromQuery] int limit = 100)
    {
        var swaps = await _swapService.GetRecentSwapsAsync(limit);
        return Ok(swaps);
    }

    /// <summary>
    /// Get exchange rate between two tokens
    /// </summary>
    [HttpGet("rate")]
    public async Task<ActionResult> GetRate([FromQuery] string tokenIn, [FromQuery] string tokenOut)
    {
        var rate = await _swapService.GetRateAsync(tokenIn, tokenOut);
        return Ok(new { tokenIn, tokenOut, rate });
    }

    /// <summary>
    /// Find best route for swap
    /// </summary>
    [HttpGet("route")]
    public async Task<ActionResult> GetBestRoute([FromQuery] string tokenIn, [FromQuery] string tokenOut)
    {
        var route = await _swapService.FindBestRouteAsync(tokenIn, tokenOut);
        return Ok(new { route });
    }
}
