using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.QuantumService.Infrastructure;
using Ierahkwa.QuantumService.Domain;

namespace Ierahkwa.QuantumService.API;

[ApiController]
[Route("api/[controller]")]
public class CircuitsController : ControllerBase
{
    private readonly QuantumServiceDbContext _db;
    public CircuitsController(QuantumServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<QuantumCircuit>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<QuantumCircuit>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] QuantumCircuit entity)
    { _db.Set<QuantumCircuit>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] QuantumCircuit entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<QuantumCircuit>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<QuantumCircuit>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
