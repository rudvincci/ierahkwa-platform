using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AITutors.Application.Exceptions;
using Pupitre.AITutors.Contracts.Commands;
using Pupitre.AITutors.Domain.Entities;
using Pupitre.AITutors.Domain.Repositories;

namespace Pupitre.AITutors.Application.Commands.Handlers;

internal sealed class AddTutorHandler : ICommandHandler<AddTutor>
{
    private readonly ITutorRepository _tutorRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddTutorHandler(ITutorRepository tutorRepository,
        IEventProcessor eventProcessor)
    {
        _tutorRepository = tutorRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddTutor command, CancellationToken cancellationToken = default)
    {
        
        var tutor = await _tutorRepository.GetAsync(command.Id);
        
        if(tutor is not null)
        {
            throw new TutorAlreadyExistsException(command.Id);
        }

        tutor = Tutor.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags);
        await _tutorRepository.AddAsync(tutor, cancellationToken);
        await _eventProcessor.ProcessAsync(tutor.Events);
    }
}

