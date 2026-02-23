using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/cases")]
public class CasesController : ControllerBase
{
    private readonly DbService _db;

    public CasesController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List([FromQuery] int? clientId, [FromQuery] int? stageId)
    {
        var q = _db.Cases.AsEnumerable();
        if (clientId.HasValue) q = q.Where(c => c.ClientId == clientId.Value);
        if (stageId.HasValue) q = q.Where(c => c.CaseStageId == stageId.Value);
        var list = q.OrderByDescending(c => c.CreatedAt).Select(c => new
        {
            c.Id, c.Title, c.CaseNumber, c.ClientId, c.CourtId, c.CaseCategoryId, c.CaseStageId,
            c.Description, c.FilingDate, c.CreatedAt,
            clientName = _db.Clients.FirstOrDefault(x => x.Id == c.ClientId)?.Name,
            courtName = c.CourtId.HasValue ? _db.Courts.FirstOrDefault(x => x.Id == c.CourtId)?.Name : null,
            categoryName = c.CaseCategoryId.HasValue ? _db.CaseCategories.FirstOrDefault(x => x.Id == c.CaseCategoryId)?.Name : null,
            stageName = c.CaseStageId.HasValue ? _db.CaseStages.FirstOrDefault(x => x.Id == c.CaseStageId)?.Name : null
        });
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var c = _db.Cases.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        return Ok(new
        {
            c.Id, c.Title, c.CaseNumber, c.ClientId, c.CourtId, c.CaseCategoryId, c.CaseStageId,
            c.Description, c.FilingDate, c.CreatedAt,
            clientName = _db.Clients.FirstOrDefault(x => x.Id == c.ClientId)?.Name,
            courtName = c.CourtId.HasValue ? _db.Courts.FirstOrDefault(x => x.Id == c.CourtId)?.Name : null,
            categoryName = c.CaseCategoryId.HasValue ? _db.CaseCategories.FirstOrDefault(x => x.Id == c.CaseCategoryId)?.Name : null,
            stageName = c.CaseStageId.HasValue ? _db.CaseStages.FirstOrDefault(x => x.Id == c.CaseStageId)?.Name : null
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] Case body)
    {
        body.Id = _db.NextId("cases");
        body.CreatedAt = DateTime.UtcNow;
        _db.Cases.Add(body);
        _db.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = body.Id }, body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Case body)
    {
        var c = _db.Cases.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        c.Title = body.Title;
        c.CaseNumber = body.CaseNumber;
        c.ClientId = body.ClientId;
        c.CourtId = body.CourtId;
        c.CaseCategoryId = body.CaseCategoryId;
        c.CaseStageId = body.CaseStageId;
        c.Description = body.Description;
        c.FilingDate = body.FilingDate;
        _db.SaveChanges();
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var c = _db.Cases.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        _db.Cases.Remove(c);
        _db.SaveChanges();
        return NoContent();
    }
}
