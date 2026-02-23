using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/updates")]
public class UpdatesController : ControllerBase
{
    private readonly DbService _db;

    public UpdatesController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Updates.OrderByDescending(u => u.CreatedAt));

    [HttpPost]
    public IActionResult Create([FromBody] Update body)
    {
        body.Id = _db.NextId("updates");
        body.CreatedAt = DateTime.UtcNow;
        _db.Updates.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var u = _db.Updates.FirstOrDefault(x => x.Id == id);
        if (u == null) return NotFound();
        _db.Updates.Remove(u);
        _db.SaveChanges();
        return NoContent();
    }
}
