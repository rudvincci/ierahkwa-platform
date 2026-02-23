using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

/// <summary>
/// Handler for updating an identity's zone.
/// </summary>
internal sealed class UpdateZoneHandler : ICommandHandler<UpdateZone>
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateZoneHandler(
        IIdentityRepository repository,
        IEventProcessor eventProcessor)
    {
        _repository = repository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateZone command, CancellationToken cancellationToken = default)
    {
        var identity = await _repository.GetAsync(command.IdentityId, cancellationToken);
        if (identity == null)
            throw new IdentityNotFoundException(command.IdentityId);

        identity.UpdateZone(command.Zone);

        // Handle optimistic locking with retry logic
        try
        {
            await _repository.UpdateAsync(identity, cancellationToken);
            await _eventProcessor.ProcessAsync(identity.Events);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Reload entity with fresh version and retry once
            var freshIdentity = await _repository.GetAsync(command.IdentityId, cancellationToken);
            if (freshIdentity == null)
                throw new IdentityNotFoundException(command.IdentityId);

            // Re-apply the domain operation
            freshIdentity.UpdateZone(command.Zone);

            await _repository.UpdateAsync(freshIdentity, cancellationToken);
            await _eventProcessor.ProcessAsync(freshIdentity.Events);
        }
    }
}

