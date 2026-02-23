using Mamey.Contexts;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Microservice.Infrastructure;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

/// <summary>
/// Handler for adding a new identity.
/// Follows the pattern: NO ILogger in handlers - delegate to services.
/// </summary>
internal sealed class AddIdentityHandler : ICommandHandler<AddIdentity>
{
    private readonly IIdentityService _identityService;
    private readonly IEventProcessor _eventProcessor;
    private readonly IIdentityRepository _identityRepository;
    private readonly IContext _context;

    public AddIdentityHandler(
        IIdentityService identityService,
        IEventProcessor eventProcessor,
        IIdentityRepository identityRepository,
        IContext context)
    {
        _identityService = identityService;
        _eventProcessor = eventProcessor;
        _identityRepository = identityRepository;
        _context = context;
    }

    public async Task HandleAsync(AddIdentity command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.Id);
        var existing = await _identityRepository.GetAsync(identityId, cancellationToken);
        
        if(existing is not null)
        {
            throw new IdentityAlreadyExistsException(identityId);
        }

        // Map DTOs to domain value objects
        var personalDetails = command.PersonalDetails.ToDomain();
        var contactInformation = command.ContactInformation.ToDomain();
        var biometricData = command.BiometricData.ToDomain();

        // Delegate to service for business logic and logging
        var identity = await _identityService.CreateIdentityAsync(
            identityId,
            command.Name,
            personalDetails,
            contactInformation,
            biometricData,
            command.Zone,
            command.ClanRegistrarId,
            _context.CorrelationId.ToString(),
            cancellationToken);

        // Process domain events
        await _eventProcessor.ProcessAsync(identity.Events);
    }
}

