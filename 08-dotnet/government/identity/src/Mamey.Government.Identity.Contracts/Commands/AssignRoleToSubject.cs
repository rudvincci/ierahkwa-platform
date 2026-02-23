using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record AssignRoleToSubject : ICommand
{
    public AssignRoleToSubject(Guid subjectId, Guid roleId)
    {
        SubjectId = subjectId;
        RoleId = roleId;
    }

    public Guid SubjectId { get; init; }
    public Guid RoleId { get; init; }
}
