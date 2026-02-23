using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.Commands.Handlers;

internal sealed class RevokeTravelIdentityHandler : ICommandHandler<RevokeTravelIdentity>
{
    private readonly ITravelIdentityRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<RevokeTravelIdentityHandler> _logger;

    public RevokeTravelIdentityHandler(
        ITravelIdentityRepository repository,
        IMessageBroker messageBroker,
        ILogger<RevokeTravelIdentityHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(RevokeTravelIdentity command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking travel identity {TravelIdentityId}", command.TravelIdentityId);

        var travelIdentity = await _repository.GetAsync(new TravelIdentityId(command.TravelIdentityId), cancellationToken);
        if (travelIdentity is null)
        {
            throw new InvalidOperationException($"Travel identity with ID {command.TravelIdentityId} not found");
        }

        if (!travelIdentity.IsActive)
        {
            throw new InvalidOperationException($"Travel identity {command.TravelIdentityId} is already revoked");
        }

        travelIdentity.Revoke(command.Reason);

        await _repository.UpdateAsync(travelIdentity, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new TravelIdentityRevokedEvent(travelIdentity.Id.Value, travelIdentity.CitizenId, command.Reason),
            cancellationToken);

        _logger.LogInformation("Travel identity {TravelIdentityId} revoked. Reason: {Reason}",
            command.TravelIdentityId, command.Reason);
    }
}
