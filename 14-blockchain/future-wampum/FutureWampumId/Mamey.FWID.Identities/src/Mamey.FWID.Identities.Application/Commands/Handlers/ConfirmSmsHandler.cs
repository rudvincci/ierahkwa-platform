using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class ConfirmSmsHandler : ICommandHandler<ConfirmSms>
{
    private readonly ISmsConfirmationService _smsConfirmationService;
    private readonly IEventProcessor _eventProcessor;

    public ConfirmSmsHandler(
        ISmsConfirmationService smsConfirmationService,
        IEventProcessor eventProcessor)
    {
        _smsConfirmationService = smsConfirmationService ?? throw new ArgumentNullException(nameof(smsConfirmationService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(ConfirmSms command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        await _smsConfirmationService.ConfirmSmsAsync(
            identityId,
            command.Code,
            cancellationToken);
    }
}

