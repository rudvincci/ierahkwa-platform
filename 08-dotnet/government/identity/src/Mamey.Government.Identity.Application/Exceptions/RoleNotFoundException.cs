using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class RoleNotFoundException : DomainException
{
    public RoleNotFoundException(Guid roleId) : base($"Role with ID '{roleId}' was not found.")
    {
    }
}
