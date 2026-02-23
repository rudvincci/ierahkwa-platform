using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models;

namespace NET10.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoolController : ControllerBase
{
    private readonly IPoolService _poolService;

    public PoolController(IPoolService poolService)
    {
        _poolService = poolService;
    }

    /// <summary>
    /// Get all liquidity pools
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<LiquidityPool>>> GetAllPools()
    {
        var pools = await _poolService.GetAllPoolsAsync();
        return Ok(pools);
    }

    /// <summary>
    /// Get pool by ID
    /// </summary>
    [HttpGet("{poolId}")]
    public async Task<ActionResult<LiquidityPool>> GetPool(string poolId)
    {
        var pool = await _poolService.GetPoolByIdAsync(poolId);
        if (pool == null)
            return NotFound();
        return Ok(pool);
    }

    /// <summary>
    /// Get pool by token pair
    /// </summary>
    [HttpGet("pair")]
    public async Task<ActionResult<LiquidityPool>> GetPoolByPair(
        [FromQuery] string token0, 
        [FromQuery] string token1)
    {
        var pool = await _poolService.GetPoolByPairAsync(token0, token1);
        if (pool == null)
            return NotFound();
        return Ok(pool);
    }

    /// <summary>
    /// Create new liquidity pool
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LiquidityPool>> CreatePool([FromBody] CreatePoolRequest request)
    {
        var pool = await _poolService.CreatePoolAsync(request.Token0Id, request.Token1Id, request.Fee);
        return CreatedAtAction(nameof(GetPool), new { poolId = pool.Id }, pool);
    }

    /// <summary>
    /// Add liquidity to pool
    /// </summary>
    [HttpPost("add-liquidity")]
    public async Task<ActionResult<LiquidityTransaction>> AddLiquidity([FromBody] LiquidityRequest request)
    {
        try
        {
            request.Action = LiquidityAction.Add;
            var tx = await _poolService.AddLiquidityAsync(request);
            return Ok(tx);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove liquidity from pool
    /// </summary>
    [HttpPost("remove-liquidity")]
    public async Task<ActionResult<LiquidityTransaction>> RemoveLiquidity([FromBody] LiquidityRequest request)
    {
        try
        {
            request.Action = LiquidityAction.Remove;
            var tx = await _poolService.RemoveLiquidityAsync(request);
            return Ok(tx);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Calculate amounts for adding liquidity
    /// </summary>
    [HttpGet("{poolId}/calculate-add")]
    public async Task<ActionResult> CalculateAddLiquidity(string poolId, [FromQuery] decimal token0Amount)
    {
        var (t0, t1) = await _poolService.CalculateAddLiquidityAsync(poolId, token0Amount);
        return Ok(new { token0Amount = t0, token1Amount = t1 });
    }

    /// <summary>
    /// Calculate amounts for removing liquidity
    /// </summary>
    [HttpGet("{poolId}/calculate-remove")]
    public async Task<ActionResult> CalculateRemoveLiquidity(string poolId, [FromQuery] decimal lpTokenAmount)
    {
        var (t0, t1) = await _poolService.CalculateRemoveLiquidityAsync(poolId, lpTokenAmount);
        return Ok(new { token0Amount = t0, token1Amount = t1 });
    }

    /// <summary>
    /// Get user's liquidity positions
    /// </summary>
    [HttpGet("positions/{userId}")]
    public async Task<ActionResult<List<LiquidityPosition>>> GetUserPositions(string userId)
    {
        var positions = await _poolService.GetUserPositionsAsync(userId);
        return Ok(positions);
    }

    /// <summary>
    /// Get user's position in specific pool
    /// </summary>
    [HttpGet("{poolId}/position/{userId}")]
    public async Task<ActionResult<LiquidityPosition>> GetUserPosition(string poolId, string userId)
    {
        var position = await _poolService.GetUserPositionAsync(userId, poolId);
        if (position == null)
            return NotFound();
        return Ok(position);
    }

    /// <summary>
    /// Get user's liquidity history
    /// </summary>
    [HttpGet("history/{userId}")]
    public async Task<ActionResult<List<LiquidityTransaction>>> GetUserHistory(string userId, [FromQuery] int limit = 50)
    {
        var history = await _poolService.GetUserLiquidityHistoryAsync(userId, limit);
        return Ok(history);
    }

    /// <summary>
    /// Get top pools by TVL
    /// </summary>
    [HttpGet("top/tvl")]
    public async Task<ActionResult<List<LiquidityPool>>> GetTopByTVL([FromQuery] int limit = 10)
    {
        var pools = await _poolService.GetTopPoolsByTVLAsync(limit);
        return Ok(pools);
    }

    /// <summary>
    /// Get top pools by volume
    /// </summary>
    [HttpGet("top/volume")]
    public async Task<ActionResult<List<LiquidityPool>>> GetTopByVolume([FromQuery] int limit = 10)
    {
        var pools = await _poolService.GetTopPoolsByVolumeAsync(limit);
        return Ok(pools);
    }
}

public class CreatePoolRequest
{
    public string Token0Id { get; set; } = string.Empty;
    public string Token1Id { get; set; } = string.Empty;
    public decimal Fee { get; set; } = 0.003m;
}
