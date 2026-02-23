using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/courts")]
public class CourtsController : ControllerBase
{
    private readonly DbService _db;

    public CourtsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Courts.OrderByDescending(c => c.CreatedAt));

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var c = _db.Courts.FirstOrDefault(x => x.Id == id);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Court body)
    {
        body.Id = _db.NextId("courts");
        body.CreatedAt = DateTime.UtcNow;
        _db.Courts.Add(body);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = body.Id }, body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Court body)
    {
        var c = _db.Courts.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        c.Name = body.Name;
        c.Location = body.Location;
        _db.SaveChanges();
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var c = _db.Courts.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        _db.Courts.Remove(c);
        _db.SaveChanges();
        return NoContent();
    }
}
