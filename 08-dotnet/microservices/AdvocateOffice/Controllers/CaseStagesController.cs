using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/case-stages")]
public class CaseStagesController : ControllerBase
{
    private readonly DbService _db;

    public CaseStagesController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.CaseStages.OrderBy(c => c.Name));

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var c = _db.CaseStages.FirstOrDefault(x => x.Id == id);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CaseStage body)
    {
        body.Id = _db.NextId("caseStages");
        body.CreatedAt = DateTime.UtcNow;
        _db.CaseStages.Add(body);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = body.Id }, body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CaseStage body)
    {
        var c = _db.CaseStages.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        c.Name = body.Name;
        _db.SaveChanges();
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var c = _db.CaseStages.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        _db.CaseStages.Remove(c);
        _db.SaveChanges();
        return NoContent();
    }
}
