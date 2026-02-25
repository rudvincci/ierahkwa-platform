using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.SmartFactoryService.Infrastructure;
using Ierahkwa.SmartFactoryService.Domain;

namespace Ierahkwa.SmartFactoryService.API;

[ApiController]
[Route("api/[controller]")]
public class QualityController : ControllerBase
{
    private readonly SmartFactoryServiceDbContext _db;
    public QualityController(SmartFactoryServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<QualityCheck>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<QualityCheck>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] QualityCheck entity)
    { _db.Set<QualityCheck>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] QualityCheck entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<QualityCheck>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<QualityCheck>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
