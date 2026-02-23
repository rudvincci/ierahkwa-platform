using ProjectHub.Core.Interfaces;
using ProjectHub.Core.Models;

namespace ProjectHub.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly List<Project> _projects = new();
    private readonly List<ProjectTask> _tasks = new();
    private readonly List<ProjectMilestone> _milestones = new();
    private readonly List<ProjectMember> _members = new();
    private readonly List<TimeEntry> _timeEntries = new();
    private readonly List<TaskComment> _comments = new();
    private readonly List<Board> _boards = new();

    public Task<Project> CreateProjectAsync(Project project)
    {
        project.Id = Guid.NewGuid();
        project.ProjectCode = $"PRJ-{DateTime.UtcNow:yyyyMM}-{_projects.Count + 1:D4}";
        project.Status = ProjectStatus.Planning;
        project.CreatedAt = DateTime.UtcNow;
        _projects.Add(project);

        var board = new Board { Id = Guid.NewGuid(), ProjectId = project.Id, Name = "Main Board",
            Columns = new() {
                new() { Id = Guid.NewGuid(), Name = "Backlog", OrderIndex = 0 },
                new() { Id = Guid.NewGuid(), Name = "Todo", OrderIndex = 1 },
                new() { Id = Guid.NewGuid(), Name = "In Progress", OrderIndex = 2, WipLimit = 5 },
                new() { Id = Guid.NewGuid(), Name = "Review", OrderIndex = 3 },
                new() { Id = Guid.NewGuid(), Name = "Done", OrderIndex = 4 }
            }};
        _boards.Add(board);
        return Task.FromResult(project);
    }

    public Task<Project?> GetProjectByIdAsync(Guid id) => Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));

    public Task<IEnumerable<Project>> GetProjectsAsync(string? department = null, ProjectStatus? status = null)
    {
        var query = _projects.AsEnumerable();
        if (!string.IsNullOrEmpty(department)) query = query.Where(p => p.Department == department);
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);
        return Task.FromResult(query);
    }

    public Task<IEnumerable<Project>> GetUserProjectsAsync(Guid userId)
    {
        var projectIds = _members.Where(m => m.UserId == userId).Select(m => m.ProjectId).ToHashSet();
        return Task.FromResult(_projects.Where(p => projectIds.Contains(p.Id) || p.ManagerId == userId));
    }

    public Task<Project> UpdateProjectAsync(Project project)
    {
        var existing = _projects.FirstOrDefault(p => p.Id == project.Id);
        if (existing != null) { existing.Name = project.Name; existing.Status = project.Status; existing.ProgressPercent = project.ProgressPercent; existing.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(existing ?? project);
    }

    public Task DeleteProjectAsync(Guid id) { _projects.RemoveAll(p => p.Id == id); return Task.CompletedTask; }

    public Task<ProjectTask> CreateTaskAsync(ProjectTask task)
    {
        task.Id = Guid.NewGuid();
        task.TaskCode = $"TASK-{_tasks.Count + 1:D5}";
        task.Status = TaskStatus.Backlog;
        task.BoardColumn = "Backlog";
        task.CreatedAt = DateTime.UtcNow;
        _tasks.Add(task);
        return Task.FromResult(task);
    }

    public Task<ProjectTask?> GetTaskByIdAsync(Guid id) => Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));
    public Task<IEnumerable<ProjectTask>> GetProjectTasksAsync(Guid projectId) => Task.FromResult(_tasks.Where(t => t.ProjectId == projectId));
    public Task<IEnumerable<ProjectTask>> GetUserTasksAsync(Guid userId) => Task.FromResult(_tasks.Where(t => t.AssigneeId == userId));

    public Task<ProjectTask> UpdateTaskAsync(ProjectTask task)
    {
        var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existing != null) { existing.Title = task.Title; existing.Status = task.Status; existing.ProgressPercent = task.ProgressPercent; existing.AssigneeId = task.AssigneeId; }
        return Task.FromResult(existing ?? task);
    }

    public Task<ProjectTask> MoveTaskAsync(Guid taskId, string column, int orderIndex)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null) { task.BoardColumn = column; task.OrderIndex = orderIndex; task.Status = column switch { "Done" => TaskStatus.Done, "In Progress" => TaskStatus.InProgress, "Review" => TaskStatus.Review, "Todo" => TaskStatus.Todo, _ => TaskStatus.Backlog }; }
        return Task.FromResult(task!);
    }

    public Task DeleteTaskAsync(Guid id) { _tasks.RemoveAll(t => t.Id == id); return Task.CompletedTask; }

    public Task<ProjectMilestone> CreateMilestoneAsync(ProjectMilestone milestone) { milestone.Id = Guid.NewGuid(); _milestones.Add(milestone); return Task.FromResult(milestone); }
    public Task<IEnumerable<ProjectMilestone>> GetMilestonesAsync(Guid projectId) => Task.FromResult(_milestones.Where(m => m.ProjectId == projectId));
    public Task<ProjectMilestone> UpdateMilestoneAsync(ProjectMilestone milestone) { var e = _milestones.FirstOrDefault(m => m.Id == milestone.Id); if (e != null) { e.Name = milestone.Name; e.Status = milestone.Status; } return Task.FromResult(e ?? milestone); }

    public Task<ProjectMember> AddMemberAsync(Guid projectId, ProjectMember member) { member.Id = Guid.NewGuid(); member.ProjectId = projectId; member.JoinedAt = DateTime.UtcNow; _members.Add(member); return Task.FromResult(member); }
    public Task<IEnumerable<ProjectMember>> GetMembersAsync(Guid projectId) => Task.FromResult(_members.Where(m => m.ProjectId == projectId));
    public Task RemoveMemberAsync(Guid memberId) { _members.RemoveAll(m => m.Id == memberId); return Task.CompletedTask; }

    public Task<TimeEntry> LogTimeAsync(TimeEntry entry) { entry.Id = Guid.NewGuid(); entry.CreatedAt = DateTime.UtcNow; _timeEntries.Add(entry); return Task.FromResult(entry); }
    public Task<IEnumerable<TimeEntry>> GetTimeEntriesAsync(Guid? taskId = null, Guid? userId = null)
    {
        var query = _timeEntries.AsEnumerable();
        if (taskId.HasValue) query = query.Where(t => t.TaskId == taskId.Value);
        if (userId.HasValue) query = query.Where(t => t.UserId == userId.Value);
        return Task.FromResult(query);
    }

    public Task<TaskComment> AddCommentAsync(TaskComment comment) { comment.Id = Guid.NewGuid(); comment.CreatedAt = DateTime.UtcNow; _comments.Add(comment); return Task.FromResult(comment); }
    public Task<Board> GetBoardAsync(Guid projectId) => Task.FromResult(_boards.FirstOrDefault(b => b.ProjectId == projectId) ?? new Board());
    public Task<Board> UpdateBoardAsync(Board board) { var e = _boards.FirstOrDefault(b => b.Id == board.Id); if (e != null) e.Columns = board.Columns; return Task.FromResult(e ?? board); }

    public Task<ProjectStatistics> GetStatisticsAsync(Guid? projectId = null, string? department = null)
    {
        var projects = _projects.AsEnumerable();
        if (projectId.HasValue) projects = projects.Where(p => p.Id == projectId.Value);
        if (!string.IsNullOrEmpty(department)) projects = projects.Where(p => p.Department == department);
        var pList = projects.ToList();
        var tasks = projectId.HasValue ? _tasks.Where(t => t.ProjectId == projectId.Value).ToList() : _tasks;

        return Task.FromResult(new ProjectStatistics {
            TotalProjects = pList.Count, ActiveProjects = pList.Count(p => p.Status == ProjectStatus.Active),
            CompletedProjects = pList.Count(p => p.Status == ProjectStatus.Completed), TotalTasks = tasks.Count,
            CompletedTasks = tasks.Count(t => t.Status == TaskStatus.Done),
            OverdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Done),
            TotalBudget = pList.Sum(p => p.Budget), TotalSpent = pList.Sum(p => p.ActualCost),
            AverageProgress = pList.Any() ? pList.Average(p => p.ProgressPercent) : 0,
            TasksByStatus = tasks.GroupBy(t => t.Status.ToString()).ToDictionary(g => g.Key, g => g.Count())
        });
    }
}
