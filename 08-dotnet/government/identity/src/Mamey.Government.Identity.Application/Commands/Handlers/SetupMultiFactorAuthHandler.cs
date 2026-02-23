using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class SetupMultiFactorAuthHandler : ICommandHandler<SetupMultiFactorAuth>
{
    private readonly IMultiFactorAuthRepository _multiFactorAuthRepository;
    private readonly IEventProcessor _eventProcessor;

    public SetupMultiFactorAuthHandler(IMultiFactorAuthRepository multiFactorAuthRepository, IEventProcessor eventProcessor)
    {
        _multiFactorAuthRepository = multiFactorAuthRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(SetupMultiFactorAuth command, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetAsync(command.Id);
        
        if (multiFactorAuth is not null)
        {
            throw new MultiFactorAuthAlreadyExistsException(command.Id);
        }

        multiFactorAuth = MultiFactorAuth.Create(command.Id, command.UserId, command.EnabledMethods?.Cast<MfaMethod>());
        await _multiFactorAuthRepository.AddAsync(multiFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(multiFactorAuth.Events);
    }
}
