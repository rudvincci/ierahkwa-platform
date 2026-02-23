using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Accessibility.Application.Exceptions;
using Pupitre.Accessibility.Contracts.Commands;
using Pupitre.Accessibility.Domain.Entities;
using Pupitre.Accessibility.Domain.Repositories;

namespace Pupitre.Accessibility.Application.Commands.Handlers;

internal sealed class AddAccessProfileHandler : ICommandHandler<AddAccessProfile>
{
    private readonly IAccessProfileRepository _accessprofileRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddAccessProfileHandler(IAccessProfileRepository accessprofileRepository,
        IEventProcessor eventProcessor)
    {
        _accessprofileRepository = accessprofileRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddAccessProfile command, CancellationToken cancellationToken = default)
    {
        
        var accessprofile = await _accessprofileRepository.GetAsync(command.Id);
        
        if(accessprofile is not null)
        {
            throw new AccessProfileAlreadyExistsException(command.Id);
        }

        accessprofile = AccessProfile.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _accessprofileRepository.AddAsync(accessprofile, cancellationToken);
        await _eventProcessor.ProcessAsync(accessprofile.Events);
    }
}

