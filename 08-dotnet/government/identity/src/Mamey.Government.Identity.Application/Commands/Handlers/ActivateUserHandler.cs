using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class ActivateUserHandler : ICommandHandler<ActivateUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventProcessor _eventProcessor;

    public ActivateUserHandler(IUserRepository userRepository, IEventProcessor eventProcessor)
    {
        _userRepository = userRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(ActivateUser command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(command.UserId);
        
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        user.Activate();
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _eventProcessor.ProcessAsync(user.Events);
    }
}
