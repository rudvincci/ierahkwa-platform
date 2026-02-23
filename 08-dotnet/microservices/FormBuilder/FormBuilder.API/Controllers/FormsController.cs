using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using Microsoft.AspNetCore.Mvc;
namespace FormBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormsController : ControllerBase
{
    private readonly IFormService _service;
    public FormsController(IFormService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Form>> Create([FromBody] Form form) => await _service.CreateFormAsync(form);
    [HttpGet("{id}")] public async Task<ActionResult<Form>> GetById(Guid id) { var f = await _service.GetFormByIdAsync(id); return f == null ? NotFound() : f; }
    [HttpGet("code/{code}")] public async Task<ActionResult<Form>> GetByCode(string code) { var f = await _service.GetFormByCodeAsync(code); return f == null ? NotFound() : f; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Form>>> GetAll([FromQuery] FormType? type, [FromQuery] FormStatus? status, [FromQuery] string? department) => Ok(await _service.GetFormsAsync(type, status, department));
    [HttpPut("{id}")] public async Task<ActionResult<Form>> Update(Guid id, [FromBody] Form form) => await _service.UpdateFormAsync(form);
    [HttpPost("{id}/publish")] public async Task<ActionResult<Form>> Publish(Guid id) => await _service.PublishFormAsync(id);
    [HttpPost("{id}/close")] public async Task<ActionResult<Form>> Close(Guid id) => await _service.CloseFormAsync(id);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteFormAsync(id); return NoContent(); }
    [HttpPost("{id}/fields")] public async Task<ActionResult<FormField>> AddField(Guid id, [FromBody] FormField field) { field.FormId = id; return await _service.AddFieldAsync(field); }
    [HttpPost("{id}/submit")] public async Task<ActionResult<FormSubmission>> Submit(Guid id, [FromBody] FormSubmission submission) { submission.FormId = id; return await _service.SubmitFormAsync(submission); }
    [HttpGet("{id}/submissions")] public async Task<ActionResult<IEnumerable<FormSubmission>>> GetSubmissions(Guid id, [FromQuery] SubmissionStatus? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to) => Ok(await _service.GetSubmissionsAsync(id, status, from, to));
    [HttpGet("{id}/export")] public async Task<ActionResult> Export(Guid id, [FromQuery] string format = "csv") { var data = await _service.ExportSubmissionsAsync(id, format); return File(data, "text/csv", $"submissions-{id}.csv"); }
    [HttpGet("statistics")] public async Task<ActionResult<FormStatistics>> GetStatistics([FromQuery] Guid? formId, [FromQuery] string? department) => await _service.GetStatisticsAsync(formId, department);
}

[ApiController]
[Route("api/form-submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly IFormService _service;
    public SubmissionsController(IFormService service) => _service = service;
    [HttpGet("{id}")] public async Task<ActionResult<FormSubmission>> GetById(Guid id) { var s = await _service.GetSubmissionByIdAsync(id); return s == null ? NotFound() : s; }
    [HttpPost("{id}/status")] public async Task<ActionResult<FormSubmission>> UpdateStatus(Guid id, [FromQuery] SubmissionStatus status, [FromQuery] Guid? processedBy, [FromQuery] string? notes) => await _service.UpdateSubmissionStatusAsync(id, status, processedBy, notes);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteSubmissionAsync(id); return NoContent(); }
}

[ApiController]
[Route("api/form-templates")]
public class TemplatesController : ControllerBase
{
    private readonly IFormService _service;
    public TemplatesController(IFormService service) => _service = service;
    [HttpPost] public async Task<ActionResult<FormTemplate>> Create([FromBody] FormTemplate template) => await _service.CreateTemplateAsync(template);
    [HttpGet] public async Task<ActionResult<IEnumerable<FormTemplate>>> GetAll([FromQuery] string? category, [FromQuery] bool? isPublic) => Ok(await _service.GetTemplatesAsync(category, isPublic));
    [HttpPost("{id}/create-form")] public async Task<ActionResult<Form>> CreateForm(Guid id, [FromQuery] string name, [FromQuery] Guid createdBy) => await _service.CreateFormFromTemplateAsync(id, name, createdBy);
}
