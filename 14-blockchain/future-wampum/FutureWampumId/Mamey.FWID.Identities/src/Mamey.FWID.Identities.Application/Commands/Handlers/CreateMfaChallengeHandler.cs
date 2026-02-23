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

internal sealed class CreateMfaChallengeHandler : ICommandHandler<CreateMfaChallenge>
{
    private readonly IMultiFactorAuthService _mfaService;
    private readonly IEventProcessor _eventProcessor;

    public CreateMfaChallengeHandler(
        IMultiFactorAuthService mfaService,
        IEventProcessor eventProcessor)
    {
        _mfaService = mfaService ?? throw new ArgumentNullException(nameof(mfaService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(CreateMfaChallenge command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        var result = await _mfaService.CreateMfaChallengeAsync(
            identityId,
            command.Method.ToDomain(),
            cancellationToken);

        // Store result in command context for response (if needed)
    }
}

