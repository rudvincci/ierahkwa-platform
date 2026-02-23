using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly DbService _db;

    public DocumentsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List([FromQuery] int? caseId)
    {
        var q = caseId.HasValue ? _db.Documents.Where(d => d.CaseId == caseId.Value) : _db.Documents.AsEnumerable();
        return Ok(q.OrderByDescending(d => d.CreatedAt));
    }

    [HttpPost]
    public IActionResult Create([FromBody] Document body)
    {
        body.Id = _db.NextId("documents");
        body.CreatedAt = DateTime.UtcNow;
        _db.Documents.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var d = _db.Documents.FirstOrDefault(x => x.Id == id);
        if (d == null) return NotFound();
        _db.Documents.Remove(d);
        _db.SaveChanges();
        return NoContent();
    }
}
