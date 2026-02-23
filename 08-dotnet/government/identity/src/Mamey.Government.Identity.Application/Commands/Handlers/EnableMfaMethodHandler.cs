using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class EnableMfaMethodHandler : ICommandHandler<EnableMfaMethod>
{
    private readonly IMultiFactorAuthRepository _multiFactorAuthRepository;
    private readonly IEventProcessor _eventProcessor;

    public EnableMfaMethodHandler(IMultiFactorAuthRepository multiFactorAuthRepository, IEventProcessor eventProcessor)
    {
        _multiFactorAuthRepository = multiFactorAuthRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(EnableMfaMethod command, CancellationToken cancellationToken = default)
    {
        var multiFactorAuth = await _multiFactorAuthRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        
        if (multiFactorAuth is null)
        {
            throw new MultiFactorAuthNotFoundException(command.UserId);
        }

        multiFactorAuth.EnableMethod((MfaMethod)command.Method);
        await _multiFactorAuthRepository.UpdateAsync(multiFactorAuth, cancellationToken);
        await _eventProcessor.ProcessAsync(multiFactorAuth.Events);
    }
}
