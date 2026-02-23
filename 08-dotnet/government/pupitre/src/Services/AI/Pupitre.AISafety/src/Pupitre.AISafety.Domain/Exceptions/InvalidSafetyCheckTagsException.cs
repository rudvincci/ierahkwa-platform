using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AISafety.Domain.Exceptions;

internal class InvalidSafetyCheckTagsException : DomainException
{
    public override string Code { get; } = "invalid_safetycheck_tags";

    public InvalidSafetyCheckTagsException() : base("SafetyCheck tags are invalid.")
    {
    }
}
