using Microsoft.AspNetCore.Mvc;
using ProjectHub.Core.Interfaces;
using ProjectHub.Core.Models;

namespace ProjectHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;
    public ProjectsController(IProjectService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Project>> Create([FromBody] Project project) => CreatedAtAction(nameof(GetById), new { id = (await _service.CreateProjectAsync(project)).Id }, project);
    [HttpGet("{id}")] public async Task<ActionResult<Project>> GetById(Guid id) { var p = await _service.GetProjectByIdAsync(id); return p == null ? NotFound() : p; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Project>>> GetAll([FromQuery] string? department, [FromQuery] ProjectStatus? status) => Ok(await _service.GetProjectsAsync(department, status));
    [HttpGet("user/{userId}")] public async Task<ActionResult<IEnumerable<Project>>> GetByUser(Guid userId) => Ok(await _service.GetUserProjectsAsync(userId));
    [HttpPut("{id}")] public async Task<ActionResult<Project>> Update(Guid id, [FromBody] Project project) => id != project.Id ? BadRequest() : await _service.UpdateProjectAsync(project);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteProjectAsync(id); return NoContent(); }

    [HttpPost("{id}/tasks")] public async Task<ActionResult<ProjectTask>> CreateTask(Guid id, [FromBody] ProjectTask task) { task.ProjectId = id; return await _service.CreateTaskAsync(task); }
    [HttpGet("{id}/tasks")] public async Task<ActionResult<IEnumerable<ProjectTask>>> GetTasks(Guid id) => Ok(await _service.GetProjectTasksAsync(id));
    [HttpGet("{id}/board")] public async Task<ActionResult<Board>> GetBoard(Guid id) => await _service.GetBoardAsync(id);
    [HttpPut("{id}/board")] public async Task<ActionResult<Board>> UpdateBoard(Guid id, [FromBody] Board board) => await _service.UpdateBoardAsync(board);

    [HttpPost("{id}/members")] public async Task<ActionResult<ProjectMember>> AddMember(Guid id, [FromBody] ProjectMember member) => await _service.AddMemberAsync(id, member);
    [HttpGet("{id}/members")] public async Task<ActionResult<IEnumerable<ProjectMember>>> GetMembers(Guid id) => Ok(await _service.GetMembersAsync(id));

    [HttpPost("{id}/milestones")] public async Task<ActionResult<ProjectMilestone>> CreateMilestone(Guid id, [FromBody] ProjectMilestone milestone) { milestone.ProjectId = id; return await _service.CreateMilestoneAsync(milestone); }
    [HttpGet("{id}/milestones")] public async Task<ActionResult<IEnumerable<ProjectMilestone>>> GetMilestones(Guid id) => Ok(await _service.GetMilestonesAsync(id));

    [HttpGet("statistics")] public async Task<ActionResult<ProjectStatistics>> GetStatistics([FromQuery] Guid? projectId, [FromQuery] string? department) => await _service.GetStatisticsAsync(projectId, department);
}

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IProjectService _service;
    public TasksController(IProjectService service) => _service = service;

    [HttpGet("{id}")] public async Task<ActionResult<ProjectTask>> GetById(Guid id) { var t = await _service.GetTaskByIdAsync(id); return t == null ? NotFound() : t; }
    [HttpGet("user/{userId}")] public async Task<ActionResult<IEnumerable<ProjectTask>>> GetByUser(Guid userId) => Ok(await _service.GetUserTasksAsync(userId));
    [HttpPut("{id}")] public async Task<ActionResult<ProjectTask>> Update(Guid id, [FromBody] ProjectTask task) => await _service.UpdateTaskAsync(task);
    [HttpPost("{id}/move")] public async Task<ActionResult<ProjectTask>> Move(Guid id, [FromQuery] string column, [FromQuery] int orderIndex) => await _service.MoveTaskAsync(id, column, orderIndex);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteTaskAsync(id); return NoContent(); }
    [HttpPost("{id}/comments")] public async Task<ActionResult<TaskComment>> AddComment(Guid id, [FromBody] TaskComment comment) { comment.TaskId = id; return await _service.AddCommentAsync(comment); }
    [HttpPost("{id}/time")] public async Task<ActionResult<TimeEntry>> LogTime(Guid id, [FromBody] TimeEntry entry) { entry.TaskId = id; return await _service.LogTimeAsync(entry); }
    [HttpGet("{id}/time")] public async Task<ActionResult<IEnumerable<TimeEntry>>> GetTimeEntries(Guid id) => Ok(await _service.GetTimeEntriesAsync(id));
}
