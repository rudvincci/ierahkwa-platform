using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/vendors")]
public class VendorsController : ControllerBase
{
    private readonly DbService _db;

    public VendorsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Vendors.OrderByDescending(v => v.CreatedAt));

    [HttpPost]
    public IActionResult Create([FromBody] Vendor body)
    {
        body.Id = _db.NextId("vendors");
        body.CreatedAt = DateTime.UtcNow;
        _db.Vendors.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Vendor body)
    {
        var v = _db.Vendors.FirstOrDefault(x => x.Id == id);
        if (v == null) return NotFound();
        v.Name = body.Name;
        v.Email = body.Email;
        v.Phone = body.Phone;
        _db.SaveChanges();
        return Ok(v);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var v = _db.Vendors.FirstOrDefault(x => x.Id == id);
        if (v == null) return NotFound();
        _db.Vendors.Remove(v);
        _db.SaveChanges();
        return NoContent();
    }
}
