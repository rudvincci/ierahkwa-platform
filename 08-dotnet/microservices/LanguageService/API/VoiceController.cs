using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.LanguageService.Infrastructure;
using Ierahkwa.LanguageService.Domain;

namespace Ierahkwa.LanguageService.API;

[ApiController]
[Route("api/[controller]")]
public class VoiceController : ControllerBase
{
    private readonly LanguageServiceDbContext _db;
    public VoiceController(LanguageServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<VoiceCommand>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<VoiceCommand>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] VoiceCommand entity)
    { _db.Set<VoiceCommand>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] VoiceCommand entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<VoiceCommand>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<VoiceCommand>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
