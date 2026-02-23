using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class ConfirmEmailHandler : ICommandHandler<ConfirmEmail>
{
    private readonly IEmailConfirmationService _emailConfirmationService;
    private readonly IEventProcessor _eventProcessor;

    public ConfirmEmailHandler(
        IEmailConfirmationService emailConfirmationService,
        IEventProcessor eventProcessor)
    {
        _emailConfirmationService = emailConfirmationService ?? throw new ArgumentNullException(nameof(emailConfirmationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(ConfirmEmail command, CancellationToken cancellationToken = default)
    {
        await _emailConfirmationService.ConfirmEmailAsync(
            command.Token,
            cancellationToken);
    }
}

