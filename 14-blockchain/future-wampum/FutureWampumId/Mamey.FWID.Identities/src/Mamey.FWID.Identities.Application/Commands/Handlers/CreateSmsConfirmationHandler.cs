using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class CreateSmsConfirmationHandler : ICommandHandler<CreateSmsConfirmation>
{
    private readonly ISmsConfirmationService _smsConfirmationService;
    private readonly IEventProcessor _eventProcessor;

    public CreateSmsConfirmationHandler(
        ISmsConfirmationService smsConfirmationService,
        IEventProcessor eventProcessor)
    {
        _smsConfirmationService = smsConfirmationService ?? throw new ArgumentNullException(nameof(smsConfirmationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(CreateSmsConfirmation command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        var result = await _smsConfirmationService.CreateSmsConfirmationAsync(
            identityId,
            command.PhoneNumber,
            cancellationToken);

        // Store result in command context for response (if needed)
    }
}

