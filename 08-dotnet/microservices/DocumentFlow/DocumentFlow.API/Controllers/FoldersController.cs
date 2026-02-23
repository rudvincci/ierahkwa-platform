using DocumentFlow.Core.Interfaces;
using DocumentFlow.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoldersController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public FoldersController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost]
    public async Task<ActionResult<Folder>> CreateFolder([FromBody] Folder folder)
    {
        var created = await _documentService.CreateFolderAsync(folder);
        return CreatedAtAction(nameof(GetFolder), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Folder>> GetFolder(Guid id)
    {
        var folder = await _documentService.GetFolderByIdAsync(id);
        if (folder == null) return NotFound();
        return folder;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Folder>>> GetFolders([FromQuery] Guid? parentId)
    {
        var folders = await _documentService.GetFoldersByParentAsync(parentId);
        return Ok(folders);
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<Folder>>> GetFolderTree([FromQuery] Guid? rootId)
    {
        var folders = await _documentService.GetFolderTreeAsync(rootId);
        return Ok(folders);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Folder>> UpdateFolder(Guid id, [FromBody] Folder folder)
    {
        if (id != folder.Id) return BadRequest();
        var updated = await _documentService.UpdateFolderAsync(folder);
        return updated;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFolder(Guid id, [FromQuery] bool recursive = false)
    {
        await _documentService.DeleteFolderAsync(id, recursive);
        return NoContent();
    }

    [HttpPost("{id}/move")]
    public async Task<ActionResult<Folder>> MoveFolder(Guid id, [FromBody] MoveFolderRequest request)
    {
        var folder = await _documentService.MoveFolderAsync(id, request.NewParentId);
        return folder;
    }

    [HttpGet("{id}/documents")]
    public async Task<ActionResult<IEnumerable<Document>>> GetFolderDocuments(Guid id)
    {
        var documents = await _documentService.GetDocumentsByFolderAsync(id);
        return Ok(documents);
    }
}

public class MoveFolderRequest
{
    public Guid? NewParentId { get; set; }
}
