using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models;

namespace NET10.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Get all tokens
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Token>>> GetAllTokens()
    {
        var tokens = await _tokenService.GetAllTokensAsync();
        return Ok(tokens);
    }

    /// <summary>
    /// Get token by ID
    /// </summary>
    [HttpGet("{tokenId}")]
    public async Task<ActionResult<Token>> GetToken(string tokenId)
    {
        var token = await _tokenService.GetTokenByIdAsync(tokenId);
        if (token == null)
            return NotFound();
        return Ok(token);
    }

    /// <summary>
    /// Get token by symbol
    /// </summary>
    [HttpGet("symbol/{symbol}")]
    public async Task<ActionResult<Token>> GetTokenBySymbol(string symbol)
    {
        var token = await _tokenService.GetTokenBySymbolAsync(symbol);
        if (token == null)
            return NotFound();
        return Ok(token);
    }

    /// <summary>
    /// Get token by address
    /// </summary>
    [HttpGet("address/{address}")]
    public async Task<ActionResult<Token>> GetTokenByAddress(string address)
    {
        var token = await _tokenService.GetTokenByAddressAsync(address);
        if (token == null)
            return NotFound();
        return Ok(token);
    }

    /// <summary>
    /// Search tokens
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<Token>>> SearchTokens([FromQuery] string q)
    {
        var tokens = await _tokenService.SearchTokensAsync(q);
        return Ok(tokens);
    }

    /// <summary>
    /// Add new token (admin only)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Token>> AddToken([FromBody] Token token)
    {
        var created = await _tokenService.AddTokenAsync(token);
        return CreatedAtAction(nameof(GetToken), new { tokenId = created.Id }, created);
    }

    /// <summary>
    /// Update token (admin only)
    /// </summary>
    [HttpPut("{tokenId}")]
    public async Task<ActionResult<Token>> UpdateToken(string tokenId, [FromBody] Token token)
    {
        token.Id = tokenId;
        var updated = await _tokenService.UpdateTokenAsync(token);
        return Ok(updated);
    }

    /// <summary>
    /// Remove token (admin only)
    /// </summary>
    [HttpDelete("{tokenId}")]
    public async Task<ActionResult> RemoveToken(string tokenId)
    {
        var success = await _tokenService.RemoveTokenAsync(tokenId);
        if (!success)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get user token balances
    /// </summary>
    [HttpGet("balances/{userId}")]
    public async Task<ActionResult<List<TokenBalance>>> GetUserBalances(string userId)
    {
        var balances = await _tokenService.GetUserBalancesAsync(userId);
        return Ok(balances);
    }

    /// <summary>
    /// Get token price
    /// </summary>
    [HttpGet("{tokenId}/price")]
    public async Task<ActionResult> GetTokenPrice(string tokenId)
    {
        var price = await _tokenService.GetTokenPriceAsync(tokenId);
        return Ok(new { tokenId, price });
    }
}
