using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Commands.Handlers;

internal sealed class AssignRoleToSubjectHandler : ICommandHandler<AssignRoleToSubject>
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IEventProcessor _eventProcessor;

    public AssignRoleToSubjectHandler(ISubjectRepository subjectRepository, IRoleRepository roleRepository, IEventProcessor eventProcessor)
    {
        _subjectRepository = subjectRepository;
        _roleRepository = roleRepository;
        _eventProcessor = eventProcessor;
    }

    public async Task HandleAsync(AssignRoleToSubject command, CancellationToken cancellationToken = default)
    {
        var subject = await _subjectRepository.GetAsync(command.SubjectId);
        if (subject is null)
        {
            throw new SubjectNotFoundException(command.SubjectId);
        }

        var role = await _roleRepository.GetAsync(command.RoleId);
        if (role is null)
        {
            throw new RoleNotFoundException(command.RoleId);
        }

        subject.AddRole(new RoleId(command.RoleId));
        await _subjectRepository.UpdateAsync(subject, cancellationToken);
        await _eventProcessor.ProcessAsync(subject.Events);
    }
}
