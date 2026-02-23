using WorkflowEngine.Core.Interfaces;
using WorkflowEngine.Core.Models;

namespace WorkflowEngine.Infrastructure.Services;

public class WorkflowService : IWorkflowService
{
    private static readonly List<Workflow> _workflows = new()
    {
        new Workflow
        {
            Id = "WF-001",
            Name = "Citizen Onboarding",
            Description = "Automated workflow for new citizen registration",
            Category = "Citizen Management",
            Status = WorkflowStatus.Running,
            ExecutionCount = 2456,
            SuccessRate = 99.5m,
            AverageExecutionTime = TimeSpan.FromSeconds(12),
            Nodes = new List<WorkflowNode>
            {
                new() { Id = "N1", Type = "Trigger", Name = "New Registration", Icon = "📥" },
                new() { Id = "N2", Type = "Action", Name = "Validate Data", Icon = "✅" },
                new() { Id = "N3", Type = "Condition", Name = "Valid?", Icon = "❓" },
                new() { Id = "N4", Type = "Action", Name = "Assign AI", Icon = "🤖" },
                new() { Id = "N5", Type = "Action", Name = "Send Welcome", Icon = "📧" },
                new() { Id = "N6", Type = "End", Name = "Complete", Icon = "🎉" }
            }
        },
        new Workflow
        {
            Id = "WF-002",
            Name = "Payment Processing",
            Description = "Process payments and transactions",
            Category = "Financial",
            Status = WorkflowStatus.Running,
            ExecutionCount = 5678,
            SuccessRate = 99.8m,
            AverageExecutionTime = TimeSpan.FromSeconds(3)
        },
        new Workflow
        {
            Id = "WF-003",
            Name = "Document Approval",
            Description = "Multi-step document approval workflow",
            Category = "Documents",
            Status = WorkflowStatus.Paused,
            ExecutionCount = 1234,
            SuccessRate = 98.2m,
            AverageExecutionTime = TimeSpan.FromMinutes(5)
        }
    };

    private static readonly List<WorkflowExecution> _executions = new();

    public Task<IEnumerable<Workflow>> GetAllAsync() => Task.FromResult(_workflows.AsEnumerable());

    public Task<IEnumerable<Workflow>> GetActiveAsync() => 
        Task.FromResult(_workflows.Where(w => w.Status == WorkflowStatus.Running));

    public Task<Workflow?> GetByIdAsync(string id) => 
        Task.FromResult(_workflows.FirstOrDefault(w => w.Id == id));

    public Task<Workflow> CreateAsync(Workflow workflow)
    {
        workflow.Id = $"WF-{(_workflows.Count + 1):D3}";
        _workflows.Add(workflow);
        return Task.FromResult(workflow);
    }

    public Task UpdateAsync(Workflow workflow)
    {
        var index = _workflows.FindIndex(w => w.Id == workflow.Id);
        if (index >= 0) _workflows[index] = workflow;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        _workflows.RemoveAll(w => w.Id == id);
        return Task.CompletedTask;
    }

    public Task<WorkflowExecution> ExecuteAsync(string workflowId, Dictionary<string, object> input)
    {
        var workflow = _workflows.FirstOrDefault(w => w.Id == workflowId);
        var execution = new WorkflowExecution
        {
            WorkflowId = workflowId,
            WorkflowName = workflow?.Name ?? "Unknown",
            Input = input,
            Logs = new List<ExecutionLog>
            {
                new() { NodeName = "Start", Level = LogLevel.Info, Message = "Workflow execution started" },
                new() { NodeName = "Validate", Level = LogLevel.Success, Message = "Data validation passed" },
                new() { NodeName = "Process", Level = LogLevel.Info, Message = "Processing..." }
            }
        };
        
        // Simulate completion
        execution.Status = ExecutionStatus.Completed;
        execution.CompletedAt = DateTime.UtcNow.AddSeconds(2);
        execution.Logs.Add(new ExecutionLog { NodeName = "Complete", Level = LogLevel.Success, Message = "Workflow completed successfully" });
        
        _executions.Add(execution);
        
        if (workflow != null)
        {
            workflow.ExecutionCount++;
            workflow.LastExecutedAt = DateTime.UtcNow;
        }
        
        return Task.FromResult(execution);
    }

    public Task PauseAsync(string id)
    {
        var workflow = _workflows.FirstOrDefault(w => w.Id == id);
        if (workflow != null) workflow.Status = WorkflowStatus.Paused;
        return Task.CompletedTask;
    }

    public Task ResumeAsync(string id)
    {
        var workflow = _workflows.FirstOrDefault(w => w.Id == id);
        if (workflow != null) workflow.Status = WorkflowStatus.Running;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<WorkflowExecution>> GetExecutionsAsync(string workflowId) =>
        Task.FromResult(_executions.Where(e => e.WorkflowId == workflowId));

    public Task<WorkflowStats> GetStatsAsync()
    {
        return Task.FromResult(new WorkflowStats
        {
            TotalWorkflows = _workflows.Count,
            ActiveWorkflows = _workflows.Count(w => w.Status == WorkflowStatus.Running),
            TotalExecutions = _workflows.Sum(w => w.ExecutionCount),
            OverallSuccessRate = 99.2m,
            AverageExecutionTime = TimeSpan.FromSeconds(5)
        });
    }
}
