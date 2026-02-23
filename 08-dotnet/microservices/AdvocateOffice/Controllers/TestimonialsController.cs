using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/testimonials")]
public class TestimonialsController : ControllerBase
{
    private readonly DbService _db;

    public TestimonialsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Testimonials.OrderByDescending(t => t.CreatedAt));

    [HttpPost]
    public IActionResult Create([FromBody] Testimonial body)
    {
        body.Id = _db.NextId("testimonials");
        body.CreatedAt = DateTime.UtcNow;
        _db.Testimonials.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var t = _db.Testimonials.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        _db.Testimonials.Remove(t);
        _db.SaveChanges();
        return NoContent();
    }
}
