using WorkflowEngine.Core.Models;

namespace WorkflowEngine.Core.Interfaces;

public interface IWorkflowService
{
    Task<IEnumerable<Workflow>> GetAllAsync();
    Task<IEnumerable<Workflow>> GetActiveAsync();
    Task<Workflow?> GetByIdAsync(string id);
    Task<Workflow> CreateAsync(Workflow workflow);
    Task UpdateAsync(Workflow workflow);
    Task DeleteAsync(string id);
    Task<WorkflowExecution> ExecuteAsync(string workflowId, Dictionary<string, object> input);
    Task PauseAsync(string id);
    Task ResumeAsync(string id);
    Task<IEnumerable<WorkflowExecution>> GetExecutionsAsync(string workflowId);
    Task<WorkflowStats> GetStatsAsync();
}
