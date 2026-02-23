using SpikeOffice.Core.Enums;

namespace SpikeOffice.Core.Entities;

/// <summary>Task for Kanban board.</summary>
public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Todo;
    public int SortOrder { get; set; }
    public Guid? AssigneeId { get; set; }
    public Employee? Assignee { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public Guid? BoardId { get; set; }
    public TaskBoard? Board { get; set; }
}
