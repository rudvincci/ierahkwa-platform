namespace Mamey.Auth.Abstractions;

/// <summary>
/// Interface for managing tokens (JWT, Azure, etc.).
/// </summary>
public interface ITokenManager
{
    /// <summary>
    /// Generates a token based on the provided claims.
    /// </summary>
    /// <param name="claims">The claims to include in the token.</param>
    /// <returns>The generated token.</returns>
    Task<string> GenerateTokenAsync(IEnumerable<KeyValuePair<string, string>> claims);

    /// <summary>
    /// Validates the token and returns whether it is valid.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Revokes the token (if supported).
    /// </summary>
    /// <param name="token">The token to revoke.</param>
    /// <returns>True if the revocation was successful; otherwise, false.</returns>
    Task<bool> RevokeTokenAsync(string token);
}