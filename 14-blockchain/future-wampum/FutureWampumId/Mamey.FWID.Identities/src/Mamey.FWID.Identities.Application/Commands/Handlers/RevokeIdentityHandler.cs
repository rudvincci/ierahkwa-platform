using Mamey.Contexts;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Microservice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

/// <summary>
/// Handler for revoking an identity (soft delete).
/// Follows the pattern: NO ILogger in handlers - delegate to services.
/// </summary>
internal sealed class RevokeIdentityHandler : ICommandHandler<RevokeIdentity>
{
    private readonly IIdentityService _identityService;
    private readonly IEventProcessor _eventProcessor;
    private readonly IIdentityRepository _repository;
    private readonly IContext _context;

    public RevokeIdentityHandler(
        IIdentityService identityService,
        IEventProcessor eventProcessor,
        IIdentityRepository repository,
        IContext context)
    {
        _identityService = identityService;
        _eventProcessor = eventProcessor;
        _repository = repository;
        _context = context;
    }

    public async Task HandleAsync(RevokeIdentity command, CancellationToken cancellationToken = default)
    {
        var identity = await _repository.GetAsync(command.IdentityId, cancellationToken);
        if (identity == null)
            throw new IdentityNotFoundException(command.IdentityId);

        // Delegate to service for business logic and logging
        await _identityService.RevokeIdentityAsync(
            command.IdentityId,
            command.Reason,
            command.RevokedBy,
            _context.CorrelationId.ToString(),
            cancellationToken);

        // Reload to get updated entity with events
        identity = await _repository.GetAsync(command.IdentityId, cancellationToken);
        if (identity != null)
        {
            // Process domain events
            await _eventProcessor.ProcessAsync(identity.Events);
        }
    }
}

