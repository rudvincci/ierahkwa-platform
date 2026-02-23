namespace DocumentFlow.Core.Models;

public class DocumentWorkflow
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid WorkflowTemplateId { get; set; }
    public string WorkflowName { get; set; } = string.Empty;
    public WorkflowStatus Status { get; set; }
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public Guid InitiatedBy { get; set; }
    public string? InitiatedByName { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Comments { get; set; }

    // Navigation
    public Document? Document { get; set; }
    public WorkflowTemplate? Template { get; set; }
    public List<WorkflowStep> Steps { get; set; } = new();
}

public class WorkflowTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Department { get; set; }
    public bool IsActive { get; set; } = true;
    public string StepsDefinition { get; set; } = "[]"; // JSON array of step definitions
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WorkflowStep
{
    public Guid Id { get; set; }
    public Guid WorkflowId { get; set; }
    public int StepNumber { get; set; }
    public string StepName { get; set; } = string.Empty;
    public WorkflowStepType StepType { get; set; }
    public Guid? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public Guid? AssignedRole { get; set; }
    public string? AssignedRoleName { get; set; }
    public WorkflowStepStatus Status { get; set; }
    public string? Action { get; set; }
    public string? Comments { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? CompletedBy { get; set; }
    public string? CompletedByName { get; set; }

    // Navigation
    public DocumentWorkflow? Workflow { get; set; }
}

public enum WorkflowStatus
{
    Draft,
    InProgress,
    Completed,
    Cancelled,
    OnHold,
    Rejected
}

public enum WorkflowStepType
{
    Approval,
    Review,
    Signature,
    Notification,
    Task,
    Conditional,
    Parallel
}

public enum WorkflowStepStatus
{
    Pending,
    InProgress,
    Completed,
    Skipped,
    Rejected,
    Delegated
}
