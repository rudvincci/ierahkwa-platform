using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.InsuranceService.Infrastructure;
using Ierahkwa.InsuranceService.Domain;

namespace Ierahkwa.InsuranceService.API;

[ApiController]
[Route("api/[controller]")]
public class PensionsController : ControllerBase
{
    private readonly InsuranceServiceDbContext _db;
    public PensionsController(InsuranceServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<PensionAccount>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<PensionAccount>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] PensionAccount entity)
    { _db.Set<PensionAccount>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] PensionAccount entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<PensionAccount>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<PensionAccount>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
