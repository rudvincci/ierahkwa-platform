using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Commands.Integration.Identities;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Commands.Handlers.Integration;

/// <summary>
/// Handler for VerifyIdentity integration command from other services.
/// </summary>
internal sealed class VerifyIdentityIntegrationCommandHandler : ICommandHandler<VerifyIdentityIntegrationCommand>
{
    private readonly IIdentityRepository _repository;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<VerifyIdentityIntegrationCommandHandler> _logger;

    public VerifyIdentityIntegrationCommandHandler(
        IIdentityRepository repository,
        IEventProcessor eventProcessor,
        ILogger<VerifyIdentityIntegrationCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(VerifyIdentityIntegrationCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received VerifyIdentity integration command: IdentityId={IdentityId}",
            command.IdentityId);

        var identity = await _repository.GetAsync(new IdentityId(command.IdentityId), cancellationToken);
        if (identity == null)
        {
            throw new IdentityNotFoundException(new IdentityId(command.IdentityId));
        }

        // ProvidedBiometric is already a domain object in the integration command
        var threshold = command.Threshold ?? 0.95;
        identity.VerifyBiometric(command.ProvidedBiometric, threshold);

        await _repository.UpdateAsync(identity, cancellationToken);
        await _eventProcessor.ProcessAsync(identity.Events);

        _logger.LogInformation("Verified identity from integration command: {IdentityId}", command.IdentityId);
    }
}


