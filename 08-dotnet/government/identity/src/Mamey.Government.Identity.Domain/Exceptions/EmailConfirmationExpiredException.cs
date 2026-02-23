using Mamey.Exceptions;
using Mamey.Types;
using Mamey.Government.Identity.Domain.Entities;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class EmailConfirmationExpiredException : DomainException
{
    public override string Code { get; } = "email_confirmation_expired";

    public EmailConfirmationExpiredException() : base("Email confirmation has expired.")
    {
    }

    public EmailConfirmationExpiredException(EmailConfirmationId id) : base($"Email confirmation with ID '{id}' has expired.")
    {
    }
}
