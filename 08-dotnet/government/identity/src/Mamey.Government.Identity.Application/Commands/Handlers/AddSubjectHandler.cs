using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class AddSubjectHandler : ICommandHandler<AddSubject>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IEventProcessor _eventProcessor;

    public AddSubjectHandler(ISubjectRepository subjectRepository,
        IEventProcessor eventProcessor)
    {
        _subjectRepository = subjectRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AddSubject command, CancellationToken cancellationToken = default)
    {
        
        var subject = await _subjectRepository.GetAsync(command.Id);
        
        if(subject is not null)
        {
            throw new SubjectAlreadyExistsException(command.Id);
        }

        subject = Subject.Create(command.Id, command.Name, command.Email, tags: command.Tags);
        await _subjectRepository.AddAsync(subject, cancellationToken);
        await _eventProcessor.ProcessAsync(subject.Events);
    }
}

