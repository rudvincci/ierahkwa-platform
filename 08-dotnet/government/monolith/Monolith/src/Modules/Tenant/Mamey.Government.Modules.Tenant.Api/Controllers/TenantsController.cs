using System;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Tenant.Core.Commands;
using Mamey.Government.Modules.Tenant.Core.DTO;
using Mamey.Government.Modules.Tenant.Core.Queries;
using Mamey.MicroMonolith.Abstractions;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.Government.Modules.Tenant.Api.Controllers;

[Authorize(Roles = "Admin")]
internal class TenantsController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IContext _context;

    public TenantsController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IContext context)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _context = context;
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get tenant by ID")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantDto>> GetAsync(Guid id)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetTenant { TenantId = id }));

    [HttpGet]
    [SwaggerOperation("Browse tenants")]
    [ProducesResponseType(typeof(PagedResult<TenantSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TenantSummaryDto>>> BrowseAsync(
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new BrowseTenants
        {
            Status = status,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };
        return Ok(await _queryDispatcher.QueryAsync(query));
    }

    [HttpPost]
    [SwaggerOperation("Create new tenant")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTenantRequest request)
    {
        var command = new CreateTenant(request.DisplayName, request.Domain);
        await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetAsync), new { id = command.Id }, new { TenantId = command.Id });
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation("Update tenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTenantRequest request)
    {
        await _commandDispatcher.SendAsync(new UpdateTenant(id, request.DisplayName, request.Domain));
        return NoContent();
    }

    [HttpPost("{id:guid}/suspend")]
    [SwaggerOperation("Suspend tenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SuspendAsync(Guid id)
    {
        var suspendedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new SuspendTenant(id, suspendedBy));
        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    [SwaggerOperation("Activate tenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAsync(Guid id)
    {
        var activatedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new ActivateTenant(id, activatedBy));
        return NoContent();
    }
}

// Request DTOs
public record CreateTenantRequest(string DisplayName, string? Domain = null);
public record UpdateTenantRequest(string DisplayName, string? Domain = null);
