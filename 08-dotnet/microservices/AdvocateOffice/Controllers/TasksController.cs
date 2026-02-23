using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly DbService _db;

    public TasksController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List([FromQuery] int? caseId, [FromQuery] string? status)
    {
        var q = _db.Tasks.AsEnumerable();
        if (caseId.HasValue) q = q.Where(t => t.CaseId == caseId.Value);
        if (!string.IsNullOrEmpty(status)) q = q.Where(t => t.Status == status);
        return Ok(q.OrderByDescending(t => t.CreatedAt));
    }

    [HttpPost]
    public IActionResult Create([FromBody] CaseTask body)
    {
        body.Id = _db.NextId("tasks");
        body.CreatedAt = DateTime.UtcNow;
        _db.Tasks.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CaseTask body)
    {
        var t = _db.Tasks.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        t.Title = body.Title;
        t.Description = body.Description;
        t.CaseId = body.CaseId;
        t.Status = body.Status;
        t.DueDate = body.DueDate;
        _db.SaveChanges();
        return Ok(t);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var t = _db.Tasks.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        _db.Tasks.Remove(t);
        _db.SaveChanges();
        return NoContent();
    }
}
