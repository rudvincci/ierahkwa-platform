using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class ConfirmEmailHandler : ICommandHandler<ConfirmEmail>
{
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEventProcessor _eventProcessor;

    public ConfirmEmailHandler(IEmailConfirmationRepository emailConfirmationRepository, IUserRepository userRepository, IEventProcessor eventProcessor)
    {
        _emailConfirmationRepository = emailConfirmationRepository;
        _userRepository = userRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(ConfirmEmail command, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetByConfirmationCodeAsync(command.ConfirmationCode, cancellationToken);
        
        if (emailConfirmation is null)
        {
            throw new EmailConfirmationNotFoundException(command.ConfirmationCode);
        }

        emailConfirmation.Confirm(command.ConfirmationCode);
        await _emailConfirmationRepository.UpdateAsync(emailConfirmation, cancellationToken);

        // Update user's email confirmation status
        var user = await _userRepository.GetAsync(emailConfirmation.UserId, cancellationToken);
        if (user is not null)
        {
            user.ConfirmEmail();
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _eventProcessor.ProcessAsync(user.Events);
        }

        await _eventProcessor.ProcessAsync(emailConfirmation.Events);
    }
}
