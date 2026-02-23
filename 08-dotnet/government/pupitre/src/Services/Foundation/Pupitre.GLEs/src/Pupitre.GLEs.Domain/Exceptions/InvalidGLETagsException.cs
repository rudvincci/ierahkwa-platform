using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.GLEs.Domain.Exceptions;

internal class InvalidGLETagsException : DomainException
{
    public override string Code { get; } = "invalid_gle_tags";

    public InvalidGLETagsException() : base("GLE tags are invalid.")
    {
    }
}
