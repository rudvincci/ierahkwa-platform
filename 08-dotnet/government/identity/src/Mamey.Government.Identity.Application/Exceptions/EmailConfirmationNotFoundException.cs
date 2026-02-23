using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class EmailConfirmationNotFoundException : DomainException
{
    public EmailConfirmationNotFoundException(string confirmationCode) : base($"Email confirmation with code '{confirmationCode}' was not found.")
    {
    }

    public EmailConfirmationNotFoundException(Guid userId) : base($"Email confirmation for user '{userId}' was not found.")
    {
    }
}
