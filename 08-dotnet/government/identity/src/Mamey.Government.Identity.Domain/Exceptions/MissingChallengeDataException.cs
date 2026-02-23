using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingChallengeDataException : DomainException
{
    public override string Code { get; } = "missing_challenge_data";

    public MissingChallengeDataException() : base("Challenge data is missing.")
    {
    }
}
