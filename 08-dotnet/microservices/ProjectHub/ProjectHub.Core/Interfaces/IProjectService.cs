using ProjectHub.Core.Models;

namespace ProjectHub.Core.Interfaces;

public interface IProjectService
{
    Task<Project> CreateProjectAsync(Project project);
    Task<Project?> GetProjectByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetProjectsAsync(string? department = null, ProjectStatus? status = null);
    Task<IEnumerable<Project>> GetUserProjectsAsync(Guid userId);
    Task<Project> UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Guid id);

    Task<ProjectTask> CreateTaskAsync(ProjectTask task);
    Task<ProjectTask?> GetTaskByIdAsync(Guid id);
    Task<IEnumerable<ProjectTask>> GetProjectTasksAsync(Guid projectId);
    Task<IEnumerable<ProjectTask>> GetUserTasksAsync(Guid userId);
    Task<ProjectTask> UpdateTaskAsync(ProjectTask task);
    Task<ProjectTask> MoveTaskAsync(Guid taskId, string column, int orderIndex);
    Task DeleteTaskAsync(Guid id);

    Task<ProjectMilestone> CreateMilestoneAsync(ProjectMilestone milestone);
    Task<IEnumerable<ProjectMilestone>> GetMilestonesAsync(Guid projectId);
    Task<ProjectMilestone> UpdateMilestoneAsync(ProjectMilestone milestone);

    Task<ProjectMember> AddMemberAsync(Guid projectId, ProjectMember member);
    Task<IEnumerable<ProjectMember>> GetMembersAsync(Guid projectId);
    Task RemoveMemberAsync(Guid memberId);

    Task<TimeEntry> LogTimeAsync(TimeEntry entry);
    Task<IEnumerable<TimeEntry>> GetTimeEntriesAsync(Guid? taskId = null, Guid? userId = null);

    Task<TaskComment> AddCommentAsync(TaskComment comment);
    Task<Board> GetBoardAsync(Guid projectId);
    Task<Board> UpdateBoardAsync(Board board);

    Task<ProjectStatistics> GetStatisticsAsync(Guid? projectId = null, string? department = null);
}

public class ProjectStatistics
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalSpent { get; set; }
    public double AverageProgress { get; set; }
    public Dictionary<string, int> TasksByStatus { get; set; } = new();
    public Dictionary<string, int> ProjectsByDepartment { get; set; } = new();
}
