using Mamey.Exceptions;

namespace Pupitre.Users.Domain.Exceptions;

internal class MissingUserTagsException : DomainException
{
    public MissingUserTagsException()
        : base("User tags are missing.")
    {
    }

    public override string Code => "missing_user_tags";
}