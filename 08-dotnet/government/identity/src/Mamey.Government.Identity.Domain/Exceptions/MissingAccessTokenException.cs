using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingAccessTokenException : DomainException
{
    public override string Code { get; } = "missing_access_token";

    public MissingAccessTokenException() : base("Access token is missing.")
    {
    }
}
