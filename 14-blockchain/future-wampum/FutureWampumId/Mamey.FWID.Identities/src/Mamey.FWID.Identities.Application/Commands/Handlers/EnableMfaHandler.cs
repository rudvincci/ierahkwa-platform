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

internal sealed class EnableMfaHandler : ICommandHandler<EnableMfa>
{
    private readonly IMultiFactorAuthService _mfaService;
    private readonly IEventProcessor _eventProcessor;

    public EnableMfaHandler(
        IMultiFactorAuthService mfaService,
        IEventProcessor eventProcessor)
    {
        _mfaService = mfaService ?? throw new ArgumentNullException(nameof(mfaService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(EnableMfa command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        await _mfaService.EnableMfaAsync(
            identityId,
            command.Method.ToDomain(),
            command.VerificationCode,
            cancellationToken);
    }
}

