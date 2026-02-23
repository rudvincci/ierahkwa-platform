using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class SignInHandler : ICommandHandler<SignIn>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IEventProcessor _eventProcessor;

    public SignInHandler(
        IAuthenticationService authenticationService,
        IEventProcessor eventProcessor)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(SignIn command, CancellationToken cancellationToken = default)
    {
        var result = await _authenticationService.SignInAsync(
            command.Username,
            command.Password,
            command.IpAddress,
            command.UserAgent,
            cancellationToken);

        // Store result in command context for response (if needed)
        // The result will be returned via the API endpoint
    }
}

