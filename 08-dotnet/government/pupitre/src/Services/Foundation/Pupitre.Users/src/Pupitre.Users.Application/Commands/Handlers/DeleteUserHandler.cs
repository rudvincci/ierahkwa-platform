using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Users.Application.Exceptions;
using Pupitre.Users.Contracts.Commands;
using Pupitre.Users.Domain.Repositories;

namespace Pupitre.Users.Application.Commands.Handlers;

internal sealed class DeleteUserHandler : ICommandHandler<DeleteUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteUserHandler(IUserRepository userRepository, 
    IEventProcessor eventProcessor)
    {
        _userRepository = userRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteUser command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(command.Id, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        await _userRepository.DeleteAsync(user.Id);
        await _eventProcessor.ProcessAsync(user.Events);
    }
}


