using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class CreateUserHandler : ICommandHandler<CreateUser>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventProcessor _eventProcessor;

    public CreateUserHandler(IUserRepository userRepository, IEventProcessor eventProcessor)
    {
        _userRepository = userRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(CreateUser command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email);
        
        if (user is not null)
        {
            throw new UserAlreadyExistsException(user.Email);
        }

        user = User.Create(command.Id, command.SubjectId, command.Username, command.Email, command.PasswordHash);
        await _userRepository.AddAsync(user, cancellationToken);
        await _eventProcessor.ProcessAsync(user.Events);
    }
}
