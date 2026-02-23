using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class RoleAlreadyExistsException : DomainException
{
    public RoleAlreadyExistsException(Guid roleId) : base($"Role with ID '{roleId}' already exists.")
    {
    }
}
