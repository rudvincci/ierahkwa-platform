using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Curricula.Application.Exceptions;
using Pupitre.Curricula.Contracts.Commands;
using Pupitre.Curricula.Domain.Repositories;

namespace Pupitre.Curricula.Application.Commands.Handlers;

internal sealed class DeleteCurriculumHandler : ICommandHandler<DeleteCurriculum>
{
    private readonly ICurriculumRepository _curriculumRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteCurriculumHandler(ICurriculumRepository curriculumRepository, 
    IEventProcessor eventProcessor)
    {
        _curriculumRepository = curriculumRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteCurriculum command, CancellationToken cancellationToken = default)
    {
        var curriculum = await _curriculumRepository.GetAsync(command.Id, cancellationToken);

        if (curriculum is null)
        {
            throw new CurriculumNotFoundException(command.Id);
        }

        await _curriculumRepository.DeleteAsync(curriculum.Id);
        await _eventProcessor.ProcessAsync(curriculum.Events);
    }
}


