using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Commands.Handlers;

internal sealed class GenerateBackupCodesHandler : ICommandHandler<GenerateBackupCodes>
{
    private readonly IMultiFactorAuthService _mfaService;
    private readonly IEventProcessor _eventProcessor;

    public GenerateBackupCodesHandler(
        IMultiFactorAuthService mfaService,
        IEventProcessor eventProcessor)
    {
        _mfaService = mfaService ?? throw new ArgumentNullException(nameof(mfaService));
        _eventProcessor = eventProcessor ?? throw new ArgumentNullException(nameof(eventProcessor));
    }

    public async Task HandleAsync(GenerateBackupCodes command, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(command.IdentityId);
        var codes = await _mfaService.GenerateBackupCodesAsync(
            identityId,
            command.Count,
            cancellationToken);

        // Store codes in command context for response (if needed)
    }
}

