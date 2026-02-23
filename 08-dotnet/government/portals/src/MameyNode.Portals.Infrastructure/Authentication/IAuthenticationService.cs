namespace MameyNode.Portals.Infrastructure.Authentication;

/// <summary>
/// Authentication service interface supporting multiple authentication methods
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticate using JWT token
    /// </summary>
    Task<AuthenticationResult> AuthenticateWithJwtAsync(string token, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Authenticate using Azure AD
    /// </summary>
    Task<AuthenticationResult> AuthenticateWithAzureAdAsync(string accessToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Authenticate using FutureWampumID
    /// </summary>
    Task<AuthenticationResult> AuthenticateWithFutureWampumIdAsync(string did, string proof, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get current authenticated user
    /// </summary>
    Task<AuthenticatedUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sign out current user
    /// </summary>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Authentication result
/// </summary>
public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public AuthenticatedUser? User { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Authenticated user information
/// </summary>
public class AuthenticatedUser
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string AuthenticationMethod { get; set; } = string.Empty;
    public Dictionary<string, string> Claims { get; set; } = new();
}


