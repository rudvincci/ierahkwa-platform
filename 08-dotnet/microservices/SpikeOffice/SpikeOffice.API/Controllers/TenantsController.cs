using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpikeOffice.Infrastructure.Data;

namespace SpikeOffice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly SpikeOfficeDbContext _db;

    public TenantsController(SpikeOfficeDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var list = await _db.Tenants.IgnoreQueryFilters()
            .Where(t => t.IsActive)
            .Select(t => new { t.Id, t.Name, t.UrlPrefix, t.LogoUrl, t.IerahkwaDepartmentCode, t.IgtTokenSymbol })
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var t = await _db.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (t == null) return NotFound();
        return Ok(new { t.Id, t.Name, t.UrlPrefix, t.LogoUrl, t.DefaultLanguage, t.IerahkwaDepartmentCode, t.IgtTokenSymbol });
    }
}
