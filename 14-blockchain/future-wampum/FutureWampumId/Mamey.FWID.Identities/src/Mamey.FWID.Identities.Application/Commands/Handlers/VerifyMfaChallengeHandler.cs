using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class VerifyMfaChallengeHandler : ICommandHandler<VerifyMfaChallenge>
{
    private readonly IMultiFactorAuthService _mfaService;
    private readonly IEventProcessor _eventProcessor;

    public VerifyMfaChallengeHandler(
        IMultiFactorAuthService mfaService,
        IEventProcessor eventProcessor)
    {
        _mfaService = mfaService ?? throw new ArgumentNullException(nameof(mfaService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(VerifyMfaChallenge command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        var isValid = await _mfaService.VerifyMfaChallengeAsync(
            identityId,
            command.Method.ToDomain(),
            command.Code,
            cancellationToken);

        if (!isValid)
        {
            throw new InvalidOperationException("Invalid MFA challenge code");
        }
    }
}

