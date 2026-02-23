using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class ResendEmailConfirmationHandler : ICommandHandler<ResendEmailConfirmation>
{
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;
    private readonly IEventProcessor _eventProcessor;

    public ResendEmailConfirmationHandler(IEmailConfirmationRepository emailConfirmationRepository, IEventProcessor eventProcessor)
    {
        _emailConfirmationRepository = emailConfirmationRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(ResendEmailConfirmation command, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        
        if (emailConfirmation is null)
        {
            throw new EmailConfirmationNotFoundException(command.UserId);
        }

        emailConfirmation.Resend(command.NewConfirmationCode, command.NewExpiresAt);
        await _emailConfirmationRepository.UpdateAsync(emailConfirmation, cancellationToken);
        await _eventProcessor.ProcessAsync(emailConfirmation.Events);
    }
}
