using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.Commands.Handlers;

internal sealed class RenewTravelIdentityHandler : ICommandHandler<RenewTravelIdentity>
{
    private readonly ITravelIdentityRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<RenewTravelIdentityHandler> _logger;

    public RenewTravelIdentityHandler(
        ITravelIdentityRepository repository,
        IMessageBroker messageBroker,
        ILogger<RenewTravelIdentityHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(RenewTravelIdentity command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Renewing travel identity {TravelIdentityId}", command.TravelIdentityId);

        var travelIdentity = await _repository.GetAsync(new TravelIdentityId(command.TravelIdentityId), cancellationToken);
        if (travelIdentity is null)
        {
            throw new InvalidOperationException($"Travel identity with ID {command.TravelIdentityId} not found");
        }

        if (!travelIdentity.IsActive)
        {
            throw new InvalidOperationException($"Cannot renew revoked travel identity {command.TravelIdentityId}");
        }

        var newExpiryDate = DateTime.UtcNow.AddYears(command.ValidityYears);
        travelIdentity.Renew(newExpiryDate);

        await _repository.UpdateAsync(travelIdentity, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new TravelIdentityRenewedEvent(travelIdentity.Id.Value, travelIdentity.CitizenId, newExpiryDate),
            cancellationToken);

        _logger.LogInformation("Travel identity {TravelIdentityId} renewed until {ExpiryDate}",
            command.TravelIdentityId, newExpiryDate);
    }
}
