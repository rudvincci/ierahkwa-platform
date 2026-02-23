using Mamey.Portal.Auth.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Portal.Auth.Domain.Entities;

public sealed class AuthUser : AggregateRoot<string>
{
    public string Email { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string[] Roles { get; private set; } = Array.Empty<string>();
    public string Issuer { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private AuthUser() { }

    public AuthUser(
        IssuerSubject issuerSubject,
        string email,
        string? displayName,
        string[] roles,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
        : base(issuerSubject.ToString())
    {
        Issuer = issuerSubject.Issuer;
        Subject = issuerSubject.Subject;
        Email = email;
        DisplayName = displayName;
        Roles = roles;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
