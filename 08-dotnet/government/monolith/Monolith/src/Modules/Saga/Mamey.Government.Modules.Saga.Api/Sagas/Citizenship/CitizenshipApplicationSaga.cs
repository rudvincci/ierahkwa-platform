using System;
using System.Threading.Tasks;
using Chronicle;
using Mamey.Government.Modules.Saga.Api.Messages.Citizens;
using Mamey.Government.Modules.Saga.Api.Messages.Citizenship;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.MicroMonolith.Abstractions.Time;

namespace Mamey.Government.Modules.Saga.Api.Sagas.Citizenship;

/// <summary>
/// 9-step citizenship application saga workflow:
/// 1. Submit - Application submission
/// 2. Validate - Data validation
/// 3. KYC - AI-powered KYC processing
/// 4. Agent Review - Manual agent review
/// 5. Approve - Application approval
/// 6. Create Citizen - CitizenManagement creates citizen
/// 7. Issue Passport - PassportManagement issues passport
/// 8. Issue Travel ID - TravelIdentityManagement issues ID
/// 9. Create Payment Plan - PaymentService creates schedule
/// </summary>
internal sealed class CitizenshipApplicationSagaData
{
    public Guid ApplicationId { get; set; }
    public Guid ApplicantId { get; set; }
    public bool IsValidated { get; set; }
    public bool IsKycCompleted { get; set; }
    public bool IsKycApproved { get; set; }
    public bool IsAgentReviewed { get; set; }
    public bool IsApproved { get; set; }
    public Guid? CitizenId { get; set; }
    public Guid? PassportId { get; set; }
    public Guid? TravelIdentityId { get; set; }
    public Guid? PaymentPlanId { get; set; }
    public string? RejectionReason { get; set; }
}

internal sealed class CitizenshipApplicationSaga : Saga<CitizenshipApplicationSagaData>,
    ISagaStartAction<ApplicationSubmittedEvent>,
    ISagaAction<ApplicationValidated>,
    // ISagaAction<KycCompleted>,
    ISagaAction<ApplicationApprovedEvent>,
    ISagaAction<ApplicationRejectedEvent>//,
    // ISagaAction<CitizenCreatedEvent>,
    // ISagaAction<PassportIssued>,
    // ISagaAction<TravelIdentityIssued>,
    // ISagaAction<PaymentPlanCreated>
{
    private readonly IMessageBroker _messageBroker;
    private readonly IClock _clock;

    public CitizenshipApplicationSaga(IMessageBroker messageBroker, IClock clock)
    {
        _messageBroker = messageBroker;
        _clock = clock;
    }

    public override SagaId ResolveId(object message, ISagaContext context)
        => message switch
        {
            ApplicationSubmittedEvent m => m.ApplicationId.ToString(),
            ApplicationValidated m => m.ApplicationId.ToString(),
            KycCompleted m => m.ApplicationId.ToString(),
            ApplicationApprovedEvent m => m.ApplicationId.ToString(),
            ApplicationRejectedEvent m => m.ApplicationId.ToString(),
            // CitizenCreated m => m.ApplicationId.ToString(),
            PassportIssued m => m.ApplicationId.ToString(),
            TravelIdentityIssued m => m.ApplicationId.ToString(),
            PaymentPlanCreated m => m.ApplicationId.ToString(),
            _ => base.ResolveId(message, context)
        };

    // Step 1: Application Submitted
    public Task HandleAsync(ApplicationSubmittedEvent message, ISagaContext context)
    {
        Data.ApplicationId = message.ApplicationId;
        // Data.ApplicantId = message.ApplicantId ?? Guid.Empty;
        
        // Trigger validation step
        // This will be handled by the CitizenshipApplications module
        return Task.CompletedTask;
    }

    public Task CompensateAsync(ApplicationSubmittedEvent message, ISagaContext context)
    {
        // Compensation: Mark application as withdrawn
        return Task.CompletedTask;
    }

    // Step 2: Application Validated
    public Task HandleAsync(ApplicationValidated message, ISagaContext context)
    {
        Data.IsValidated = true;
        
        // Trigger KYC step
        // This will be handled by the CitizenshipApplications module
        return Task.CompletedTask;
    }

    public Task CompensateAsync(ApplicationValidated message, ISagaContext context)
    {
        // Compensation: Revert validation status
        return Task.CompletedTask;
    }

    // Step 3: KYC Completed
    public async Task HandleAsync(KycCompleted message, ISagaContext context)
    {
        Data.IsKycCompleted = true;
        Data.IsKycApproved = message.IsApproved;

        if (!message.IsApproved)
        {
            // KYC failed - reject application
            Data.RejectionReason = message.RejectionReason ?? "KYC verification failed";
            await CompleteAsync();
            return;
        }

        // KYC approved - proceed to agent review
        // This will be handled by the CitizenshipApplications module
    }

    public Task CompensateAsync(KycCompleted message, ISagaContext context)
    {
        // Compensation: Revert KYC status
        return Task.CompletedTask;
    }

    // Step 4 & 5: Application Approved (after agent review)
    public async Task HandleAsync(ApplicationApprovedEvent message, ISagaContext context)
    {
        Data.IsApproved = true;
        
        // Step 6: Create Citizen
        // This will trigger the Citizens module to create a citizen record
        // The Citizens module will publish CitizenCreated event
    }

    public Task CompensateAsync(ApplicationApprovedEvent message, ISagaContext context)
    {
        // Compensation: Revert approval, mark as rejected
        return Task.CompletedTask;
    }

    // Step 5 (alternative): Application Rejected
    public Task HandleAsync(ApplicationRejectedEvent message, ISagaContext context)
    {
        Data.RejectionReason = message.Reason;
        return CompleteAsync();
    }

    public Task CompensateAsync(ApplicationRejectedEvent message, ISagaContext context)
    {
        return Task.CompletedTask;
    }

    // Step 6: Citizen Created
    public async Task HandleAsync(CitizenCreatedEvent message, ISagaContext context)
    {
        Data.CitizenId = message.CitizenId;
        
        // Step 7: Issue Passport (mandatory)
        // This will trigger the Passports module
        // The Passports module will publish PassportIssued event
    }

    public Task CompensateAsync(CitizenCreatedEvent message, ISagaContext context)
    {
        // Compensation: Delete citizen record
        return Task.CompletedTask;
    }

    // Step 7: Passport Issued
    public async Task HandleAsync(PassportIssued message, ISagaContext context)
    {
        Data.PassportId = message.PassportId;
        
        // Step 8: Issue Travel ID
        // This will trigger the TravelIdentities module
        // The TravelIdentities module will publish TravelIdentityIssued event
    }

    public Task CompensateAsync(PassportIssued message, ISagaContext context)
    {
        // Compensation: Revoke passport
        return Task.CompletedTask;
    }

    // Step 8: Travel Identity Issued
    public async Task HandleAsync(TravelIdentityIssued message, ISagaContext context)
    {
        Data.TravelIdentityId = message.TravelIdentityId;
        
        // Step 9: Create Payment Plan
        // This will trigger the Payments module
        // The Payments module will publish PaymentPlanCreated event
    }

    public Task CompensateAsync(TravelIdentityIssued message, ISagaContext context)
    {
        // Compensation: Revoke travel identity
        return Task.CompletedTask;
    }

    // Step 9: Payment Plan Created
    public Task HandleAsync(PaymentPlanCreated message, ISagaContext context)
    {
        Data.PaymentPlanId = message.PaymentPlanId;
        
        // All steps completed - saga is complete
        return CompleteAsync();
    }

    public Task CompensateAsync(PaymentPlanCreated message, ISagaContext context)
    {
        // Compensation: Cancel payment plan
        return Task.CompletedTask;
    }
}
