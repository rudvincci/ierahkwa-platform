using DocumentFlow.Core.Interfaces;
using DocumentFlow.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost]
    public async Task<ActionResult<Document>> CreateDocument([FromForm] CreateDocumentRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("File is required");

        var document = new Document
        {
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Category = request.Category,
            Department = request.Department,
            FileName = request.File.FileName,
            FileExtension = Path.GetExtension(request.File.FileName),
            ContentType = request.File.ContentType,
            FileSize = request.File.Length,
            SecurityLevel = request.SecurityLevel,
            Tags = request.Tags,
            CreatedBy = request.UserId,
            OwnerId = request.UserId
        };

        using var stream = request.File.OpenReadStream();
        var created = await _documentService.CreateDocumentAsync(document, stream);
        return CreatedAtAction(nameof(GetDocument), new { id = created.Id }, created);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Document>> GetDocument(Guid id)
    {
        var document = await _documentService.GetDocumentByIdAsync(id);
        if (document == null) return NotFound();
        return document;
    }

    [HttpGet("number/{documentNumber}")]
    public async Task<ActionResult<Document>> GetDocumentByNumber(string documentNumber)
    {
        var document = await _documentService.GetDocumentByNumberAsync(documentNumber);
        if (document == null) return NotFound();
        return document;
    }

    [HttpGet("department/{department}")]
    public async Task<ActionResult<IEnumerable<Document>>> GetDocumentsByDepartment(string department)
    {
        var documents = await _documentService.GetDocumentsByDepartmentAsync(department);
        return Ok(documents);
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<ActionResult<IEnumerable<Document>>> GetDocumentsByOwner(Guid ownerId)
    {
        var documents = await _documentService.GetDocumentsByOwnerAsync(ownerId);
        return Ok(documents);
    }

    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<Document>>> SearchDocuments([FromBody] DocumentSearchCriteria criteria)
    {
        var documents = await _documentService.SearchDocumentsAsync(criteria);
        return Ok(documents);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Document>> UpdateDocument(Guid id, [FromBody] Document document)
    {
        if (id != document.Id) return BadRequest();
        var updated = await _documentService.UpdateDocumentAsync(document);
        return updated;
    }

    [HttpPost("{id}/upload")]
    public async Task<ActionResult<Document>> UpdateDocumentFile(Guid id, [FromForm] IFormFile file, [FromForm] string? changeNotes)
    {
        using var stream = file.OpenReadStream();
        var updated = await _documentService.UpdateDocumentFileAsync(id, stream, changeNotes);
        return updated;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDocument(Guid id)
    {
        await _documentService.DeleteDocumentAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/archive")]
    public async Task<ActionResult<Document>> ArchiveDocument(Guid id)
    {
        var document = await _documentService.ArchiveDocumentAsync(id);
        return document;
    }

    [HttpPost("{id}/restore")]
    public async Task<ActionResult<Document>> RestoreDocument(Guid id)
    {
        var document = await _documentService.RestoreDocumentAsync(id);
        return document;
    }

    [HttpGet("{id}/download")]
    public async Task<ActionResult> DownloadDocument(Guid id)
    {
        var document = await _documentService.GetDocumentByIdAsync(id);
        if (document == null) return NotFound();
        
        var stream = await _documentService.DownloadDocumentAsync(id);
        return File(stream, document.ContentType, document.FileName);
    }

    [HttpGet("{id}/versions")]
    public async Task<ActionResult<IEnumerable<DocumentVersion>>> GetVersions(Guid id)
    {
        var versions = await _documentService.GetDocumentVersionsAsync(id);
        return Ok(versions);
    }

    [HttpGet("{id}/versions/{version}")]
    public async Task<ActionResult<DocumentVersion>> GetVersion(Guid id, int version)
    {
        var ver = await _documentService.GetDocumentVersionAsync(id, version);
        if (ver == null) return NotFound();
        return ver;
    }

    [HttpPost("{id}/revert/{version}")]
    public async Task<ActionResult<Document>> RevertToVersion(Guid id, int version)
    {
        var document = await _documentService.RevertToVersionAsync(id, version);
        return document;
    }

    [HttpGet("{id}/comments")]
    public async Task<ActionResult<IEnumerable<DocumentComment>>> GetComments(Guid id)
    {
        var comments = await _documentService.GetDocumentCommentsAsync(id);
        return Ok(comments);
    }

    [HttpPost("{id}/comments")]
    public async Task<ActionResult<DocumentComment>> AddComment(Guid id, [FromBody] DocumentComment comment)
    {
        comment.DocumentId = id;
        var created = await _documentService.AddCommentAsync(comment);
        return CreatedAtAction(nameof(GetComments), new { id }, created);
    }

    [HttpPost("{id}/workflow")]
    public async Task<ActionResult<DocumentWorkflow>> StartWorkflow(Guid id, [FromBody] StartWorkflowRequest request)
    {
        var workflow = await _documentService.StartWorkflowAsync(id, request.WorkflowTemplateId, request.InitiatedBy);
        return CreatedAtAction(nameof(GetWorkflow), new { id, workflowId = workflow.Id }, workflow);
    }

    [HttpGet("{id}/workflow/{workflowId}")]
    public async Task<ActionResult<DocumentWorkflow>> GetWorkflow(Guid id, Guid workflowId)
    {
        var workflow = await _documentService.GetWorkflowAsync(workflowId);
        if (workflow == null) return NotFound();
        return workflow;
    }

    [HttpPost("{id}/ocr")]
    public async Task<ActionResult> ProcessOcr(Guid id)
    {
        await _documentService.ProcessOcrAsync(id);
        return Ok();
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<DocumentStatistics>> GetStatistics([FromQuery] string? department)
    {
        var stats = await _documentService.GetStatisticsAsync(department);
        return stats;
    }

    [HttpGet("recent/{userId}")]
    public async Task<ActionResult<IEnumerable<Document>>> GetRecentDocuments(Guid userId, [FromQuery] int count = 10)
    {
        var documents = await _documentService.GetRecentDocumentsAsync(userId, count);
        return Ok(documents);
    }

    [HttpGet("pending-approvals/{userId}")]
    public async Task<ActionResult<IEnumerable<Document>>> GetPendingApprovals(Guid userId)
    {
        var documents = await _documentService.GetPendingApprovalsAsync(userId);
        return Ok(documents);
    }
}

public class CreateDocumentRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType Type { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public SecurityLevel SecurityLevel { get; set; }
    public string? Tags { get; set; }
    public Guid UserId { get; set; }
    public IFormFile? File { get; set; }
}

public class StartWorkflowRequest
{
    public Guid WorkflowTemplateId { get; set; }
    public Guid InitiatedBy { get; set; }
}
