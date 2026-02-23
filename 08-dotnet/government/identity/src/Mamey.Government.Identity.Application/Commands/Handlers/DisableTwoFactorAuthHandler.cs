using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class DisableTwoFactorAuthHandler : ICommandHandler<DisableTwoFactorAuth>
{
    private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEventProcessor _eventProcessor;

    public DisableTwoFactorAuthHandler(ITwoFactorAuthRepository twoFactorAuthRepository, IUserRepository userRepository, IEventProcessor eventProcessor)
    {
        _twoFactorAuthRepository = twoFactorAuthRepository;
        _userRepository = userRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DisableTwoFactorAuth command, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _twoFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        
        if (twoFactorAuth is null)
        {
            throw new TwoFactorAuthNotFoundException(command.UserId);
        }

        twoFactorAuth.Disable();
        await _twoFactorAuthRepository.UpdateAsync(twoFactorAuth, cancellationToken);

        // Update user's 2FA status
        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is not null)
        {
            user.DisableTwoFactor();
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _eventProcessor.ProcessAsync(user.Events);
        }

        await _eventProcessor.ProcessAsync(twoFactorAuth.Events);
    }
}
