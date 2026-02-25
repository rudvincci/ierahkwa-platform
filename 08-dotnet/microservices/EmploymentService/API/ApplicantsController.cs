using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.EmploymentService.Infrastructure;
using Ierahkwa.EmploymentService.Domain;

namespace Ierahkwa.EmploymentService.API;

[ApiController]
[Route("api/[controller]")]
public class ApplicantsController : ControllerBase
{
    private readonly EmploymentServiceDbContext _db;
    public ApplicantsController(EmploymentServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<Applicant>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<Applicant>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] Applicant entity)
    { _db.Set<Applicant>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] Applicant entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<Applicant>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<Applicant>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
