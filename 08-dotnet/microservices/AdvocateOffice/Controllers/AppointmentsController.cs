using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly DbService _db;

    public AppointmentsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List([FromQuery] int? clientId, [FromQuery] int? caseId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var q = _db.Appointments.AsEnumerable();
        if (clientId.HasValue) q = q.Where(a => a.ClientId == clientId);
        if (caseId.HasValue) q = q.Where(a => a.CaseId == caseId);
        if (from.HasValue) q = q.Where(a => a.StartAt >= from.Value);
        if (to.HasValue) q = q.Where(a => a.StartAt <= to.Value);
        return Ok(q.OrderBy(a => a.StartAt));
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var a = _db.Appointments.FirstOrDefault(x => x.Id == id);
        return a == null ? NotFound() : Ok(a);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Appointment body)
    {
        body.Id = _db.NextId("appointments");
        body.CreatedAt = DateTime.UtcNow;
        _db.Appointments.Add(body);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = body.Id }, body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Appointment body)
    {
        var a = _db.Appointments.FirstOrDefault(x => x.Id == id);
        if (a == null) return NotFound();
        a.Title = body.Title;
        a.ClientId = body.ClientId;
        a.CaseId = body.CaseId;
        a.StartAt = body.StartAt;
        a.EndAt = body.EndAt;
        a.Location = body.Location;
        a.Notes = body.Notes;
        a.Status = body.Status;
        _db.SaveChanges();
        return Ok(a);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var a = _db.Appointments.FirstOrDefault(x => x.Id == id);
        if (a == null) return NotFound();
        _db.Appointments.Remove(a);
        _db.SaveChanges();
        return NoContent();
    }
}
