using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.AITutors.Application.Exceptions;
using Pupitre.AITutors.Contracts.Commands;
using Pupitre.AITutors.Domain.Repositories;

namespace Pupitre.AITutors.Application.Commands.Handlers;

internal sealed class DeleteTutorHandler : ICommandHandler<DeleteTutor>
{
    private readonly ITutorRepository _tutorRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteTutorHandler(ITutorRepository tutorRepository, 
    IEventProcessor eventProcessor)
    {
        _tutorRepository = tutorRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteTutor command, CancellationToken cancellationToken = default)
    {
        var tutor = await _tutorRepository.GetAsync(command.Id, cancellationToken);

        if (tutor is null)
        {
            throw new TutorNotFoundException(command.Id);
        }

        await _tutorRepository.DeleteAsync(tutor.Id);
        await _eventProcessor.ProcessAsync(tutor.Events);
    }
}


