using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Integration.Identities;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Commands.Handlers.Integration;

/// <summary>
/// Handler for CreateIdentity integration command from other services.
/// </summary>
internal sealed class CreateIdentityIntegrationCommandHandler : ICommandHandler<CreateIdentityIntegrationCommand>
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<CreateIdentityIntegrationCommandHandler> _logger;

    public CreateIdentityIntegrationCommandHandler(
        IIdentityRepository repository,
        IEventProcessor eventProcessor,
        ILogger<CreateIdentityIntegrationCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(CreateIdentityIntegrationCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received CreateIdentity integration command: Id={Id}, Name={Name}",
            command.Id, command.Name);

        // Check if identity already exists
        var existingIdentity = await _repository.GetAsync(new IdentityId(command.Id), cancellationToken);
        if (existingIdentity != null)
        {
            _logger.LogWarning("Identity already exists: {IdentityId}", command.Id);
            throw new IdentityAlreadyExistsException(new IdentityId(command.Id));
        }

        // Use the existing AddIdentityHandler logic
        var identityId = new IdentityId(command.Id);
        var identity = new Identity(
            identityId,
            command.Name,
            command.PersonalDetails,
            command.ContactInformation,
            command.BiometricData,
            command.Zone,
            command.ClanRegistrarId);

        await _repository.AddAsync(identity, cancellationToken);
        await _eventProcessor.ProcessAsync(identity.Events);

        _logger.LogInformation("Created identity from integration command: {IdentityId}", identityId.Value);
    }
}


