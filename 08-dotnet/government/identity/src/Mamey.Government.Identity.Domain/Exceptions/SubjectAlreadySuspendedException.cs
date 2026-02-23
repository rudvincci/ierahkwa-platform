using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class SubjectAlreadySuspendedException : DomainException
{
    public override string Code { get; } = "subject_already_suspended";

    public SubjectAlreadySuspendedException() : base("Subject is already suspended.")
    {
    }
}
