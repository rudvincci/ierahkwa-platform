using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class VerifyTwoFactorAuthHandler : ICommandHandler<VerifyTwoFactorAuth>
{
    private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;
    private readonly IEventProcessor _eventProcessor;

    public VerifyTwoFactorAuthHandler(ITwoFactorAuthRepository twoFactorAuthRepository, IEventProcessor eventProcessor)
    {
        _twoFactorAuthRepository = twoFactorAuthRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(VerifyTwoFactorAuth command, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(command.UserId);
        }

        twoFactorAuth.Verify(command.TotpCode);
        await _twoFactorAuthRepository.UpdateAsync(twoFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(twoFactorAuth.Events);
    }
}
