using Mamey.Exceptions;

namespace Mamey.ServiceName.Domain.Exceptions;

internal class MissingEntityNameTagsException : DomainException
{
    public MissingEntityNameTagsException()
        : base("EntityName tags are missing.")
    {
    }

    public override string Code => "missing_entityname_tags";
}