using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Core.Interfaces;
using ServiceDesk.Core.Models;

namespace ServiceDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _service;
    public TicketsController(ITicketService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Ticket>> Create([FromBody] Ticket ticket) => CreatedAtAction(nameof(GetById), new { id = (await _service.CreateTicketAsync(ticket)).Id }, ticket);
    [HttpGet("{id}")] public async Task<ActionResult<Ticket>> GetById(Guid id) { var t = await _service.GetTicketByIdAsync(id); return t == null ? NotFound() : t; }
    [HttpGet("number/{number}")] public async Task<ActionResult<Ticket>> GetByNumber(string number) { var t = await _service.GetTicketByNumberAsync(number); return t == null ? NotFound() : t; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Ticket>>> GetAll([FromQuery] TicketStatus? status, [FromQuery] TicketPriority? priority, [FromQuery] string? department, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) => Ok(await _service.GetTicketsAsync(status, priority, department, page, pageSize));
    [HttpGet("requester/{requesterId}")] public async Task<ActionResult<IEnumerable<Ticket>>> GetByRequester(Guid requesterId) => Ok(await _service.GetRequesterTicketsAsync(requesterId));
    [HttpGet("assigned/{agentId}")] public async Task<ActionResult<IEnumerable<Ticket>>> GetAssigned(Guid agentId) => Ok(await _service.GetAssignedTicketsAsync(agentId));
    [HttpPut("{id}")] public async Task<ActionResult<Ticket>> Update(Guid id, [FromBody] Ticket ticket) => await _service.UpdateTicketAsync(ticket);
    [HttpPost("{id}/assign")] public async Task<ActionResult<Ticket>> Assign(Guid id, [FromQuery] Guid? agentId, [FromQuery] Guid? groupId) => await _service.AssignTicketAsync(id, agentId, groupId);
    [HttpPost("{id}/status")] public async Task<ActionResult<Ticket>> UpdateStatus(Guid id, [FromBody] TicketStatus status) => await _service.UpdateStatusAsync(id, status);
    [HttpPost("{id}/resolve")] public async Task<ActionResult<Ticket>> Resolve(Guid id, [FromBody] string resolution) => await _service.ResolveTicketAsync(id, resolution);
    [HttpPost("{id}/reopen")] public async Task<ActionResult<Ticket>> Reopen(Guid id) => await _service.ReopenTicketAsync(id);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteTicketAsync(id); return NoContent(); }
    [HttpPost("{id}/comments")] public async Task<ActionResult<TicketComment>> AddComment(Guid id, [FromBody] TicketComment comment) { comment.TicketId = id; return await _service.AddCommentAsync(comment); }
    [HttpGet("{id}/comments")] public async Task<ActionResult<IEnumerable<TicketComment>>> GetComments(Guid id) => Ok(await _service.GetCommentsAsync(id));
    [HttpGet("statistics")] public async Task<ActionResult<ServiceDeskStatistics>> GetStatistics([FromQuery] string? department) => await _service.GetStatisticsAsync(department);
}

[ApiController]
[Route("api/[controller]")]
public class KnowledgeBaseController : ControllerBase
{
    private readonly ITicketService _service;
    public KnowledgeBaseController(ITicketService service) => _service = service;

    [HttpPost] public async Task<ActionResult<KnowledgeArticle>> Create([FromBody] KnowledgeArticle article) => await _service.CreateArticleAsync(article);
    [HttpGet("{id}")] public async Task<ActionResult<KnowledgeArticle>> GetById(Guid id) { await _service.IncrementArticleViewAsync(id); var a = await _service.GetArticleByIdAsync(id); return a == null ? NotFound() : a; }
    [HttpGet] public async Task<ActionResult<IEnumerable<KnowledgeArticle>>> Search([FromQuery] string? query, [FromQuery] string? category) => Ok(await _service.SearchArticlesAsync(query, category));
    [HttpPut("{id}")] public async Task<ActionResult<KnowledgeArticle>> Update(Guid id, [FromBody] KnowledgeArticle article) => await _service.UpdateArticleAsync(article);
    [HttpPost("{id}/publish")] public async Task<ActionResult<KnowledgeArticle>> Publish(Guid id) => await _service.PublishArticleAsync(id);
    [HttpPost("{id}/rate")] public async Task<ActionResult> Rate(Guid id, [FromQuery] bool helpful) { await _service.RateArticleAsync(id, helpful); return Ok(); }
}

[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly ITicketService _service;
    public AgentsController(ITicketService service) => _service = service;

    [HttpPost] public async Task<ActionResult<SupportAgent>> Create([FromBody] SupportAgent agent) => await _service.CreateAgentAsync(agent);
    [HttpGet] public async Task<ActionResult<IEnumerable<SupportAgent>>> GetAll([FromQuery] Guid? groupId) => Ok(await _service.GetAgentsAsync(groupId));
    [HttpPost("{id}/status")] public async Task<ActionResult<SupportAgent>> UpdateStatus(Guid id, [FromBody] AgentStatus status) => await _service.UpdateAgentStatusAsync(id, status);
}
