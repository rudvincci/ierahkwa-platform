using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.ServiceName.Domain.Exceptions;

internal class InvalidEntityNameTagsException : DomainException
{
    public override string Code { get; } = "invalid_entityname_tags";

    public InvalidEntityNameTagsException() : base("EntityName tags are invalid.")
    {
    }
}
