using Mamey.Types;

namespace Mamey.Exceptions;

public class InvalidEmailException(string email) : DomainException($"Email: '{email}' is invalid")
{
    public InvalidEmailException(Email email)
        : this(email.Value)
        => Email = email.Value;

    public string Email { get; } = email;
    public override string Code => "invalid_email";
}