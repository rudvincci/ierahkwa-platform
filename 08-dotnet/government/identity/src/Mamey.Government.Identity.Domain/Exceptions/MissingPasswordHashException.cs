using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingPasswordHashException : DomainException
{
    public override string Code { get; } = "missing_password_hash";

    public MissingPasswordHashException() : base("Password hash is missing.")
    {
    }
}
