using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.InsuranceService.Infrastructure;
using Ierahkwa.InsuranceService.Domain;

namespace Ierahkwa.InsuranceService.API;

[ApiController]
[Route("api/[controller]")]
public class BeneficiariesController : ControllerBase
{
    private readonly InsuranceServiceDbContext _db;
    public BeneficiariesController(InsuranceServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<Beneficiary>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<Beneficiary>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] Beneficiary entity)
    { _db.Set<Beneficiary>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] Beneficiary entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<Beneficiary>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<Beneficiary>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
