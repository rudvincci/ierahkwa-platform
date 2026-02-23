using Mamey.Exceptions;

namespace Mamey.Government.Identity.Domain.Exceptions;

public class EmailConfirmationAlreadyConfirmedException : DomainException
{
    public EmailConfirmationAlreadyConfirmedException() : base("Email confirmation has already been confirmed.")
    {
    }

    public EmailConfirmationAlreadyConfirmedException(string message) : base(message)
    {
    }
}
