using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.EmploymentService.Infrastructure;
using Ierahkwa.EmploymentService.Domain;

namespace Ierahkwa.EmploymentService.API;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly EmploymentServiceDbContext _db;
    public JobsController(EmploymentServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<JobPosting>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<JobPosting>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] JobPosting entity)
    { _db.Set<JobPosting>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] JobPosting entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<JobPosting>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<JobPosting>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
