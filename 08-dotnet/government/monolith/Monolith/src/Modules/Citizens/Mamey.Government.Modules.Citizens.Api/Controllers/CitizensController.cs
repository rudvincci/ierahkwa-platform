using System;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Commands;
using Mamey.Government.Modules.Citizens.Core.DTO;
using Mamey.Government.Modules.Citizens.Core.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mamey.Government.Modules.Citizens.Api.Controllers;

[ApiController]
[Route("api/citizens")]
[Authorize]
public class CitizensController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public CitizensController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CitizenDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CitizenDto>> Get(Guid id)
    {
        var citizen = await _queryDispatcher.QueryAsync(new GetCitizen(id));
        if (citizen is null)
        {
            return NotFound();
        }
        return Ok(citizen);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [ProducesResponseType(typeof(PagedResult<CitizenSummaryDto>), 200)]
    public async Task<ActionResult<PagedResult<CitizenSummaryDto>>> Browse(
        [FromQuery] Guid tenantId,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _queryDispatcher.QueryAsync(new BrowseCitizens(
            tenantId, status, searchTerm, isActive, page, pageSize));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] CreateCitizenRequest request)
    {
        var citizenId = Guid.NewGuid();
        await _commandDispatcher.SendAsync(new CreateCitizen(
            citizenId,
            request.TenantId,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Email,
            request.Phone,
            request.Street,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.PhotoUrl,
            request.ApplicationId));
        
        return CreatedAtAction(nameof(Get), new { id = citizenId }, null);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCitizenRequest request)
    {
        await _commandDispatcher.SendAsync(new UpdateCitizen(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Street,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.PhotoUrl));
        
        return NoContent();
    }

    [HttpPost("{id:guid}/progress-status")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ProgressStatus(Guid id, [FromBody] ProgressStatusRequest request)
    {
        await _commandDispatcher.SendAsync(new ProgressCitizenStatus(
            id,
            request.NewStatus,
            request.ApprovedBy,
            request.Reason));
        
        return NoContent();
    }

    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Deactivate(Guid id, [FromBody] DeactivateCitizenRequest request)
    {
        await _commandDispatcher.SendAsync(new DeactivateCitizen(
            id,
            request.Reason,
            request.DeactivatedBy));
        
        return NoContent();
    }
}

public record CreateCitizenRequest(
    Guid TenantId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string? Email,
    string? Phone,
    string? Street,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    string? PhotoUrl,
    Guid? ApplicationId = null);

public record UpdateCitizenRequest(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Street,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    string? PhotoUrl);

public record ProgressStatusRequest(
    string NewStatus,
    string ApprovedBy,
    string? Reason);

public record DeactivateCitizenRequest(
    string Reason,
    string DeactivatedBy);
