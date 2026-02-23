using Microsoft.AspNetCore.Mvc;
using IDOFactory.Core.Interfaces;
using IDOFactory.Core.Models;

namespace IDOFactory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IDOController : ControllerBase
{
    private readonly IIDOService _idoService;
    private readonly ILogger<IDOController> _logger;

    public IDOController(IIDOService idoService, ILogger<IDOController> logger)
    {
        _idoService = idoService;
        _logger = logger;
    }

    /// <summary>
    /// Get all IDO pools
    /// </summary>
    [HttpGet("pools")]
    public async Task<ActionResult<IEnumerable<IDOPool>>> GetAllPools()
    {
        var pools = await _idoService.GetAllPoolsAsync();
        return Ok(pools);
    }

    /// <summary>
    /// Get pools by status
    /// </summary>
    [HttpGet("pools/status/{status}")]
    public async Task<ActionResult<IEnumerable<IDOPool>>> GetPoolsByStatus(IDOPoolStatus status)
    {
        var pools = await _idoService.GetPoolsByStatusAsync(status);
        return Ok(pools);
    }

    /// <summary>
    /// Get active (live) pools
    /// </summary>
    [HttpGet("pools/active")]
    public async Task<ActionResult<IEnumerable<IDOPool>>> GetActivePools()
    {
        var pools = await _idoService.GetPoolsByStatusAsync(IDOPoolStatus.Live);
        return Ok(pools);
    }

    /// <summary>
    /// Get upcoming pools
    /// </summary>
    [HttpGet("pools/upcoming")]
    public async Task<ActionResult<IEnumerable<IDOPool>>> GetUpcomingPools()
    {
        var pools = await _idoService.GetPoolsByStatusAsync(IDOPoolStatus.Upcoming);
        return Ok(pools);
    }

    /// <summary>
    /// Get a specific pool by ID
    /// </summary>
    [HttpGet("pools/{poolId}")]
    public async Task<ActionResult<IDOPool>> GetPool(string poolId)
    {
        var pool = await _idoService.GetPoolByIdAsync(poolId);
        if (pool == null) return NotFound();
        return Ok(pool);
    }

    /// <summary>
    /// Create a new IDO pool
    /// </summary>
    [HttpPost("pools")]
    public async Task<ActionResult<IDOPool>> CreatePool([FromBody] IDOPool pool)
    {
        try
        {
            var created = await _idoService.CreatePoolAsync(pool);
            return CreatedAtAction(nameof(GetPool), new { poolId = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pool");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing pool
    /// </summary>
    [HttpPut("pools/{poolId}")]
    public async Task<ActionResult<IDOPool>> UpdatePool(string poolId, [FromBody] IDOPool pool)
    {
        try
        {
            pool.Id = poolId;
            var updated = await _idoService.UpdatePoolAsync(pool);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pool");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a draft pool
    /// </summary>
    [HttpDelete("pools/{poolId}")]
    public async Task<ActionResult> DeletePool(string poolId)
    {
        try
        {
            var deleted = await _idoService.DeletePoolAsync(poolId);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Start registration period for a pool
    /// </summary>
    [HttpPost("pools/{poolId}/start-registration")]
    public async Task<ActionResult<IDOPool>> StartRegistration(string poolId)
    {
        try
        {
            var pool = await _idoService.StartRegistrationAsync(poolId);
            return Ok(pool);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Start the sale
    /// </summary>
    [HttpPost("pools/{poolId}/start-sale")]
    public async Task<ActionResult<IDOPool>> StartSale(string poolId)
    {
        try
        {
            var pool = await _idoService.StartSaleAsync(poolId);
            return Ok(pool);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// End the sale
    /// </summary>
    [HttpPost("pools/{poolId}/end-sale")]
    public async Task<ActionResult<IDOPool>> EndSale(string poolId)
    {
        try
        {
            var pool = await _idoService.EndSaleAsync(poolId);
            return Ok(pool);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Finalize pool and enable claiming
    /// </summary>
    [HttpPost("pools/{poolId}/finalize")]
    public async Task<ActionResult<IDOPool>> FinalizePool(string poolId)
    {
        try
        {
            var pool = await _idoService.FinalizePoolAsync(poolId);
            return Ok(pool);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a pool
    /// </summary>
    [HttpPost("pools/{poolId}/cancel")]
    public async Task<ActionResult<IDOPool>> CancelPool(string poolId)
    {
        try
        {
            var pool = await _idoService.CancelPoolAsync(poolId);
            return Ok(pool);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Contribute to a pool
    /// </summary>
    [HttpPost("pools/{poolId}/contribute")]
    public async Task<ActionResult<IDOContribution>> Contribute(string poolId, [FromBody] ContributeRequest request)
    {
        try
        {
            var contribution = await _idoService.ContributeAsync(poolId, request.UserAddress, request.Amount);
            return Ok(contribution);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get user's contributions
    /// </summary>
    [HttpGet("contributions/user/{userAddress}")]
    public async Task<ActionResult<IEnumerable<IDOContribution>>> GetUserContributions(string userAddress)
    {
        var contributions = await _idoService.GetUserContributionsAsync(userAddress);
        return Ok(contributions);
    }

    /// <summary>
    /// Get pool contributions
    /// </summary>
    [HttpGet("pools/{poolId}/contributions")]
    public async Task<ActionResult<IEnumerable<IDOContribution>>> GetPoolContributions(string poolId)
    {
        var contributions = await _idoService.GetPoolContributionsAsync(poolId);
        return Ok(contributions);
    }

    /// <summary>
    /// Claim tokens from a contribution
    /// </summary>
    [HttpPost("contributions/{contributionId}/claim")]
    public async Task<ActionResult<IDOContribution>> ClaimTokens(string contributionId, [FromBody] ClaimRequest request)
    {
        try
        {
            var contribution = await _idoService.ClaimTokensAsync(contributionId, request.UserAddress);
            return Ok(contribution);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Request refund for a contribution
    /// </summary>
    [HttpPost("contributions/{contributionId}/refund")]
    public async Task<ActionResult<IDOContribution>> Refund(string contributionId, [FromBody] ClaimRequest request)
    {
        try
        {
            var contribution = await _idoService.RefundAsync(contributionId, request.UserAddress);
            return Ok(contribution);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Add addresses to whitelist
    /// </summary>
    [HttpPost("pools/{poolId}/whitelist/add")]
    public async Task<ActionResult> AddToWhitelist(string poolId, [FromBody] WhitelistRequest request)
    {
        try
        {
            await _idoService.AddToWhitelistAsync(poolId, request.Addresses);
            return Ok(new { success = true, count = request.Addresses.Count() });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove addresses from whitelist
    /// </summary>
    [HttpPost("pools/{poolId}/whitelist/remove")]
    public async Task<ActionResult> RemoveFromWhitelist(string poolId, [FromBody] WhitelistRequest request)
    {
        try
        {
            await _idoService.RemoveFromWhitelistAsync(poolId, request.Addresses);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Check if address is whitelisted
    /// </summary>
    [HttpGet("pools/{poolId}/whitelist/check/{address}")]
    public async Task<ActionResult<bool>> CheckWhitelist(string poolId, string address)
    {
        try
        {
            var isWhitelisted = await _idoService.IsWhitelistedAsync(poolId, address);
            return Ok(new { isWhitelisted });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get pool statistics
    /// </summary>
    [HttpGet("pools/{poolId}/statistics")]
    public async Task<ActionResult<IDOPoolStatistics>> GetPoolStatistics(string poolId)
    {
        try
        {
            var stats = await _idoService.GetPoolStatisticsAsync(poolId);
            return Ok(stats);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get platform statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<IDOStatistics>> GetPlatformStatistics()
    {
        var stats = await _idoService.GetPlatformStatisticsAsync();
        return Ok(stats);
    }
}

// Request DTOs
public record ContributeRequest(string UserAddress, decimal Amount);
public record ClaimRequest(string UserAddress);
public record WhitelistRequest(IEnumerable<string> Addresses);
