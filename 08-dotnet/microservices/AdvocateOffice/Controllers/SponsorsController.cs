using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/sponsors")]
public class SponsorsController : ControllerBase
{
    private readonly DbService _db;

    public SponsorsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Sponsors.OrderByDescending(s => s.CreatedAt));

    [HttpPost]
    public IActionResult Create([FromBody] Sponsor body)
    {
        body.Id = _db.NextId("sponsors");
        body.CreatedAt = DateTime.UtcNow;
        _db.Sponsors.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var s = _db.Sponsors.FirstOrDefault(x => x.Id == id);
        if (s == null) return NotFound();
        _db.Sponsors.Remove(s);
        _db.SaveChanges();
        return NoContent();
    }
}
