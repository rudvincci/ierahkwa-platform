using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class DeleteSubjectHandler : ICommandHandler<DeleteSubject>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IEventProcessor _eventProcessor;
    public DeleteSubjectHandler(ISubjectRepository subjectRepository, 
    IEventProcessor eventProcessor)
    {
        _subjectRepository = subjectRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(DeleteSubject command, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(command.Id, cancellationToken);

        if (subject is null)
        {
            throw new SubjectNotFoundException(command.Id);
        }

        await _subjectRepository.DeleteAsync(subject.Id);
        await _eventProcessor.ProcessAsync(subject.Events);
    }
}


