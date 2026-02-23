using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class ResendEmailConfirmationHandler : ICommandHandler<ResendEmailConfirmation>
{
    private readonly IEmailConfirmationService _emailConfirmationService;
    private readonly IEventProcessor _eventProcessor;

    public ResendEmailConfirmationHandler(
        IEmailConfirmationService emailConfirmationService,
        IEventProcessor eventProcessor)
    {
        _emailConfirmationService = emailConfirmationService ?? throw new ArgumentNullException(nameof(emailConfirmationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(ResendEmailConfirmation command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        await _emailConfirmationService.ResendEmailConfirmationAsync(
            identityId,
            command.Email,
            cancellationToken);
    }
}

