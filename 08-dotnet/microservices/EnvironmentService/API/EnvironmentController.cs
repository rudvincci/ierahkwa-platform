using EnvironmentService.Domain;
using EnvironmentService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentService.API;

[ApiController]
[Route("api/[controller]")]
public class EcoZonesController : ControllerBase
{
    private readonly EnvironmentServiceDbContext _context;
    public EcoZonesController(EnvironmentServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EcoZone>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.EcoZones.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EcoZone>> GetById(Guid id)
    {
        var entity = await _context.EcoZones.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<EcoZone>> Create(EcoZone entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.EcoZones.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, EcoZone entity)
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
        var entity = await _context.EcoZones.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class WildlifeSpeciesController : ControllerBase
{
    private readonly EnvironmentServiceDbContext _context;
    public WildlifeSpeciesController(EnvironmentServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WildlifeSpecies>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.WildlifeSpecies.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WildlifeSpecies>> GetById(Guid id)
    {
        var entity = await _context.WildlifeSpecies.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<WildlifeSpecies>> Create(WildlifeSpecies entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.WildlifeSpecies.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, WildlifeSpecies entity)
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
        var entity = await _context.WildlifeSpecies.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class GeologicalSurveysController : ControllerBase
{
    private readonly EnvironmentServiceDbContext _context;
    public GeologicalSurveysController(EnvironmentServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GeologicalSurvey>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.GeologicalSurveys.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GeologicalSurvey>> GetById(Guid id)
    {
        var entity = await _context.GeologicalSurveys.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<GeologicalSurvey>> Create(GeologicalSurvey entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.GeologicalSurveys.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, GeologicalSurvey entity)
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
        var entity = await _context.GeologicalSurveys.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class WeatherStationsController : ControllerBase
{
    private readonly EnvironmentServiceDbContext _context;
    public WeatherStationsController(EnvironmentServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeatherStation>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.WeatherStations.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WeatherStation>> GetById(Guid id)
    {
        var entity = await _context.WeatherStations.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<WeatherStation>> Create(WeatherStation entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.WeatherStations.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, WeatherStation entity)
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
        var entity = await _context.WeatherStations.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class ReforestationProjectsController : ControllerBase
{
    private readonly EnvironmentServiceDbContext _context;
    public ReforestationProjectsController(EnvironmentServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReforestationProject>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.ReforestationProjects.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReforestationProject>> GetById(Guid id)
    {
        var entity = await _context.ReforestationProjects.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<ReforestationProject>> Create(ReforestationProject entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.ReforestationProjects.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ReforestationProject entity)
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
        var entity = await _context.ReforestationProjects.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
