namespace Mamey.FWID.Sagas.Models;

/// <summary>
/// Saga - Long-running workflow with compensation support
/// </summary>
public class Saga
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SagaId { get; set; } = string.Empty;
    public string SagaType { get; set; } = string.Empty;
    public SagaStatus Status { get; set; } = SagaStatus.Created;
    
    // Steps
    public List<SagaStep> Steps { get; set; } = new();
    public int CurrentStepIndex { get; set; }
    public string? CurrentStepName { get; set; }
    
    // State
    public Dictionary<string, object> State { get; set; } = new();
    public string? StateJson { get; set; }
    
    // Context
    public string? InitiatorId { get; set; }
    public string? CorrelationId { get; set; }
    public string? ParentSagaId { get; set; }
    
    // Timing
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Timeout { get; set; }
    
    // Error handling
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; } = 3;
}

public class SagaStep
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SagaId { get; set; }
    public int StepIndex { get; set; }
    public string StepName { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Execution
    public string ActionType { get; set; } = string.Empty;
    public string? ActionData { get; set; }
    public string? ServiceEndpoint { get; set; }
    
    // Compensation
    public bool HasCompensation { get; set; }
    public string? CompensationActionType { get; set; }
    public string? CompensationData { get; set; }
    
    // Status
    public StepStatus Status { get; set; } = StepStatus.Pending;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Results
    public string? Output { get; set; }
    public string? ErrorMessage { get; set; }
    public bool WasCompensated { get; set; }
}

public class SagaDefinition
{
    public string SagaType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<StepDefinition> Steps { get; set; } = new();
    public TimeSpan? DefaultTimeout { get; set; }
    public int MaxRetries { get; set; } = 3;
}

public class StepDefinition
{
    public string StepName { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string? ServiceEndpoint { get; set; }
    public string? CompensationActionType { get; set; }
    public bool IsOptional { get; set; }
}

// Predefined saga types
public static class SagaTypes
{
    public const string KYC_VERIFICATION = "KYC_VERIFICATION";
    public const string MEMBERSHIP_UPGRADE = "MEMBERSHIP_UPGRADE";
    public const string TREASURY_DISBURSEMENT = "TREASURY_DISBURSEMENT";
    public const string IDENTITY_REGISTRATION = "IDENTITY_REGISTRATION";
    public const string TOKEN_CREATION = "TOKEN_CREATION";
    public const string BRIDGE_TRANSFER = "BRIDGE_TRANSFER";
    public const string GOVERNANCE_PROPOSAL = "GOVERNANCE_PROPOSAL";
}

public enum SagaStatus { Created, Running, Completed, Failed, Compensating, Compensated, Cancelled, TimedOut }
public enum StepStatus { Pending, Running, Completed, Failed, Skipped, Compensating, Compensated }
