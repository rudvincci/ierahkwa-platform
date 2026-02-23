using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

[Contract]
internal sealed class UpdateSubjectHandler : ICommandHandler<UpdateSubject>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IEventProcessor _eventProcessor;

    public UpdateSubjectHandler(
        ISubjectRepository subjectRepository,
        IEventProcessor eventProcessor)
    {
        _subjectRepository = subjectRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(UpdateSubject command, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(command.Id);

        if(subject is null)
        {
            throw new SubjectNotFoundException(command.Id);
        }

        subject.Update(command.Name, command.Email, tags: command.Tags);
        await _subjectRepository.UpdateAsync(subject);
        await _eventProcessor.ProcessAsync(subject.Events);
    }
}


