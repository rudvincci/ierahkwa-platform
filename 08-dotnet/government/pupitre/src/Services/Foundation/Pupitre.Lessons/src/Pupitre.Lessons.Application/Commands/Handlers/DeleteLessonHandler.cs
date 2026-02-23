using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Pupitre.Lessons.Application.Exceptions;
using Pupitre.Lessons.Contracts.Commands;
using Pupitre.Lessons.Domain.Repositories;

namespace Pupitre.Lessons.Application.Commands.Handlers;

internal sealed class DeleteLessonHandler : ICommandHandler<DeleteLesson>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteLessonHandler(ILessonRepository lessonRepository, 
    IEventProcessor eventProcessor)
    {
        _lessonRepository = lessonRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteLesson command, CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetAsync(command.Id, cancellationToken);

        if (lesson is null)
        {
            throw new LessonNotFoundException(command.Id);
        }

        await _lessonRepository.DeleteAsync(lesson.Id);
        await _eventProcessor.ProcessAsync(lesson.Events);
    }
}


