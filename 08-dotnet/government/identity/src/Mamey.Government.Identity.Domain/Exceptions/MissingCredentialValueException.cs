using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingCredentialValueException : DomainException
{
    public override string Code { get; } = "missing_credential_value";

    public MissingCredentialValueException() : base("Credential value is missing.")
    {
    }
}
