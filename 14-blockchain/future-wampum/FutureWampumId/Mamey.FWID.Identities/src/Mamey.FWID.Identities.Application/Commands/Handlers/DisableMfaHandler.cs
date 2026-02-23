using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Application.Mappers;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class DisableMfaHandler : ICommandHandler<DisableMfa>
{
    private readonly IMultiFactorAuthService _mfaService;
    private readonly IEventProcessor _eventProcessor;

    public DisableMfaHandler(
        IMultiFactorAuthService mfaService,
        IEventProcessor eventProcessor)
    {
        _mfaService = mfaService ?? throw new ArgumentNullException(nameof(mfaService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(DisableMfa command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        await _mfaService.DisableMfaAsync(
            identityId,
            command.Method.ToDomain(),
            cancellationToken);
    }
}

