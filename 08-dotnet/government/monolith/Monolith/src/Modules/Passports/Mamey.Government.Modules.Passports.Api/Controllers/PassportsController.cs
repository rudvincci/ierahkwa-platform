using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.Commands;
using Mamey.Government.Modules.Passports.Core.DTO;
using Mamey.Government.Modules.Passports.Core.Queries;
using Mamey.MicroMonolith.Abstractions;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.Government.Modules.Passports.Api.Controllers;

[Authorize]
internal class PassportsController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IContext _context;

    public PassportsController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IContext context)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _context = context;
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get passport by ID")]
    [ProducesResponseType(typeof(PassportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PassportDto>> GetAsync(Guid id)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetPassport { PassportId = id }));

    [HttpGet("by-number/{passportNumber}")]
    [SwaggerOperation("Get passport by passport number")]
    [ProducesResponseType(typeof(PassportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PassportDto>> GetByNumberAsync(string passportNumber)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetPassportByNumber { PassportNumber = passportNumber }));

    [HttpGet]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Browse passports")]
    [ProducesResponseType(typeof(PagedResult<PassportSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PassportSummaryDto>>> BrowseAsync(
        [FromQuery] Guid tenantId,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new BrowsePassports
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
    [SwaggerOperation("Get passports for a citizen")]
    [ProducesResponseType(typeof(IEnumerable<PassportSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PassportSummaryDto>>> GetByCitizenAsync(Guid citizenId)
    {
        var passports = await _queryDispatcher.QueryAsync(new GetPassportsByCitizen { CitizenId = citizenId });
        return Ok(passports);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Issue new passport")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IssueAsync([FromBody] IssuePassportRequest request)
    {
        var command = new IssuePassport(
            request.CitizenId,
            request.TenantId,
            request.PassportType ?? "Standard",
            request.ValidityYears);

        await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetAsync), new { id = command.Id }, new { PassportId = command.Id });
    }

    [HttpPost("{id:guid}/renew")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Renew passport")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenewAsync(Guid id, [FromBody] RenewPassportRequest? request = null)
    {
        await _commandDispatcher.SendAsync(new RenewPassport(id, request?.ValidityYears ?? 10));
        return Ok(new { Message = "Passport renewed successfully" });
    }

    [HttpPost("{id:guid}/revoke")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Revoke passport")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeAsync(Guid id, [FromBody] RevokePassportRequest request)
    {
        var revokedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new RevokePassport(id, request.Reason, revokedBy));
        return NoContent();
    }

    [HttpPost("{id:guid}/report-lost-stolen")]
    [Authorize(Roles = "Admin,GovernmentAgent,Citizen")]
    [SwaggerOperation("Report passport as lost or stolen")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReportLostStolenAsync(Guid id, [FromBody] ReportLostStolenRequest request)
    {
        var reportedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new ReportLostStolen(id, request.ReportType, request.Description, reportedBy));
        return NoContent();
    }

    [HttpPost("validate")]
    [AllowAnonymous]
    [SwaggerOperation("Publicly validate a passport")]
    [ProducesResponseType(typeof(PassportValidationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PassportValidationResultDto>> ValidateAsync([FromBody] ValidatePassportRequest request)
    {
        var result = await _queryDispatcher.QueryAsync(new ValidatePassport 
        { 
            PassportNumber = request.PassportNumber,
            DateOfBirth = request.DateOfBirth,
            LastName = request.LastName
        });
        return Ok(result);
    }
}

// Request DTOs
public record IssuePassportRequest(
    Guid CitizenId,
    Guid TenantId,
    string? PassportType = "Standard",
    int ValidityYears = 10);

public record RenewPassportRequest(int ValidityYears = 10);

public record RevokePassportRequest(string Reason);

public record ReportLostStolenRequest(
    string ReportType,
    string Description);

public record ValidatePassportRequest(
    string PassportNumber,
    string? DateOfBirth = null,
    string? LastName = null);
