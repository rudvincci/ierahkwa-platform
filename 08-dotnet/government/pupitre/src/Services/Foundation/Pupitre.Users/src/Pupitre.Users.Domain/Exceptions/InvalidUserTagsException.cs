using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Users.Domain.Exceptions;

internal class InvalidUserTagsException : DomainException
{
    public override string Code { get; } = "invalid_user_tags";

    public InvalidUserTagsException() : base("User tags are invalid.")
    {
    }
}
