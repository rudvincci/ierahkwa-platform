using Mamey.Exceptions;

namespace Pupitre.GLEs.Domain.Exceptions;

internal class MissingGLETagsException : DomainException
{
    public MissingGLETagsException()
        : base("GLE tags are missing.")
    {
    }

    public override string Code => "missing_gle_tags";
}