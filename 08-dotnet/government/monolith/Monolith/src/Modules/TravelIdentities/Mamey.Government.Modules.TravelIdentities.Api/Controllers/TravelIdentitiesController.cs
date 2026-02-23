using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.Commands;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;
using Mamey.Government.Modules.TravelIdentities.Core.Queries;
using Mamey.MicroMonolith.Abstractions;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.Government.Modules.TravelIdentities.Api.Controllers;

[Authorize]
internal class TravelIdentitiesController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IContext _context;

    public TravelIdentitiesController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IContext context)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _context = context;
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get travel identity by ID")]
    [ProducesResponseType(typeof(TravelIdentityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TravelIdentityDto>> GetAsync(Guid id)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetTravelIdentity { TravelIdentityId = id }));

    [HttpGet("by-number/{idNumber}")]
    [SwaggerOperation("Get travel identity by ID number")]
    [ProducesResponseType(typeof(TravelIdentityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TravelIdentityDto>> GetByNumberAsync(string idNumber)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetTravelIdentityByNumber { TravelIdentityNumber = idNumber }));

    [HttpGet]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Browse travel identities")]
    [ProducesResponseType(typeof(PagedResult<TravelIdentitySummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TravelIdentitySummaryDto>>> BrowseAsync(
        [FromQuery] Guid tenantId,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new BrowseTravelIdentities
        {
            TenantId = tenantId,
            Status = status,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };
        return Ok(await _queryDispatcher.QueryAsync(query));
    }

    [HttpGet("by-citizen/{citizenId:guid}")]
    [SwaggerOperation("Get travel identities for a citizen")]
    [ProducesResponseType(typeof(IEnumerable<TravelIdentitySummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TravelIdentitySummaryDto>>> GetByCitizenAsync(Guid citizenId)
    {
        var travelIdentities = await _queryDispatcher.QueryAsync(new GetTravelIdentitiesByCitizen { CitizenId = citizenId });
        return Ok(travelIdentities);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Issue new travel identity")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IssueAsync([FromBody] IssueTravelIdentityRequest request)
    {
        var command = new IssueTravelIdentity(
            request.CitizenId,
            request.TenantId,
            request.ValidityYears);

        await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetAsync), new { id = command.Id }, new { TravelIdentityId = command.Id });
    }

    [HttpPost("{id:guid}/renew")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Renew travel identity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenewAsync(Guid id, [FromBody] RenewTravelIdentityRequest? request = null)
    {
        await _commandDispatcher.SendAsync(new RenewTravelIdentity(id, request?.ValidityYears ?? 8));
        return Ok(new { Message = "Travel identity renewed successfully" });
    }

    [HttpPost("{id:guid}/revoke")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Revoke travel identity")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeAsync(Guid id, [FromBody] RevokeTravelIdentityRequest request)
    {
        var revokedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new RevokeTravelIdentity(id, request.Reason, revokedBy));
        return NoContent();
    }

    [HttpPost("validate")]
    [AllowAnonymous]
    [SwaggerOperation("Publicly validate a travel identity")]
    [ProducesResponseType(typeof(TravelIdentityValidationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TravelIdentityValidationResultDto>> ValidateAsync([FromBody] ValidateTravelIdentityRequest request)
    {
        var result = await _queryDispatcher.QueryAsync(new ValidateTravelIdentity 
        { 
            IdNumber = request.IdNumber,
            DateOfBirth = request.DateOfBirth,
            LastName = request.LastName
        });
        return Ok(result);
    }
}

// Request DTOs
public record IssueTravelIdentityRequest(
    Guid CitizenId,
    Guid TenantId,
    int ValidityYears = 8);

public record RenewTravelIdentityRequest(int ValidityYears = 8);

public record RevokeTravelIdentityRequest(string Reason);

public record ValidateTravelIdentityRequest(
    string IdNumber,
    string? DateOfBirth = null,
    string? LastName = null);
