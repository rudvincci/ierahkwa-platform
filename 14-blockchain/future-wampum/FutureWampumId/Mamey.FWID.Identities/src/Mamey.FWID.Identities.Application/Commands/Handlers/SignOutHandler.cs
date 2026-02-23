using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class SignOutHandler : ICommandHandler<SignOut>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IEventProcessor _eventProcessor;

    public SignOutHandler(
        IAuthenticationService authenticationService,
        IEventProcessor eventProcessor)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(SignOut command, CancellationToken cancellationToken = default)
    {
        var sessionId = new SessionId(command.SessionId);
        await _authenticationService.SignOutAsync(sessionId, cancellationToken);
    }
}

