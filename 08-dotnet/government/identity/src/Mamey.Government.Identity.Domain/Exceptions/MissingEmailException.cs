using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingEmailException : DomainException
{
    public override string Code { get; } = "missing_email";

    public MissingEmailException() : base("Email is missing.")
    {
    }
}
