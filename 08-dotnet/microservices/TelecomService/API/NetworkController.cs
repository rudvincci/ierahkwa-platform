using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.TelecomService.Infrastructure;
using Ierahkwa.TelecomService.Domain;

namespace Ierahkwa.TelecomService.API;

[ApiController]
[Route("api/[controller]")]
public class NetworkController : ControllerBase
{
    private readonly TelecomServiceDbContext _db;
    public NetworkController(TelecomServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<NetworkNode>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<NetworkNode>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] NetworkNode entity)
    { _db.Set<NetworkNode>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] NetworkNode entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<NetworkNode>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<NetworkNode>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
