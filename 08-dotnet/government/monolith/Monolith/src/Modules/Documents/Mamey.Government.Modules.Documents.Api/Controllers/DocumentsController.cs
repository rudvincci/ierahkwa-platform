using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Documents.Core.Commands;
using Mamey.Government.Modules.Documents.Core.DTO;
using Mamey.Government.Modules.Documents.Core.Queries;
using Mamey.MicroMonolith.Abstractions;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.Government.Modules.Documents.Api.Controllers;

[Authorize]
internal class DocumentsController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IContext _context;

    public DocumentsController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IContext context)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _context = context;
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get document by ID")]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentDto>> GetAsync(Guid id)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetDocument { DocumentId = id }));

    [HttpGet]
    [SwaggerOperation("Browse documents")]
    [ProducesResponseType(typeof(PagedResult<DocumentSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<DocumentSummaryDto>>> BrowseAsync(
        [FromQuery] Guid tenantId,
        [FromQuery] string? category = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new BrowseDocuments
        {
            TenantId = tenantId,
            Category = category,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };
        return Ok(await _queryDispatcher.QueryAsync(query));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Upload document")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> UploadAsync([FromBody] UploadDocumentRequest request)
    {
        var command = new UploadDocument(
            request.TenantId,
            request.FileName,
            request.ContentType,
            request.FileSize,
            request.StorageBucket,
            request.StorageKey,
            request.Category,
            request.Description);

        await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetAsync), new { id = command.Id }, new { DocumentId = command.Id });
    }

    [HttpPut("{id:guid}/metadata")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Update document metadata")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateMetadataAsync(Guid id, [FromBody] UpdateDocumentMetadataRequest request)
    {
        await _commandDispatcher.SendAsync(new UpdateDocumentMetadata(id, request.Description, request.Tags));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Delete document")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var deletedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new DeleteDocument(id, deletedBy));
        return NoContent();
    }
}

// Request DTOs
public record UploadDocumentRequest(
    Guid TenantId,
    string FileName,
    string ContentType,
    long FileSize,
    string StorageBucket,
    string StorageKey,
    string? Category = null,
    string? Description = null);

public record UpdateDocumentMetadataRequest(
    string? Description = null,
    Dictionary<string, string>? Tags = null);
