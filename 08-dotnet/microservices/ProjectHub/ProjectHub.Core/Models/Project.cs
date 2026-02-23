namespace ProjectHub.Core.Models;

public class Project
{
    public Guid Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectType Type { get; set; }
    public ProjectStatus Status { get; set; }
    public ProjectPriority Priority { get; set; }
    public string? Department { get; set; }
    public Guid ManagerId { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public Guid? SponsorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public int ProgressPercent { get; set; }
    public string? Color { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<ProjectTask> Tasks { get; set; } = new();
    public List<ProjectMember> Members { get; set; } = new();
    public List<ProjectMilestone> Milestones { get; set; } = new();
}

public class ProjectTask
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public string TaskCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    public int ProgressPercent { get; set; }
    public int OrderIndex { get; set; }
    public string? BoardColumn { get; set; }
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskComment> Comments { get; set; } = new();
    public List<TaskAttachment> Attachments { get; set; } = new();
}

public class ProjectMilestone
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public MilestoneStatus Status { get; set; }
    public decimal? Budget { get; set; }
    public List<Guid> TaskIds { get; set; } = new();
}

public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public ProjectRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public int AllocatedHours { get; set; }
}

public class TaskComment
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TaskAttachment
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class Board
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<BoardColumn> Columns { get; set; } = new();
}

public class BoardColumn
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#3B82F6";
    public int OrderIndex { get; set; }
    public int? WipLimit { get; set; }
}

public class TimeEntry
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
    public bool Billable { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum ProjectType { Internal, External, Government, Infrastructure, IT, Research, Construction, Other }
public enum ProjectStatus { Planning, Active, OnHold, Completed, Cancelled, Archived }
public enum ProjectPriority { Low, Medium, High, Critical }
public enum TaskStatus { Backlog, Todo, InProgress, Review, Done, Cancelled }
public enum TaskPriority { Low, Medium, High, Urgent }
public enum MilestoneStatus { Pending, InProgress, Completed, Missed }
public enum ProjectRole { Manager, Lead, Member, Viewer, Stakeholder }
