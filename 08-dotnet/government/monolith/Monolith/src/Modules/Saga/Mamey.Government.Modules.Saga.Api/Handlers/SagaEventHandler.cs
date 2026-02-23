using System.Threading;
using System.Threading.Tasks;
using Chronicle;
using Mamey.Government.Modules.Saga.Api.Messages.Citizenship;
using Mamey.Government.Modules.Saga.Api.Messages.Passports;
using Mamey.CQRS.Events;
using Mamey.Government.Modules.Saga.Api.Messages.Citizens;

namespace Mamey.Government.Modules.Saga.Api.Handlers;

internal sealed class SagaEventHandler :
    // Citizenship application workflow events
    IEventHandler<ApplicationSubmittedEvent>,
    IEventHandler<ApplicationValidated>,
    IEventHandler<KycCompleted>,
    IEventHandler<ApplicationApprovedEvent>,
    IEventHandler<ApplicationRejectedEvent>,
    IEventHandler<CitizenCreatedEvent>,
    IEventHandler<PassportIssued>,
    IEventHandler<TravelIdentityIssued>,
    IEventHandler<PaymentPlanCreated>,
    IEventHandler<StatusProgressionRequested>,
    // Passport workflow events
    IEventHandler<PassportRenewalRequested>
{
    private readonly ISagaCoordinator _sagaCoordinator;

    public SagaEventHandler(ISagaCoordinator sagaCoordinator)
    {
        _sagaCoordinator = sagaCoordinator;
    }

    // Citizenship application workflow handlers
    public Task HandleAsync(ApplicationSubmittedEvent @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(ApplicationValidated @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(KycCompleted @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(ApplicationApprovedEvent @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(ApplicationRejectedEvent @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(CitizenCreatedEvent @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(PassportIssued @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(TravelIdentityIssued @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(PaymentPlanCreated @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    public Task HandleAsync(StatusProgressionRequested @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    // Passport workflow handlers
    public Task HandleAsync(PassportRenewalRequested @event, CancellationToken cancellationToken = default)
        => HandleAsync(@event);

    private Task HandleAsync<T>(T message) where T : class
        => _sagaCoordinator.ProcessAsync(message, SagaContext.Empty);
}
