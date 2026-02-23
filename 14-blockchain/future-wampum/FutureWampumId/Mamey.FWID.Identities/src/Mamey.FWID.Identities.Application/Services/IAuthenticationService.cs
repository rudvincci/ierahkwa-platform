using Mamey.Auth.Jwt;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling authentication operations.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Signs in an identity using username and password (JWT primary authentication).
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <param name="userAgent">The user agent of the client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication result with access token and refresh token.</returns>
    Task<AuthenticationResult> SignInAsync(
        string username,
        string password,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in an identity using biometric data (secondary authentication).
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="biometricData">The biometric data to verify.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <param name="userAgent">The user agent of the client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication result with access token and refresh token.</returns>
    Task<AuthenticationResult> SignInWithBiometricAsync(
        IdentityId identityId,
        Domain.ValueObjects.BiometricData biometricData,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in an identity using DID (secondary authentication).
    /// </summary>
    /// <param name="did">The decentralized identifier.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <param name="userAgent">The user agent of the client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication result with access token and refresh token.</returns>
    Task<AuthenticationResult> SignInWithDidAsync(
        string did,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in an identity using Azure authentication (external authentication).
    /// </summary>
    /// <param name="azureToken">The Azure authentication token.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <param name="userAgent">The user agent of the client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication result with access token and refresh token.</returns>
    Task<AuthenticationResult> SignInWithAzureAsync(
        string azureToken,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in an identity using Identity authentication (external authentication).
    /// </summary>
    /// <param name="identityToken">The Identity authentication token.</param>
    /// <param name="ipAddress">The IP address of the client.</param>
    /// <param name="userAgent">The user agent of the client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication result with access token and refresh token.</returns>
    Task<AuthenticationResult> SignInWithIdentityAsync(
        string identityToken,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs out an identity by revoking the session.
    /// </summary>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SignOutAsync(SessionId sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The new authentication result with access token and refresh token.</returns>
    Task<AuthenticationResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the result of an authentication operation.
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    /// The access token.
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// The refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = null!;

    /// <summary>
    /// The session identifier.
    /// </summary>
    public SessionId SessionId { get; set; } = null!;

    /// <summary>
    /// The identity identifier.
    /// </summary>
    public IdentityId IdentityId { get; set; } = null!;

    /// <summary>
    /// The expiration date and time of the access token.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

