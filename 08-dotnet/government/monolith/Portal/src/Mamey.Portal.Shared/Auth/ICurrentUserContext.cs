namespace Mamey.Portal.Shared.Auth;

public interface ICurrentUserContext
{
    string UserName { get; }
    string Role { get; }

    IReadOnlyCollection<string> Roles
        => string.IsNullOrWhiteSpace(Role) ? Array.Empty<string>() : new[] { Role };

    bool IsInRole(string role)
        => Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));

    bool IsAuthenticated { get; }
}


