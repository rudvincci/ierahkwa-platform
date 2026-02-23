using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class CreateEmailConfirmationHandler : ICommandHandler<CreateEmailConfirmation>
{
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;
    private readonly IEventProcessor _eventProcessor;

    public CreateEmailConfirmationHandler(IEmailConfirmationRepository emailConfirmationRepository, IEventProcessor eventProcessor)
    {
        _emailConfirmationRepository = emailConfirmationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(CreateEmailConfirmation command, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetAsync(command.Id);
        
        if (emailConfirmation is not null)
        {
            throw new EmailConfirmationAlreadyExistsException(command.Id);
        }

        emailConfirmation = EmailConfirmation.Create(command.Id, new UserId(command.UserId), command.Email, command.ConfirmationCode, command.ExpiresAt, command.IpAddress, command.UserAgent);
        await _emailConfirmationRepository.AddAsync(emailConfirmation, cancellationToken);
        await _eventProcessor.ProcessAsync(emailConfirmation.Events);
    }
}
