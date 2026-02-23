using CitizenCRM.Core.Interfaces;
using CitizenCRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitizenCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitizensController : ControllerBase
{
    private readonly ICitizenService _service;
    public CitizensController(ICitizenService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Citizen>> Create([FromBody] Citizen citizen) => CreatedAtAction(nameof(GetById), new { id = (await _service.CreateCitizenAsync(citizen)).Id }, citizen);
    [HttpGet("{id}")] public async Task<ActionResult<Citizen>> GetById(Guid id) { var c = await _service.GetCitizenByIdAsync(id); return c == null ? NotFound() : c; }
    [HttpGet("national-id/{nationalId}")] public async Task<ActionResult<Citizen>> GetByNationalId(string nationalId) { var c = await _service.GetCitizenByNationalIdAsync(nationalId); return c == null ? NotFound() : c; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Citizen>>> Search([FromQuery] string? query, [FromQuery] CitizenStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) => Ok(await _service.SearchCitizensAsync(query, status, page, pageSize));
    [HttpPut("{id}")] public async Task<ActionResult<Citizen>> Update(Guid id, [FromBody] Citizen citizen) => id != citizen.Id ? BadRequest() : await _service.UpdateCitizenAsync(citizen);
    [HttpPost("{id}/verify")] public async Task<ActionResult<Citizen>> Verify(Guid id, [FromQuery] string method) => await _service.VerifyCitizenAsync(id, method);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteCitizenAsync(id); return NoContent(); }

    [HttpPost("{id}/cases")] public async Task<ActionResult<CitizenCase>> CreateCase(Guid id, [FromBody] CitizenCase caseItem) { caseItem.CitizenId = id; return await _service.CreateCaseAsync(caseItem); }
    [HttpGet("{id}/cases")] public async Task<ActionResult<IEnumerable<CitizenCase>>> GetCases(Guid id) => Ok(await _service.GetCitizenCasesAsync(id));
    [HttpPost("{id}/interactions")] public async Task<ActionResult<CitizenInteraction>> LogInteraction(Guid id, [FromBody] CitizenInteraction interaction) { interaction.CitizenId = id; return await _service.LogInteractionAsync(interaction); }
    [HttpGet("{id}/interactions")] public async Task<ActionResult<IEnumerable<CitizenInteraction>>> GetInteractions(Guid id) => Ok(await _service.GetCitizenInteractionsAsync(id));
    [HttpPost("{id}/documents")] public async Task<ActionResult<CitizenDocument>> AddDocument(Guid id, [FromBody] CitizenDocument doc) { doc.CitizenId = id; return await _service.AddDocumentAsync(doc); }
    [HttpGet("{id}/documents")] public async Task<ActionResult<IEnumerable<CitizenDocument>>> GetDocuments(Guid id) => Ok(await _service.GetCitizenDocumentsAsync(id));
    [HttpPost("{id}/service-requests")] public async Task<ActionResult<ServiceRequest>> CreateServiceRequest(Guid id, [FromBody] ServiceRequest request) { request.CitizenId = id; return await _service.CreateServiceRequestAsync(request); }
    [HttpGet("{id}/service-requests")] public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetServiceRequests(Guid id) => Ok(await _service.GetCitizenServiceRequestsAsync(id));

    [HttpGet("statistics")] public async Task<ActionResult<CRMStatistics>> GetStatistics([FromQuery] string? department) => await _service.GetStatisticsAsync(department);
}

[ApiController]
[Route("api/[controller]")]
public class CasesController : ControllerBase
{
    private readonly ICitizenService _service;
    public CasesController(ICitizenService service) => _service = service;

    [HttpGet("{id}")] public async Task<ActionResult<CitizenCase>> GetById(Guid id) { var c = await _service.GetCaseByIdAsync(id); return c == null ? NotFound() : c; }
    [HttpGet("status/{status}")] public async Task<ActionResult<IEnumerable<CitizenCase>>> GetByStatus(CaseStatus status, [FromQuery] string? department) => Ok(await _service.GetCasesByStatusAsync(status, department));
    [HttpGet("assigned/{agentId}")] public async Task<ActionResult<IEnumerable<CitizenCase>>> GetAssigned(Guid agentId) => Ok(await _service.GetAssignedCasesAsync(agentId));
    [HttpPut("{id}")] public async Task<ActionResult<CitizenCase>> Update(Guid id, [FromBody] CitizenCase caseItem) => await _service.UpdateCaseAsync(caseItem);
    [HttpPost("{id}/assign")] public async Task<ActionResult<CitizenCase>> Assign(Guid id, [FromQuery] Guid agentId, [FromQuery] string agentName) => await _service.AssignCaseAsync(id, agentId, agentName);
    [HttpPost("{id}/resolve")] public async Task<ActionResult<CitizenCase>> Resolve(Guid id, [FromBody] string resolution) => await _service.ResolveCaseAsync(id, resolution);
    [HttpPost("{id}/notes")] public async Task<ActionResult<CaseNote>> AddNote(Guid id, [FromBody] CaseNote note) { note.CaseId = id; return await _service.AddCaseNoteAsync(note); }
}
