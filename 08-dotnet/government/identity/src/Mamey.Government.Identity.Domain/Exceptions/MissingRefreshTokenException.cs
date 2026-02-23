using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingRefreshTokenException : DomainException
{
    public override string Code { get; } = "missing_refresh_token";

    public MissingRefreshTokenException() : base("Refresh token is missing.")
    {
    }
}
