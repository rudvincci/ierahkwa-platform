using System.Text.Json;
using Mamey.FWID.Sagas.Models;

namespace Mamey.FWID.Sagas.Services;

public interface ISagaService
{
    Task<Saga> CreateAsync(string sagaType, Dictionary<string, object> initialState, string? initiatorId = null);
    Task<Saga?> GetAsync(Guid id);
    Task<Saga?> GetByCorrelationIdAsync(string correlationId);
    Task<IReadOnlyList<Saga>> GetByStatusAsync(SagaStatus status);
    Task<bool> StartAsync(Guid id);
    Task<bool> ExecuteStepAsync(Guid id);
    Task<bool> CompleteStepAsync(Guid id, int stepIndex, string? output = null);
    Task<bool> FailStepAsync(Guid id, int stepIndex, string errorMessage);
    Task<bool> CompensateAsync(Guid id);
    Task<bool> CancelAsync(Guid id);
    Task<SagaDefinition> RegisterDefinitionAsync(SagaDefinition definition);
    Task<SagaDefinition?> GetDefinitionAsync(string sagaType);
}

public class SagaService : ISagaService
{
    private readonly Dictionary<Guid, Saga> _sagas = new();
    private readonly Dictionary<string, SagaDefinition> _definitions = new();
    private long _counter = 1000000;

    public SagaService()
    {
        InitializeDefaultDefinitions();
    }

    private void InitializeDefaultDefinitions()
    {
        // KYC Verification Saga
        _definitions[SagaTypes.KYC_VERIFICATION] = new SagaDefinition
        {
            SagaType = SagaTypes.KYC_VERIFICATION,
            Name = "KYC Verification Process",
            Steps = new()
            {
                new() { StepName = "ValidateDocuments", ActionType = "VALIDATE_DOCUMENTS", CompensationActionType = "REJECT_DOCUMENTS" },
                new() { StepName = "VerifyIdentity", ActionType = "VERIFY_IDENTITY", CompensationActionType = "REVOKE_VERIFICATION" },
                new() { StepName = "CheckAML", ActionType = "CHECK_AML", CompensationActionType = "FLAG_AML" },
                new() { StepName = "ApproveKYC", ActionType = "APPROVE_KYC", CompensationActionType = "REJECT_KYC" },
                new() { StepName = "NotifyUser", ActionType = "SEND_NOTIFICATION", IsOptional = true }
            }
        };

        // Membership Upgrade Saga
        _definitions[SagaTypes.MEMBERSHIP_UPGRADE] = new SagaDefinition
        {
            SagaType = SagaTypes.MEMBERSHIP_UPGRADE,
            Name = "Membership Tier Upgrade",
            Steps = new()
            {
                new() { StepName = "ValidatePayment", ActionType = "VALIDATE_PAYMENT", CompensationActionType = "REFUND_PAYMENT" },
                new() { StepName = "ProcessPayment", ActionType = "PROCESS_PAYMENT", CompensationActionType = "REVERSE_PAYMENT" },
                new() { StepName = "UpgradeTier", ActionType = "UPGRADE_TIER", CompensationActionType = "DOWNGRADE_TIER" },
                new() { StepName = "UpdateBenefits", ActionType = "UPDATE_BENEFITS", CompensationActionType = "REVERT_BENEFITS" },
                new() { StepName = "NotifyUpgrade", ActionType = "SEND_NOTIFICATION" }
            }
        };

        // Treasury Disbursement Saga
        _definitions[SagaTypes.TREASURY_DISBURSEMENT] = new SagaDefinition
        {
            SagaType = SagaTypes.TREASURY_DISBURSEMENT,
            Name = "Treasury Disbursement Process",
            Steps = new()
            {
                new() { StepName = "ValidateOperation", ActionType = "VALIDATE_TREASURY_OP" },
                new() { StepName = "CheckCompliance", ActionType = "CHECK_TREATY_COMPLIANCE" },
                new() { StepName = "GetApprovals", ActionType = "COLLECT_APPROVALS", CompensationActionType = "CANCEL_APPROVALS" },
                new() { StepName = "ExecuteTransfer", ActionType = "EXECUTE_TRANSFER", CompensationActionType = "REVERSE_TRANSFER" },
                new() { StepName = "RecordAudit", ActionType = "RECORD_AUDIT_TRAIL" },
                new() { StepName = "NotifyParties", ActionType = "SEND_NOTIFICATIONS" }
            }
        };

        // Identity Registration Saga
        _definitions[SagaTypes.IDENTITY_REGISTRATION] = new SagaDefinition
        {
            SagaType = SagaTypes.IDENTITY_REGISTRATION,
            Name = "New Citizen Registration",
            Steps = new()
            {
                new() { StepName = "CreateIdentity", ActionType = "CREATE_IDENTITY", CompensationActionType = "DELETE_IDENTITY" },
                new() { StepName = "GenerateFWID", ActionType = "GENERATE_FWID" },
                new() { StepName = "CreateWallet", ActionType = "CREATE_WALLET", CompensationActionType = "DEACTIVATE_WALLET" },
                new() { StepName = "AssignMembership", ActionType = "ASSIGN_MEMBERSHIP" },
                new() { StepName = "SendWelcome", ActionType = "SEND_WELCOME_NOTIFICATION" }
            }
        };
    }

    public async Task<Saga> CreateAsync(string sagaType, Dictionary<string, object> initialState, string? initiatorId = null)
    {
        var definition = await GetDefinitionAsync(sagaType);
        if (definition == null)
            throw new ArgumentException($"Unknown saga type: {sagaType}");

        var id = Interlocked.Increment(ref _counter);
        var saga = new Saga
        {
            SagaId = $"SAGA-{sagaType}-{id}",
            SagaType = sagaType,
            State = initialState,
            StateJson = JsonSerializer.Serialize(initialState),
            InitiatorId = initiatorId,
            CorrelationId = Guid.NewGuid().ToString(),
            Timeout = definition.DefaultTimeout,
            MaxRetries = definition.MaxRetries
        };

        // Create steps from definition
        for (int i = 0; i < definition.Steps.Count; i++)
        {
            var stepDef = definition.Steps[i];
            saga.Steps.Add(new SagaStep
            {
                SagaId = saga.Id,
                StepIndex = i,
                StepName = stepDef.StepName,
                ActionType = stepDef.ActionType,
                ServiceEndpoint = stepDef.ServiceEndpoint,
                HasCompensation = !string.IsNullOrEmpty(stepDef.CompensationActionType),
                CompensationActionType = stepDef.CompensationActionType
            });
        }

        _sagas[saga.Id] = saga;
        return saga;
    }

    public Task<Saga?> GetAsync(Guid id)
    {
        _sagas.TryGetValue(id, out var saga);
        return Task.FromResult(saga);
    }

    public Task<Saga?> GetByCorrelationIdAsync(string correlationId)
    {
        var saga = _sagas.Values.FirstOrDefault(s => s.CorrelationId == correlationId);
        return Task.FromResult(saga);
    }

    public Task<IReadOnlyList<Saga>> GetByStatusAsync(SagaStatus status)
    {
        var sagas = _sagas.Values.Where(s => s.Status == status).ToList();
        return Task.FromResult<IReadOnlyList<Saga>>(sagas);
    }

    public async Task<bool> StartAsync(Guid id)
    {
        var saga = await GetAsync(id);
        if (saga == null || saga.Status != SagaStatus.Created)
            return false;

        saga.Status = SagaStatus.Running;
        saga.StartedAt = DateTime.UtcNow;
        saga.CurrentStepIndex = 0;
        saga.CurrentStepName = saga.Steps.FirstOrDefault()?.StepName;

        return true;
    }

    public async Task<bool> ExecuteStepAsync(Guid id)
    {
        var saga = await GetAsync(id);
        if (saga == null || saga.Status != SagaStatus.Running)
            return false;

        if (saga.CurrentStepIndex >= saga.Steps.Count)
        {
            saga.Status = SagaStatus.Completed;
            saga.CompletedAt = DateTime.UtcNow;
            return true;
        }

        var step = saga.Steps[saga.CurrentStepIndex];
        step.Status = StepStatus.Running;
        step.StartedAt = DateTime.UtcNow;

        // Simulate step execution
        await Task.Delay(100);

        return true;
    }

    public async Task<bool> CompleteStepAsync(Guid id, int stepIndex, string? output = null)
    {
        var saga = await GetAsync(id);
        if (saga == null || stepIndex >= saga.Steps.Count)
            return false;

        var step = saga.Steps[stepIndex];
        step.Status = StepStatus.Completed;
        step.CompletedAt = DateTime.UtcNow;
        step.Output = output;

        // Move to next step
        saga.CurrentStepIndex = stepIndex + 1;
        if (saga.CurrentStepIndex < saga.Steps.Count)
            saga.CurrentStepName = saga.Steps[saga.CurrentStepIndex].StepName;
        else
        {
            saga.Status = SagaStatus.Completed;
            saga.CompletedAt = DateTime.UtcNow;
        }

        return true;
    }

    public async Task<bool> FailStepAsync(Guid id, int stepIndex, string errorMessage)
    {
        var saga = await GetAsync(id);
        if (saga == null || stepIndex >= saga.Steps.Count)
            return false;

        var step = saga.Steps[stepIndex];
        step.Status = StepStatus.Failed;
        step.ErrorMessage = errorMessage;

        saga.ErrorMessage = errorMessage;

        if (saga.RetryCount < saga.MaxRetries)
        {
            saga.RetryCount++;
            step.Status = StepStatus.Pending;
        }
        else
        {
            saga.Status = SagaStatus.Failed;
            await CompensateAsync(id);
        }

        return true;
    }

    public async Task<bool> CompensateAsync(Guid id)
    {
        var saga = await GetAsync(id);
        if (saga == null)
            return false;

        saga.Status = SagaStatus.Compensating;

        // Compensate completed steps in reverse order
        for (int i = saga.CurrentStepIndex - 1; i >= 0; i--)
        {
            var step = saga.Steps[i];
            if (step.Status == StepStatus.Completed && step.HasCompensation)
            {
                step.Status = StepStatus.Compensating;
                // Execute compensation action
                await Task.Delay(50);
                step.Status = StepStatus.Compensated;
                step.WasCompensated = true;
            }
        }

        saga.Status = SagaStatus.Compensated;
        return true;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var saga = await GetAsync(id);
        if (saga == null)
            return false;

        if (saga.Status == SagaStatus.Running)
            await CompensateAsync(id);

        saga.Status = SagaStatus.Cancelled;
        return true;
    }

    public Task<SagaDefinition> RegisterDefinitionAsync(SagaDefinition definition)
    {
        _definitions[definition.SagaType] = definition;
        return Task.FromResult(definition);
    }

    public Task<SagaDefinition?> GetDefinitionAsync(string sagaType)
    {
        _definitions.TryGetValue(sagaType, out var definition);
        return Task.FromResult(definition);
    }
}
