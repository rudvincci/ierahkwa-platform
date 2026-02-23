using Mamey.CQRS.Commands;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Commands;

internal class DeactivateRole : ICommand
{
    public RoleId Id { get; }

    public DeactivateRole(RoleId id)
    {
        Id = id;
    }
}
