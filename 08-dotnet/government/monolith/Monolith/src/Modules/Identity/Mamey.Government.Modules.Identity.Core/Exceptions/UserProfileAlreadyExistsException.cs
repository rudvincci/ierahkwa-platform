using Mamey.Exceptions;

namespace Mamey.Government.Modules.Identity.Core.Exceptions;

public sealed class UserProfileAlreadyExistsException : MameyException
{
    public string AuthenticatorIssuer { get; }
    public string AuthenticatorSubject { get; }

    public UserProfileAlreadyExistsException(string issuer, string subject)
        : base($"A user profile already exists for issuer '{issuer}' and subject '{subject}'.")
    {
        AuthenticatorIssuer = issuer;
        AuthenticatorSubject = subject;
    }
}
