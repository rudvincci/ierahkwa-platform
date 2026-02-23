using System;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.Commands;
using Mamey.Government.Modules.Identity.Core.DTO;
using Mamey.Government.Modules.Identity.Core.Queries;
using Mamey.MicroMonolith.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.Government.Modules.Identity.Api.Controllers;

[Authorize]
internal class UserProfilesController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public UserProfilesController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get user profile by ID")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> GetAsync(Guid id)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetUserProfile { UserId = id }));

    [HttpGet("by-email/{email}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation("Get user profile by email")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> GetByEmailAsync(string email)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetUserProfileByEmail { Email = email }));

    [HttpGet("by-authenticator")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation("Get user profile by authenticator")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> GetByAuthenticatorAsync(
        [FromQuery] string issuer,
        [FromQuery] string subject)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetUserProfileByAuthenticator 
        { 
            Issuer = issuer, 
            Subject = subject 
        }));

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation("Browse user profiles")]
    [ProducesResponseType(typeof(PagedResult<UserProfileSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserProfileSummaryDto>>> BrowseAsync(
        [FromQuery] Guid? tenantId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new BrowseUserProfiles
        {
            TenantId = tenantId,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };
        return Ok(await _queryDispatcher.QueryAsync(query));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation("Create user profile")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserProfileRequest request)
    {
        var command = new CreateUserProfile(
            request.AuthenticatorIssuer,
            request.AuthenticatorSubject,
            request.Email,
            request.DisplayName,
            request.TenantId);

        await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetAsync), new { id = command.Id }, new { UserId = command.Id });
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation("Update user profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateUserProfileRequest request)
    {
        await _commandDispatcher.SendAsync(new UpdateUserProfile(id, request.Email, request.DisplayName));
        return NoContent();
    }

    [HttpPut("{id:guid}/tenant")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation("Update user tenant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTenantAsync(Guid id, [FromBody] UpdateTenantRequest request)
    {
        await _commandDispatcher.SendAsync(new UpdateUserTenant(id, request.TenantId));
        return NoContent();
    }

    [HttpPost("{id:guid}/login")]
    [AllowAnonymous]
    [SwaggerOperation("Record user login (internal)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RecordLoginAsync(Guid id)
    {
        await _commandDispatcher.SendAsync(new RecordUserLogin(id));
        return NoContent();
    }
}

// Request DTOs
public record CreateUserProfileRequest(
    string AuthenticatorIssuer,
    string AuthenticatorSubject,
    string? Email = null,
    string? DisplayName = null,
    Guid? TenantId = null);

public record UpdateUserProfileRequest(string? Email, string? DisplayName);

public record UpdateTenantRequest(Guid? TenantId);
