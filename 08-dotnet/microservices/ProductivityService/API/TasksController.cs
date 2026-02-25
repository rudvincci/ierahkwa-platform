using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.ProductivityService.Infrastructure;
using Ierahkwa.ProductivityService.Domain;

namespace Ierahkwa.ProductivityService.API;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ProductivityServiceDbContext _db;
    public TasksController(ProductivityServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<ProductivityTask>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<ProductivityTask>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] ProductivityTask entity)
    { _db.Set<ProductivityTask>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] ProductivityTask entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<ProductivityTask>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<ProductivityTask>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
