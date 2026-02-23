using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingSecretKeyException : DomainException
{
    public override string Code { get; } = "missing_secret_key";

    public MissingSecretKeyException() : base("Secret key is missing.")
    {
    }
}
