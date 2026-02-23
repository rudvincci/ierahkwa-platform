using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly DbService _db;

    public ClientsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Clients.OrderByDescending(c => c.CreatedAt));

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var c = _db.Clients.FirstOrDefault(x => x.Id == id);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Client body)
    {
        body.Id = _db.NextId("clients");
        body.CreatedAt = DateTime.UtcNow;
        _db.Clients.Add(body);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = body.Id }, body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Client body)
    {
        var c = _db.Clients.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        c.Name = body.Name;
        c.Email = body.Email;
        c.Phone = body.Phone;
        c.Address = body.Address;
        _db.SaveChanges();
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var c = _db.Clients.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        _db.Clients.Remove(c);
        _db.SaveChanges();
        return NoContent();
    }
}
