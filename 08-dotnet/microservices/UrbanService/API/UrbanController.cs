using UrbanService.Domain;
using UrbanService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UrbanService.API;

[ApiController]
[Route("api/[controller]")]
public class LandParcelsController : ControllerBase
{
    private readonly UrbanServiceDbContext _context;
    public LandParcelsController(UrbanServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LandParcel>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.LandParcels.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LandParcel>> GetById(Guid id)
    {
        var entity = await _context.LandParcels.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<LandParcel>> Create(LandParcel entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.LandParcels.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, LandParcel entity)
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
        var entity = await _context.LandParcels.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class UrbanPlansController : ControllerBase
{
    private readonly UrbanServiceDbContext _context;
    public UrbanPlansController(UrbanServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UrbanPlan>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.UrbanPlans.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UrbanPlan>> GetById(Guid id)
    {
        var entity = await _context.UrbanPlans.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<UrbanPlan>> Create(UrbanPlan entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.UrbanPlans.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UrbanPlan entity)
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
        var entity = await _context.UrbanPlans.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class HousingUnitsController : ControllerBase
{
    private readonly UrbanServiceDbContext _context;
    public HousingUnitsController(UrbanServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HousingUnit>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.HousingUnits.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HousingUnit>> GetById(Guid id)
    {
        var entity = await _context.HousingUnits.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<HousingUnit>> Create(HousingUnit entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.HousingUnits.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, HousingUnit entity)
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
        var entity = await _context.HousingUnits.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ZoningRegulationsController : ControllerBase
{
    private readonly UrbanServiceDbContext _context;
    public ZoningRegulationsController(UrbanServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ZoningRegulation>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.ZoningRegulations.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ZoningRegulation>> GetById(Guid id)
    {
        var entity = await _context.ZoningRegulations.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<ZoningRegulation>> Create(ZoningRegulation entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.ZoningRegulations.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ZoningRegulation entity)
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
        var entity = await _context.ZoningRegulations.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ConstructionPermitsController : ControllerBase
{
    private readonly UrbanServiceDbContext _context;
    public ConstructionPermitsController(UrbanServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConstructionPermit>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.ConstructionPermits.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConstructionPermit>> GetById(Guid id)
    {
        var entity = await _context.ConstructionPermits.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<ConstructionPermit>> Create(ConstructionPermit entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.ConstructionPermits.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ConstructionPermit entity)
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
        var entity = await _context.ConstructionPermits.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
