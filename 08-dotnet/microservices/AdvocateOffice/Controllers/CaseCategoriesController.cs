using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/case-categories")]
public class CaseCategoriesController : ControllerBase
{
    private readonly DbService _db;

    public CaseCategoriesController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.CaseCategories.OrderBy(c => c.Name));

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var c = _db.CaseCategories.FirstOrDefault(x => x.Id == id);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CaseCategory body)
    {
        body.Id = _db.NextId("caseCategories");
        body.CreatedAt = DateTime.UtcNow;
        _db.CaseCategories.Add(body);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = body.Id }, body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CaseCategory body)
    {
        var c = _db.CaseCategories.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        c.Name = body.Name;
        _db.SaveChanges();
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var c = _db.CaseCategories.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        _db.CaseCategories.Remove(c);
        _db.SaveChanges();
        return NoContent();
    }
}
