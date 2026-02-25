using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.AIEngineService.Infrastructure;
using Ierahkwa.AIEngineService.Domain;

namespace Ierahkwa.AIEngineService.API;

[ApiController]
[Route("api/[controller]")]
public class InferenceController : ControllerBase
{
    private readonly AIEngineServiceDbContext _db;
    public InferenceController(AIEngineServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<InferenceRequest>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<InferenceRequest>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] InferenceRequest entity)
    { _db.Set<InferenceRequest>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] InferenceRequest entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<InferenceRequest>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<InferenceRequest>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
