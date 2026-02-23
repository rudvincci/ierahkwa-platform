using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidEmailException : DomainException
{
    public override string Code { get; } = "invalid_email";

    public InvalidEmailException() : base("Email is invalid.")
    {
    }
}
