using Mamey.CQRS.Commands;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Application.Commands;

internal class DeactivatePermission : ICommand
{
    public PermissionId Id { get; }

    public DeactivatePermission(PermissionId id)
    {
        Id = id;
    }
}
