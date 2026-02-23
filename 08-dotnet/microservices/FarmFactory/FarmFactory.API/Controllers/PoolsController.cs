using FarmFactory.Core.Interfaces;
using FarmFactory.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FarmFactory.API.Controllers;

/// <summary>
/// Farm pools - IERAHKWA FarmFactory
/// Staking & yield farming on ETH, BSC, Polygon, Aurora, xDai, IERAHKWA.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PoolsController : ControllerBase
{
    private readonly IFarmFactoryService _farm;
    private readonly ILogger<PoolsController> _log;

    public PoolsController(IFarmFactoryService farm, ILogger<PoolsController> log)
    {
        _farm = farm;
        _log = log;
    }

    /// <summary>List pools. Optional: network (ETH|BSC|POLYGON|AURORA|XDAI|IERAHKWA), activeOnly.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FarmPool>>> GetPools([FromQuery] string? network = null, [FromQuery] bool activeOnly = true)
    {
        var pools = await _farm.GetPoolsAsync(network, activeOnly);
        return Ok(pools);
    }

    /// <summary>Get pool by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FarmPool>> GetPool(Guid id)
    {
        var pool = await _farm.GetPoolAsync(id);
        if (pool == null) return NotFound();
        return Ok(pool);
    }

    /// <summary>Create pool (admin).</summary>
    [HttpPost]
    public async Task<ActionResult<FarmPool>> CreatePool([FromBody] CreatePoolRequest request)
    {
        var pool = await _farm.CreatePoolAsync(request);
        _log.LogInformation("IERAHKWA FarmFactory: Pool created {Id} {Name} on {Network}", pool.Id, pool.Name, pool.Network);
        return CreatedAtAction(nameof(GetPool), new { id = pool.Id }, pool);
    }
}
