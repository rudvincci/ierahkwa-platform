using Mamey.Exceptions;

namespace Mamey.Identity.AspNetCore.Exceptions;

/// <summary>
/// Base exception for identity errors.
/// </summary>
public class IdentityException : MameyException
{
    public IdentityException() { }
    public IdentityException(string message) : base(message) { }

    public IdentityException(string code, string reason)
        : base(code, reason)
    {
    }
    public IdentityException(string message, string code, string reason)  : base(code, reason){}

}
/// <summary>
/// Thrown when an operation is attempted across tenants.
/// </summary>
public class TenantMismatchException : IdentityException
{
    public TenantMismatchException()
    { }

    public TenantMismatchException(Guid expected, Guid actual)
        : base($"Tenant mismatch: expected {expected}, got {actual}.", "tenant_mismatch", reason: $"Tenant mismatch: expected {expected}, got {actual}.")
    { }
}
/// <summary>
/// Thrown when a user is not authorized to perform an action.
/// </summary>
public class AuthorizationFailedException : IdentityException
{
    public AuthorizationFailedException() { }

    public AuthorizationFailedException(string permission)
        : base($"Authorization failed. Missing permission '{permission}'.", "authorization_failed", $"Authorization failed. Missing permission '{permission}'.")
    { }
}

