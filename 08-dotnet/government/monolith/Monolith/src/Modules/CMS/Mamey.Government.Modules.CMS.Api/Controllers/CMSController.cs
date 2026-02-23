using System;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CMS.Core.Commands;
using Mamey.Government.Modules.CMS.Core.DTO;
using Mamey.Government.Modules.CMS.Core.Queries;
using Mamey.MicroMonolith.Abstractions;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.Government.Modules.CMS.Api.Controllers;

[Authorize]
internal class CMSController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IContext _context;

    public CMSController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IContext context)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _context = context;
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [SwaggerOperation("Get content by ID")]
    [ProducesResponseType(typeof(ContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentDto>> GetAsync(Guid id)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetContent { ContentId = id }));

    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    [SwaggerOperation("Get content by slug")]
    [ProducesResponseType(typeof(ContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentDto>> GetBySlugAsync(string slug)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetContentBySlug { Slug = slug }));

    [HttpGet]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Browse content")]
    [ProducesResponseType(typeof(PagedResult<ContentSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ContentSummaryDto>>> BrowseAsync(
        [FromQuery] Guid tenantId,
        [FromQuery] string? contentType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new BrowseContent
        {
            TenantId = tenantId,
            ContentType = contentType,
            Status = status,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };
        return Ok(await _queryDispatcher.QueryAsync(query));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Create content")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateContentRequest request)
    {
        var command = new CreateContent(
            request.TenantId,
            request.Title,
            request.Slug,
            request.ContentType,
            request.Body,
            request.Excerpt);

        await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetAsync), new { id = command.Id }, new { ContentId = command.Id });
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Publish content")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PublishAsync(Guid id)
    {
        await _commandDispatcher.SendAsync(new PublishContent(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/unpublish")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Unpublish content")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnpublishAsync(Guid id)
    {
        await _commandDispatcher.SendAsync(new UnpublishContent(id));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation("Delete content")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var deletedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new DeleteContent(id, deletedBy));
        return NoContent();
    }
}

// Request DTOs
public record CreateContentRequest(
    Guid TenantId,
    string Title,
    string Slug,
    string ContentType,
    string? Body = null,
    string? Excerpt = null);
