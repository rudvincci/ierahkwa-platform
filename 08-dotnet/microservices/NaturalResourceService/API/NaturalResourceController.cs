using NaturalResourceService.Domain;
using NaturalResourceService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NaturalResourceService.API;

[ApiController]
[Route("api/[controller]")]
public class WaterSourcesController : ControllerBase
{
    private readonly NaturalResourceServiceDbContext _context;
    public WaterSourcesController(NaturalResourceServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WaterSource>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.WaterSources.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WaterSource>> GetById(Guid id)
    {
        var entity = await _context.WaterSources.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<WaterSource>> Create(WaterSource entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.WaterSources.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, WaterSource entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.WaterSources.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class EnergyPlantsController : ControllerBase
{
    private readonly NaturalResourceServiceDbContext _context;
    public EnergyPlantsController(NaturalResourceServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnergyPlant>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.EnergyPlants.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnergyPlant>> GetById(Guid id)
    {
        var entity = await _context.EnergyPlants.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<EnergyPlant>> Create(EnergyPlant entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.EnergyPlants.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, EnergyPlant entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.EnergyPlants.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class NuclearFacilitiesController : ControllerBase
{
    private readonly NaturalResourceServiceDbContext _context;
    public NuclearFacilitiesController(NaturalResourceServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NuclearFacility>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.NuclearFacilities.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NuclearFacility>> GetById(Guid id)
    {
        var entity = await _context.NuclearFacilities.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<NuclearFacility>> Create(NuclearFacility entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.NuclearFacilities.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, NuclearFacility entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.NuclearFacilities.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class MiningOperationsController : ControllerBase
{
    private readonly NaturalResourceServiceDbContext _context;
    public MiningOperationsController(NaturalResourceServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MiningOperation>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.MiningOperations.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MiningOperation>> GetById(Guid id)
    {
        var entity = await _context.MiningOperations.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<MiningOperation>> Create(MiningOperation entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.MiningOperations.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, MiningOperation entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.MiningOperations.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ResourceReservesController : ControllerBase
{
    private readonly NaturalResourceServiceDbContext _context;
    public ResourceReservesController(NaturalResourceServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ResourceReserve>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.ResourceReserves.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResourceReserve>> GetById(Guid id)
    {
        var entity = await _context.ResourceReserves.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<ResourceReserve>> Create(ResourceReserve entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.ResourceReserves.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ResourceReserve entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.ResourceReserves.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
