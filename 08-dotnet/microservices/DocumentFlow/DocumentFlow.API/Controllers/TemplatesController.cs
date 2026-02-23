using DocumentFlow.Core.Interfaces;
using DocumentFlow.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public TemplatesController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost]
    public async Task<ActionResult<DocumentTemplate>> CreateTemplate([FromForm] CreateTemplateRequest request)
    {
        if (request.File == null) return BadRequest("File is required");

        var template = new DocumentTemplate
        {
            Name = request.Name,
            Description = request.Description,
            DocumentType = request.DocumentType,
            Category = request.Category,
            Department = request.Department,
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            TemplateFields = request.TemplateFields,
            DefaultWorkflowId = request.DefaultWorkflowId,
            CreatedBy = request.UserId
        };

        using var stream = request.File.OpenReadStream();
        var created = await _documentService.CreateTemplateAsync(template, stream);
        return CreatedAtAction(nameof(GetTemplates), created);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentTemplate>>> GetTemplates([FromQuery] string? category)
    {
        var templates = await _documentService.GetTemplatesAsync(category);
        return Ok(templates);
    }

    [HttpPost("{id}/create-document")]
    public async Task<ActionResult<Document>> CreateDocumentFromTemplate(Guid id, [FromBody] Dictionary<string, string> fieldValues)
    {
        var document = await _documentService.CreateDocumentFromTemplateAsync(id, fieldValues);
        return CreatedAtAction("GetDocument", "Documents", new { id = document.Id }, document);
    }
}

public class CreateTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType DocumentType { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public string? TemplateFields { get; set; }
    public Guid? DefaultWorkflowId { get; set; }
    public Guid UserId { get; set; }
    public IFormFile? File { get; set; }
}
