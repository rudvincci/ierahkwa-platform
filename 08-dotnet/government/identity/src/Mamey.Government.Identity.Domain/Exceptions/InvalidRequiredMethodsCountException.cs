using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class InvalidRequiredMethodsCountException : DomainException
{
    public override string Code { get; } = "invalid_required_methods_count";

    public InvalidRequiredMethodsCountException() : base("Required methods count must be between 1 and 5.")
    {
    }
}
