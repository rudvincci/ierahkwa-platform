using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingUsernameException : DomainException
{
    public override string Code { get; } = "missing_username";

    public MissingUsernameException() : base("Username is missing.")
    {
    }
}
