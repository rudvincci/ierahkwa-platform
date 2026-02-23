using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models;

namespace NET10.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FarmController : ControllerBase
{
    private readonly IFarmService _farmService;

    public FarmController(IFarmService farmService)
    {
        _farmService = farmService;
    }

    /// <summary>
    /// Get all farms
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Farm>>> GetAllFarms()
    {
        var farms = await _farmService.GetAllFarmsAsync();
        return Ok(farms);
    }

    /// <summary>
    /// Get active farms only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<Farm>>> GetActiveFarms()
    {
        var farms = await _farmService.GetActiveFarmsAsync();
        return Ok(farms);
    }

    /// <summary>
    /// Get farm by ID
    /// </summary>
    [HttpGet("{farmId}")]
    public async Task<ActionResult<Farm>> GetFarm(string farmId)
    {
        var farm = await _farmService.GetFarmByIdAsync(farmId);
        if (farm == null)
            return NotFound();
        return Ok(farm);
    }

    /// <summary>
    /// Create new farm (admin only)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Farm>> CreateFarm([FromBody] Farm farm)
    {
        var created = await _farmService.CreateFarmAsync(farm);
        return CreatedAtAction(nameof(GetFarm), new { farmId = created.Id }, created);
    }

    /// <summary>
    /// Update farm (admin only)
    /// </summary>
    [HttpPut("{farmId}")]
    public async Task<ActionResult<Farm>> UpdateFarm(string farmId, [FromBody] Farm farm)
    {
        farm.Id = farmId;
        var updated = await _farmService.UpdateFarmAsync(farm);
        return Ok(updated);
    }

    /// <summary>
    /// End farm (admin only)
    /// </summary>
    [HttpPost("{farmId}/end")]
    public async Task<ActionResult> EndFarm(string farmId)
    {
        var success = await _farmService.EndFarmAsync(farmId);
        if (!success)
            return NotFound();
        return Ok(new { message = "Farm ended successfully" });
    }

    /// <summary>
    /// Stake tokens in farm
    /// </summary>
    [HttpPost("stake")]
    public async Task<ActionResult<FarmTransaction>> Stake([FromBody] FarmActionRequest request)
    {
        try
        {
            request.Action = FarmAction.Stake;
            var tx = await _farmService.StakeAsync(request);
            return Ok(tx);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Unstake tokens from farm
    /// </summary>
    [HttpPost("unstake")]
    public async Task<ActionResult<FarmTransaction>> Unstake([FromBody] FarmActionRequest request)
    {
        try
        {
            request.Action = FarmAction.Unstake;
            var tx = await _farmService.UnstakeAsync(request);
            return Ok(tx);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Harvest rewards
    /// </summary>
    [HttpPost("harvest")]
    public async Task<ActionResult<FarmTransaction>> Harvest([FromBody] FarmActionRequest request)
    {
        try
        {
            request.Action = FarmAction.Harvest;
            var tx = await _farmService.HarvestAsync(request);
            return Ok(tx);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Compound rewards (harvest + restake)
    /// </summary>
    [HttpPost("compound")]
    public async Task<ActionResult<FarmTransaction>> Compound([FromBody] FarmActionRequest request)
    {
        try
        {
            request.Action = FarmAction.Compound;
            var tx = await _farmService.CompoundAsync(request);
            return Ok(tx);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get user's farming positions
    /// </summary>
    [HttpGet("positions/{userId}")]
    public async Task<ActionResult<List<FarmPosition>>> GetUserPositions(string userId)
    {
        var positions = await _farmService.GetUserPositionsAsync(userId);
        return Ok(positions);
    }

    /// <summary>
    /// Get user's position in specific farm
    /// </summary>
    [HttpGet("{farmId}/position/{userId}")]
    public async Task<ActionResult<FarmPosition>> GetUserPosition(string farmId, string userId)
    {
        var position = await _farmService.GetUserPositionAsync(userId, farmId);
        if (position == null)
            return NotFound();
        return Ok(position);
    }

    /// <summary>
    /// Calculate pending rewards
    /// </summary>
    [HttpGet("{farmId}/pending/{userId}")]
    public async Task<ActionResult> GetPendingRewards(string farmId, string userId)
    {
        var rewards = await _farmService.CalculatePendingRewardsAsync(userId, farmId);
        return Ok(new { farmId, userId, pendingRewards = rewards });
    }

    /// <summary>
    /// Get user's farm history
    /// </summary>
    [HttpGet("history/{userId}")]
    public async Task<ActionResult<List<FarmTransaction>>> GetUserHistory(string userId, [FromQuery] int limit = 50)
    {
        var history = await _farmService.GetUserFarmHistoryAsync(userId, limit);
        return Ok(history);
    }

    /// <summary>
    /// Get top farms by APR
    /// </summary>
    [HttpGet("top/apr")]
    public async Task<ActionResult<List<Farm>>> GetTopByAPR([FromQuery] int limit = 10)
    {
        var farms = await _farmService.GetTopFarmsByAPRAsync(limit);
        return Ok(farms);
    }

    /// <summary>
    /// Get top farms by TVL
    /// </summary>
    [HttpGet("top/tvl")]
    public async Task<ActionResult<List<Farm>>> GetTopByTVL([FromQuery] int limit = 10)
    {
        var farms = await _farmService.GetTopFarmsByTVLAsync(limit);
        return Ok(farms);
    }
}
