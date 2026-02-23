using System;
using System.Threading.Tasks;
using Chronicle;
using Mamey.Government.Modules.Saga.Api.Messages.Citizenship;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.MicroMonolith.Abstractions.Time;

namespace Mamey.Government.Modules.Saga.Api.Sagas.Citizenship;

/// <summary>
/// Saga for citizenship status progression:
/// Probationary (2 years) -> Resident (3 years) -> Citizen (permanent)
/// </summary>
internal sealed class StatusProgressionSagaData
{
    public Guid CitizenId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public bool ApplicationApproved { get; set; }
    public bool DocumentsReissued { get; set; }
}

internal sealed class StatusProgressionSaga : Saga<StatusProgressionSagaData>,
    ISagaStartAction<StatusProgressionRequested>,
    ISagaAction<ApplicationApprovedEvent>//,
    // ISagaAction<PassportIssued>,
    // ISagaAction<TravelIdentityIssued>
{
    private readonly IMessageBroker _messageBroker;
    private readonly IClock _clock;

    public StatusProgressionSaga(IMessageBroker messageBroker, IClock clock)
    {
        _messageBroker = messageBroker;
        _clock = clock;
    }

    public override SagaId ResolveId(object message, ISagaContext context)
        => message switch
        {
            StatusProgressionRequested m => m.CitizenId.ToString(),
            ApplicationApprovedEvent m => m.ApplicationId.ToString(), // Will need to link via CitizenId
            PassportIssued m => m.CitizenId.ToString(),
            TravelIdentityIssued m => m.CitizenId.ToString(),
            _ => base.ResolveId(message, context)
        };

    public Task HandleAsync(StatusProgressionRequested message, ISagaContext context)
    {
        Data.CitizenId = message.CitizenId;
        Data.FromStatus = message.FromStatus;
        Data.ToStatus = message.ToStatus;
        
        // Trigger status progression application
        // This will be handled by the CitizenshipApplications module
        return Task.CompletedTask;
    }

    public Task CompensateAsync(StatusProgressionRequested message, ISagaContext context)
    {
        // Compensation: Revert status progression request
        return Task.CompletedTask;
    }

    public async Task HandleAsync(ApplicationApprovedEvent message, ISagaContext context)
    {
        Data.ApplicationApproved = true;
        
        // Status progression approved - reissue documents with new validity periods
        // Passports and Travel IDs will be reissued
    }

    public Task CompensateAsync(ApplicationApprovedEvent message, ISagaContext context)
    {
        return Task.CompletedTask;
    }

    public async Task HandleAsync(PassportIssued message, ISagaContext context)
    {
        // Passport reissued with new validity period
        // Wait for Travel Identity to be reissued
    }

    public Task CompensateAsync(PassportIssued message, ISagaContext context)
    {
        return Task.CompletedTask;
    }

    public Task HandleAsync(TravelIdentityIssued message, ISagaContext context)
    {
        Data.DocumentsReissued = true;
        
        // All documents reissued - status progression complete
        return CompleteAsync();
    }

    public Task CompensateAsync(TravelIdentityIssued message, ISagaContext context)
    {
        return Task.CompletedTask;
    }
}
