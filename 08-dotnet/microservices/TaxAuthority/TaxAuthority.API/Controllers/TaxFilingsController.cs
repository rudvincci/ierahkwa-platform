using Microsoft.AspNetCore.Mvc;
using TaxAuthority.Core.Interfaces;
using TaxAuthority.Core.Models;

namespace TaxAuthority.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxFilingsController : ControllerBase
{
    private readonly ITaxFilingService _taxFilingService;

    public TaxFilingsController(ITaxFilingService taxFilingService)
    {
        _taxFilingService = taxFilingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaxFiling>>> GetAll()
    {
        var filings = await _taxFilingService.GetAllAsync();
        return Ok(filings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaxFiling>> GetById(string id)
    {
        var filing = await _taxFilingService.GetByIdAsync(id);
        if (filing == null) return NotFound();
        return Ok(filing);
    }

    [HttpGet("citizen/{citizenId}")]
    public async Task<ActionResult<IEnumerable<TaxFiling>>> GetByCitizen(string citizenId)
    {
        var filings = await _taxFilingService.GetByCitizenIdAsync(citizenId);
        return Ok(filings);
    }

    [HttpPost]
    public async Task<ActionResult<TaxFiling>> Create([FromBody] TaxFiling filing)
    {
        var created = await _taxFilingService.CreateAsync(filing);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TaxFiling filing)
    {
        if (id != filing.Id) return BadRequest();
        await _taxFilingService.UpdateAsync(filing);
        return NoContent();
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(string id)
    {
        await _taxFilingService.SubmitFilingAsync(id);
        return Ok(new { message = "Filing submitted successfully" });
    }

    [HttpGet("stats")]
    public async Task<ActionResult<TaxStats>> GetStats()
    {
        var stats = await _taxFilingService.GetStatsAsync();
        return Ok(stats);
    }
}
