using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class SetupTwoFactorAuthHandler : ICommandHandler<SetupTwoFactorAuth>
{
    private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;
    private readonly IEventProcessor _eventProcessor;

    public SetupTwoFactorAuthHandler(ITwoFactorAuthRepository twoFactorAuthRepository, IEventProcessor eventProcessor)
    {
        _twoFactorAuthRepository = twoFactorAuthRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(SetupTwoFactorAuth command, CancellationToken cancellationToken = default)
    {
        var twoFactorAuth = await _twoFactorAuthRepository.GetAsync(command.Id);
        
        if (twoFactorAuth is not null)
        {
            throw new TwoFactorAuthAlreadyExistsException(command.Id);
        }

        twoFactorAuth = TwoFactorAuth.Create(command.Id, new UserId(command.UserId), command.SecretKey, command.QrCodeUrl);
        await _twoFactorAuthRepository.AddAsync(twoFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(twoFactorAuth.Events);
    }
}
