using Mamey.FWID.Identities.Application.Mappers;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class SignInWithBiometricHandler : ICommandHandler<SignInWithBiometric>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IEventProcessor _eventProcessor;

    public SignInWithBiometricHandler(
        IAuthenticationService authenticationService,
        IEventProcessor eventProcessor)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(SignInWithBiometric command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        var result = await _authenticationService.SignInWithBiometricAsync(
            identityId,
            command.BiometricData.ToDomain(),
            command.IpAddress,
            command.UserAgent,
            cancellationToken);

        // Store result in command context for response (if needed)
    }
}

