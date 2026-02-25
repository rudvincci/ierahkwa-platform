using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.SocialWelfareService.Infrastructure;
using Ierahkwa.SocialWelfareService.Domain;

namespace Ierahkwa.SocialWelfareService.API;

[ApiController]
[Route("api/[controller]")]
public class UnemploymentController : ControllerBase
{
    private readonly SocialWelfareServiceDbContext _db;
    public UnemploymentController(SocialWelfareServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<UnemploymentClaim>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<UnemploymentClaim>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] UnemploymentClaim entity)
    { _db.Set<UnemploymentClaim>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] UnemploymentClaim entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<UnemploymentClaim>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<UnemploymentClaim>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
