using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class ResendSmsConfirmationHandler : ICommandHandler<ResendSmsConfirmation>
{
    private readonly ISmsConfirmationService _smsConfirmationService;
    private readonly IEventProcessor _eventProcessor;

    public ResendSmsConfirmationHandler(
        ISmsConfirmationService smsConfirmationService,
        IEventProcessor eventProcessor)
    {
        _smsConfirmationService = smsConfirmationService ?? throw new ArgumentNullException(nameof(smsConfirmationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(ResendSmsConfirmation command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        await _smsConfirmationService.ResendSmsConfirmationAsync(
            identityId,
            command.PhoneNumber,
            cancellationToken);
    }
}

