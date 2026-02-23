using Microsoft.AspNetCore.Mvc;
using TradeX.Core.Interfaces;
using TradeX.Core.Models;

namespace TradeX.API.Controllers;

/// <summary>
/// Staking API - Ierahkwa TradeX
/// Earn passive income by locking assets
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StakingController : ControllerBase
{
    private readonly IStakingService _stakingService;
    private readonly ILogger<StakingController> _logger;
    
    public StakingController(IStakingService stakingService, ILogger<StakingController> logger)
    {
        _stakingService = stakingService;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all active staking pools
    /// </summary>
    [HttpGet("pools")]
    public async Task<ActionResult<IEnumerable<StakingPool>>> GetPools()
    {
        var pools = await _stakingService.GetActivePoolsAsync();
        return Ok(pools);
    }
    
    /// <summary>
    /// Get specific pool
    /// </summary>
    [HttpGet("pools/{id}")]
    public async Task<ActionResult<StakingPool>> GetPool(Guid id)
    {
        var pool = await _stakingService.GetPoolAsync(id);
        if (pool == null) return NotFound();
        return Ok(pool);
    }
    
    /// <summary>
    /// Stake tokens in a pool
    /// </summary>
    [HttpPost("stake")]
    public async Task<ActionResult<Stake>> Stake([FromBody] StakeRequest request)
    {
        try
        {
            var stake = await _stakingService.StakeAsync(request.UserId, request.PoolId, request.Amount);
            _logger.LogInformation("User {UserId} staked {Amount} in pool {PoolId}", 
                request.UserId, request.Amount, request.PoolId);
            return CreatedAtAction(nameof(GetUserStakes), new { userId = request.UserId }, stake);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Unstake tokens
    /// </summary>
    [HttpPost("unstake")]
    public async Task<ActionResult<Stake>> Unstake([FromBody] UnstakeRequest request)
    {
        try
        {
            var stake = await _stakingService.UnstakeAsync(request.StakeId, request.UserId, request.Early);
            return Ok(stake);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Claim staking rewards
    /// </summary>
    [HttpPost("claim")]
    public async Task<ActionResult<decimal>> ClaimRewards([FromBody] ClaimRequest request)
    {
        try
        {
            var amount = await _stakingService.ClaimRewardsAsync(request.StakeId, request.UserId);
            return Ok(new { claimedAmount = amount });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get user's stakes
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Stake>>> GetUserStakes(Guid userId)
    {
        var stakes = await _stakingService.GetUserStakesAsync(userId);
        return Ok(stakes);
    }
}

public record StakeRequest(Guid UserId, Guid PoolId, decimal Amount);
public record UnstakeRequest(Guid StakeId, Guid UserId, bool Early = false);
public record ClaimRequest(Guid StakeId, Guid UserId);
