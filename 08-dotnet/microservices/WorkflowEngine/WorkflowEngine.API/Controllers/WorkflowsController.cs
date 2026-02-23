using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Core.Interfaces;
using WorkflowEngine.Core.Models;

namespace WorkflowEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowsController : ControllerBase
{
    private readonly IWorkflowService _workflowService;

    public WorkflowsController(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Workflow>>> GetAll()
    {
        var workflows = await _workflowService.GetAllAsync();
        return Ok(workflows);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Workflow>>> GetActive()
    {
        var workflows = await _workflowService.GetActiveAsync();
        return Ok(workflows);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Workflow>> GetById(string id)
    {
        var workflow = await _workflowService.GetByIdAsync(id);
        if (workflow == null) return NotFound();
        return Ok(workflow);
    }

    [HttpPost]
    public async Task<ActionResult<Workflow>> Create([FromBody] Workflow workflow)
    {
        var created = await _workflowService.CreateAsync(workflow);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id}/execute")]
    public async Task<ActionResult<WorkflowExecution>> Execute(string id, [FromBody] Dictionary<string, object>? input = null)
    {
        var execution = await _workflowService.ExecuteAsync(id, input ?? new());
        return Ok(execution);
    }

    [HttpPost("{id}/pause")]
    public async Task<IActionResult> Pause(string id)
    {
        await _workflowService.PauseAsync(id);
        return Ok(new { message = "Workflow paused" });
    }

    [HttpPost("{id}/resume")]
    public async Task<IActionResult> Resume(string id)
    {
        await _workflowService.ResumeAsync(id);
        return Ok(new { message = "Workflow resumed" });
    }

    [HttpGet("{id}/executions")]
    public async Task<ActionResult<IEnumerable<WorkflowExecution>>> GetExecutions(string id)
    {
        var executions = await _workflowService.GetExecutionsAsync(id);
        return Ok(executions);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<WorkflowStats>> GetStats()
    {
        var stats = await _workflowService.GetStatsAsync();
        return Ok(stats);
    }
}
