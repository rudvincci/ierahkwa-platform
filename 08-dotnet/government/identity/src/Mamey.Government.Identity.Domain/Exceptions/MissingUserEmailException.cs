using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingUserEmailException : DomainException
{
    public override string Code { get; } = "missing_user_email";

    public MissingUserEmailException() : base("User email is missing.")
    {
    }
}
