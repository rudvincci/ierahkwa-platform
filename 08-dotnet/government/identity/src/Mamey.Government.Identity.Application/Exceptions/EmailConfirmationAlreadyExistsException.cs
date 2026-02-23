using Mamey.Exceptions;
using Mamey.Government.Identity.Domain.Exceptions;

namespace Mamey.Government.Identity.Application.Exceptions;

public class EmailConfirmationAlreadyExistsException : DomainException
{
    public EmailConfirmationAlreadyExistsException(Guid emailConfirmationId) : base($"Email confirmation with ID '{emailConfirmationId}' already exists.")
    {
    }
}
