using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class RemoveRoleFromSubjectHandler : ICommandHandler<RemoveRoleFromSubject>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IEventProcessor _eventProcessor;

    public RemoveRoleFromSubjectHandler(ISubjectRepository subjectRepository, IEventProcessor eventProcessor)
    {
        _subjectRepository = subjectRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(RemoveRoleFromSubject command, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(command.SubjectId);
        if (subject is null)
        {
            throw new SubjectNotFoundException(command.SubjectId);
        }

        subject.RemoveRole(new RoleId(command.RoleId));
        await _subjectRepository.UpdateAsync(subject, cancellationToken);
        await _eventProcessor.ProcessAsync(subject.Events);
    }
}
