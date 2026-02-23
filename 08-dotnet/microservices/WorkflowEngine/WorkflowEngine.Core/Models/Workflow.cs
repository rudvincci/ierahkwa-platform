namespace WorkflowEngine.Core.Models;

public class Workflow
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;
    
    public List<WorkflowNode> Nodes { get; set; } = new();
    public List<WorkflowConnection> Connections { get; set; } = new();
    
    public string? TriggerType { get; set; } // HTTP, Schedule, Event
    public string? TriggerConfig { get; set; }
    
    public int ExecutionCount { get; set; }
    public decimal SuccessRate { get; set; } = 100;
    public TimeSpan AverageExecutionTime { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastExecutedAt { get; set; }
}

public enum WorkflowStatus
{
    Draft,
    Running,
    Paused,
    Stopped,
    Error
}

public class WorkflowNode
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty; // Trigger, Action, Condition, End
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public Dictionary<string, object> Config { get; set; } = new();
    public int PositionX { get; set; }
    public int PositionY { get; set; }
}

public class WorkflowConnection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SourceNodeId { get; set; } = string.Empty;
    public string TargetNodeId { get; set; } = string.Empty;
    public string? Condition { get; set; }
}

public class WorkflowExecution
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WorkflowId { get; set; } = string.Empty;
    public string WorkflowName { get; set; } = string.Empty;
    public ExecutionStatus Status { get; set; } = ExecutionStatus.Running;
    public Dictionary<string, object> Input { get; set; } = new();
    public Dictionary<string, object> Output { get; set; } = new();
    public List<ExecutionLog> Logs { get; set; } = new();
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt - StartedAt : null;
}

public enum ExecutionStatus
{
    Running,
    Completed,
    Failed,
    Cancelled
}

public class ExecutionLog
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string NodeId { get; set; } = string.Empty;
    public string NodeName { get; set; } = string.Empty;
    public LogLevel Level { get; set; } = LogLevel.Info;
    public string Message { get; set; } = string.Empty;
}

public enum LogLevel
{
    Info,
    Success,
    Warning,
    Error
}

public class WorkflowStats
{
    public int TotalWorkflows { get; set; }
    public int ActiveWorkflows { get; set; }
    public int TotalExecutions { get; set; }
    public decimal OverallSuccessRate { get; set; }
    public TimeSpan AverageExecutionTime { get; set; }
}
