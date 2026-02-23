using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly DbService _db;

    public ProductsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List([FromQuery] string? type)
    {
        var q = string.IsNullOrEmpty(type) ? _db.Products.AsEnumerable() : _db.Products.Where(p => (p.Type ?? "").Equals(type, StringComparison.OrdinalIgnoreCase));
        return Ok(q.OrderByDescending(p => p.CreatedAt));
    }

    [HttpPost]
    public IActionResult Create([FromBody] Product body)
    {
        body.Id = _db.NextId("products");
        body.CreatedAt = DateTime.UtcNow;
        _db.Products.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Product body)
    {
        var p = _db.Products.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        p.Name = body.Name;
        p.Type = body.Type;
        p.Price = body.Price;
        _db.SaveChanges();
        return Ok(p);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var p = _db.Products.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        _db.Products.Remove(p);
        _db.SaveChanges();
        return NoContent();
    }
}
