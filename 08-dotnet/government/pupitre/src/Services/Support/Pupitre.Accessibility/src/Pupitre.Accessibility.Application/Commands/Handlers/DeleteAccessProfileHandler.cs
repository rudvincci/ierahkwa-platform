using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Accessibility.Application.Exceptions;
using Pupitre.Accessibility.Contracts.Commands;
using Pupitre.Accessibility.Domain.Repositories;

namespace Pupitre.Accessibility.Application.Commands.Handlers;

internal sealed class DeleteAccessProfileHandler : ICommandHandler<DeleteAccessProfile>
{
    private readonly IAccessProfileRepository _accessprofileRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteAccessProfileHandler(IAccessProfileRepository accessprofileRepository, 
    IEventProcessor eventProcessor)
    {
        _accessprofileRepository = accessprofileRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteAccessProfile command, CancellationToken cancellationToken = default)
    {
        var accessprofile = await _accessprofileRepository.GetAsync(command.Id, cancellationToken);

        if (accessprofile is null)
        {
            throw new AccessProfileNotFoundException(command.Id);
        }

        await _accessprofileRepository.DeleteAsync(accessprofile.Id);
        await _eventProcessor.ProcessAsync(accessprofile.Events);
    }
}


