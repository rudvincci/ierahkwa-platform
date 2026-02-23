using Mamey.Exceptions;

namespace Pupitre.AISafety.Domain.Exceptions;

internal class MissingSafetyCheckTagsException : DomainException
{
    public MissingSafetyCheckTagsException()
        : base("SafetyCheck tags are missing.")
    {
    }

    public override string Code => "missing_safetycheck_tags";
}