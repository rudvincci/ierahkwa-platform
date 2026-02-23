using System;
using System.Security.Claims;
using Mamey.Auth;
using Mamey.Auth.Jwt;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Security;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling authentication operations.
/// </summary>
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IJwtHandler _jwtHandler;
    private readonly ISecurityProvider _securityProvider;
    private readonly IHasher _hasher; // Direct IHasher for password hashing (PasswordHash is not marked with [Hashed])
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IIdentityRepository identityRepository,
        ISessionRepository sessionRepository,
        IJwtHandler jwtHandler,
        ISecurityProvider securityProvider,
        IHasher hasher,
        ILogger<AuthenticationService> logger)
    {
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _jwtHandler = jwtHandler ?? throw new ArgumentNullException(nameof(jwtHandler));
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AuthenticationResult> SignInAsync(
        string username,
        string password,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));
        
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Find identity by username
        var identity = await _identityRepository.GetSingleOrDefaultAsync(
            i => i.Username == username,
            cancellationToken);

        if (identity == null)
        {
            _logger.LogWarning("Sign-in failed: Identity not found for username {Username}", username);
            throw new InvalidOperationException("Invalid username or password");
        }

        // Verify password using IHasher directly (PasswordHash is not marked with [Hashed])
        var passwordHash = _hasher.Hash(password);
        
        // Use case-insensitive comparison for hash strings (SHA-512 hex strings should be lowercase)
        if (string.IsNullOrEmpty(identity.PasswordHash) || 
            !string.Equals(identity.PasswordHash, passwordHash, StringComparison.OrdinalIgnoreCase))
        {
            identity.IncrementFailedLoginAttempts();
            await _identityRepository.UpdateAsync(identity, cancellationToken);
            
            _logger.LogWarning(
                "Sign-in failed: Invalid password for identity {IdentityId}. Stored hash length: {StoredLength}, Computed hash length: {ComputedLength}",
                identity.Id,
                identity.PasswordHash?.Length ?? 0,
                passwordHash?.Length ?? 0);
            throw new InvalidOperationException("Invalid username or password");
        }

        // Check if account is locked
        if (identity.LockedUntil.HasValue && identity.LockedUntil.Value > DateTime.UtcNow)
        {
            _logger.LogWarning("Sign-in failed: Account locked for identity {IdentityId}", identity.Id);
            throw new InvalidOperationException("Account is locked");
        }

        // Sign in the identity
        identity.SignIn();
        await _identityRepository.UpdateAsync(identity, cancellationToken);

        // Generate tokens first
        var accessToken = GenerateAccessToken(identity);
        var refreshToken = _securityProvider.GenerateRandomString(64);
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(accessToken.Expires).DateTime;

        // Create session with the actual tokens that will be returned
        var sessionId = new SessionId();
        var session = new Session(
            sessionId,
            identity.Id,
            accessToken.AccessToken,
            refreshToken,
            expiresAt,
            ipAddress,
            userAgent);

        await _sessionRepository.AddAsync(session, cancellationToken);

        _logger.LogInformation("Sign-in successful for identity {IdentityId}", identity.Id);

        return new AuthenticationResult
        {
            AccessToken = accessToken.AccessToken,
            RefreshToken = refreshToken,
            SessionId = session.Id,
            IdentityId = identity.Id,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthenticationResult> SignInWithBiometricAsync(
        IdentityId identityId,
        BiometricData biometricData,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
            throw new InvalidOperationException("Identity not found");

        // Verify biometric
        identity.VerifyBiometric(biometricData);
        await _identityRepository.UpdateAsync(identity, cancellationToken);

        // Sign in the identity
        identity.SignIn();
        await _identityRepository.UpdateAsync(identity, cancellationToken);

        // Generate tokens first
        var accessToken = GenerateAccessToken(identity);
        var refreshToken = _securityProvider.GenerateRandomString(64);
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(accessToken.Expires).DateTime;

        // Create session with the actual tokens that will be returned
        var sessionId = new SessionId();
        var session = new Session(
            sessionId,
            identity.Id,
            accessToken.AccessToken,
            refreshToken,
            expiresAt,
            ipAddress,
            userAgent);

        await _sessionRepository.AddAsync(session, cancellationToken);

        _logger.LogInformation("Biometric sign-in successful for identity {IdentityId}", identity.Id);

        return new AuthenticationResult
        {
            AccessToken = accessToken.AccessToken,
            RefreshToken = refreshToken,
            SessionId = session.Id,
            IdentityId = identity.Id,
            ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(accessToken.Expires).DateTime
        };
    }

    public async Task<AuthenticationResult> SignInWithDidAsync(
        string did,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        // Validate DID format
        if (string.IsNullOrWhiteSpace(did))
            throw new ArgumentException("DID cannot be null or empty", nameof(did));

        // Find identity by DID
        var identity = await _identityRepository.GetByDidAsync(did, cancellationToken);
        if (identity == null)
            throw new InvalidCredentialsException("DID not found or not associated with any identity");

        // Check if identity is active
        if (identity.Status != IdentityStatus.Verified && identity.Status != IdentityStatus.Verified)
            throw new IdentityNotActiveException(identity.Id);

        // Generate tokens first
        var expiresAt = DateTime.UtcNow.AddHours(24); // DID sessions last 24 hours
        var accessToken = Guid.NewGuid().ToString("N"); // Placeholder until JWT is generated
        var refreshToken = Guid.NewGuid().ToString("N");
        
        // Create session for DID authentication
        var sessionId = new SessionId();
        var session = new Session(
            sessionId,
            identity.Id,
            accessToken,
            refreshToken,
            expiresAt,
            ipAddress,
            userAgent
        );

        await _sessionRepository.AddAsync(session, cancellationToken);

        // Update identity last login
        identity.SignIn();
        await _identityRepository.UpdateAsync(identity, cancellationToken);

        // Generate JWT token
        var token = GenerateJwtToken(identity, session);

        _logger.LogInformation("DID authentication successful for IdentityId: {IdentityId}, DID: {Did}",
            identity.Id.Value, did);

        return new AuthenticationResult
        {
            IdentityId = identity.Id,
            SessionId = sessionId,
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task<AuthenticationResult> SignInWithAzureAsync(
        string azureToken,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        // Validate Azure token
        if (string.IsNullOrWhiteSpace(azureToken))
            throw new ArgumentException("Azure token cannot be null or empty", nameof(azureToken));

        // TODO: Validate Azure token with Azure AD (requires Microsoft.Identity.Web or similar)
        // For now, extract claims from JWT token structure
        var claims = ValidateAzureToken(azureToken);

        // Extract Azure user identifier (sub claim or oid)
        var azureUserId = claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "oid")?.Value;
        if (string.IsNullOrEmpty(azureUserId))
            throw new InvalidCredentialsException("Invalid Azure token: missing user identifier");

        // Find identity by Azure user ID
        var identity = await _identityRepository.GetByAzureUserIdAsync(azureUserId, cancellationToken);
        if (identity == null)
        {
            // Auto-provision identity for Azure users if configured
            identity = await ProvisionAzureIdentityAsync(claims, cancellationToken);
        }

        // Check if identity is active
        if (identity.Status != IdentityStatus.Verified && identity.Status != IdentityStatus.Verified)
            throw new IdentityNotActiveException(identity.Id);

        // Generate tokens first
        var azureExpiresAt = DateTime.UtcNow.AddHours(8); // Azure sessions last 8 hours
        var azureAccessToken = Guid.NewGuid().ToString("N");
        var azureRefreshToken = Guid.NewGuid().ToString("N");

        // Create session for Azure authentication
        var sessionId = new SessionId();
        var session = new Session(
            sessionId,
            identity.Id,
            azureAccessToken,
            azureRefreshToken,
            azureExpiresAt,
            ipAddress,
            userAgent
        );

        await _sessionRepository.AddAsync(session, cancellationToken);

        // Update identity last login
        identity.SignIn();
        await _identityRepository.UpdateAsync(identity, cancellationToken);

        // Generate JWT token
        var token = GenerateJwtToken(identity, session);

        _logger.LogInformation("Azure authentication successful for IdentityId: {IdentityId}, AzureUserId: {AzureUserId}",
            identity.Id.Value, azureUserId);

        return new AuthenticationResult
        {
            IdentityId = identity.Id,
            SessionId = sessionId,
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task<AuthenticationResult> SignInWithIdentityAsync(
        string identityToken,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        // Validate Identity token (service-to-service token)
        if (string.IsNullOrWhiteSpace(identityToken))
            throw new ArgumentException("Identity token cannot be null or empty", nameof(identityToken));

        // Validate service token (this would be a JWT signed by the identity service itself)
        var claims = ValidateServiceToken(identityToken);

        // Extract service identifier
        var serviceId = claims.FirstOrDefault(c => c.Type == "service_id")?.Value;
        if (string.IsNullOrEmpty(serviceId))
            throw new InvalidCredentialsException("Invalid service token: missing service identifier");

        // For service-to-service authentication, we create a system identity or use a service account
        var identity = await _identityRepository.GetByServiceIdAsync(serviceId, cancellationToken);
        if (identity == null)
            throw new InvalidCredentialsException("Service not authorized");

        // Check if identity is active
        if (identity.Status != IdentityStatus.Verified && identity.Status != IdentityStatus.Verified)
            throw new IdentityNotActiveException(identity.Id);

        // Generate tokens first
        var serviceExpiresAt = DateTime.UtcNow.AddHours(1); // Service sessions last 1 hour
        var serviceAccessToken = Guid.NewGuid().ToString("N");
        var serviceRefreshToken = Guid.NewGuid().ToString("N");

        // Create session for service authentication
        var sessionId = new SessionId();
        var session = new Session(
            sessionId,
            identity.Id,
            serviceAccessToken,
            serviceRefreshToken,
            serviceExpiresAt,
            ipAddress,
            userAgent
        );

        await _sessionRepository.AddAsync(session, cancellationToken);

        // Generate JWT token
        var token = GenerateJwtToken(identity, session);

        _logger.LogInformation("Service authentication successful for IdentityId: {IdentityId}, ServiceId: {ServiceId}",
            identity.Id.Value, serviceId);

        return new AuthenticationResult
        {
            IdentityId = identity.Id,
            SessionId = sessionId,
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task SignOutAsync(SessionId sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetAsync(sessionId, cancellationToken);
        if (session == null)
            return;

        session.Revoke();
        await _sessionRepository.UpdateAsync(session, cancellationToken);

        var identity = await _identityRepository.GetAsync(session.IdentityId, cancellationToken);
        if (identity != null)
        {
            identity.SignOut();
            await _identityRepository.UpdateAsync(identity, cancellationToken);
        }

        _logger.LogInformation("Sign-out successful for session {SessionId}", sessionId);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // Find session by refresh token
        var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        if (session == null || !session.IsValid())
        {
            throw new InvalidOperationException("Invalid or expired refresh token");
        }

        var identity = await _identityRepository.GetAsync(session.IdentityId, cancellationToken);
        if (identity == null)
            throw new InvalidOperationException("Identity not found");

        // Generate new tokens
        var accessToken = GenerateAccessToken(identity);
        var newRefreshToken = _securityProvider.GenerateRandomString(64);

        // Update session with new tokens
        session.UpdateTokens(accessToken.AccessToken, newRefreshToken);
        await _sessionRepository.UpdateAsync(session, cancellationToken);

        _logger.LogInformation("Token refreshed for identity {IdentityId}", identity.Id);

        return new AuthenticationResult
        {
            AccessToken = accessToken.AccessToken,
            RefreshToken = newRefreshToken,
            SessionId = session.Id,
            IdentityId = identity.Id,
            ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(accessToken.Expires).DateTime
        };
    }

    private async Task<Session> CreateSessionAsync(
        IdentityId identityId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        var sessionId = new SessionId();
        var accessToken = _securityProvider.GenerateRandomString(64);
        var refreshToken = _securityProvider.GenerateRandomString(64);
        var expiresAt = DateTime.UtcNow.AddHours(24); // 24 hour session

        var session = new Session(
            sessionId,
            identityId,
            accessToken,
            refreshToken,
            expiresAt,
            ipAddress,
            userAgent);

        await _sessionRepository.AddAsync(session, cancellationToken);
        return session;
    }

    private JsonWebToken GenerateAccessToken(Identity identity)
    {
        var claims = new Dictionary<string, string>
        {
            { "sub", identity.Id.Value.ToString() },
            { "name", $"{identity.Name.FirstName} {identity.Name.LastName}" },
            { "email", identity.ContactInformation.Email?.Value ?? string.Empty }
        };

        return _jwtHandler.CreateToken(
            userId: identity.Id.Value.ToString(),
            role: null, // TODO: Get roles from identity
            audience: null,
            claims: claims);
    }

    private IEnumerable<Claim> ValidateAzureToken(string azureToken)
    {
        // TODO: Implement proper Azure token validation using Microsoft.Identity.Web
        // For now, decode JWT and extract claims
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(azureToken);

        return token.Claims;
    }

    private IEnumerable<Claim> ValidateServiceToken(string identityToken)
    {
        // TODO: Implement proper service token validation
        // For now, decode JWT and extract claims
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(identityToken);

        return token.Claims;
    }

    private async Task<Identity> ProvisionAzureIdentityAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        // Extract user information from Azure claims
        var azureUserId = claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "oid")?.Value;
        var email = claims.FirstOrDefault(c => c.Type == "email" || c.Type == "upn")?.Value;
        var givenName = claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
        var familyName = claims.FirstOrDefault(c => c.Type == "family_name")?.Value;

        if (string.IsNullOrEmpty(azureUserId))
            throw new InvalidOperationException("Cannot provision identity: missing Azure user ID");

        // Create new identity
        var identityId = new IdentityId(Guid.NewGuid());
        var name = new Mamey.Types.Name(
            givenName ?? "Unknown",
            familyName ?? "User"
        );

        var contactInfo = new ContactInformation(email);

        var identity = new Identity(
            identityId,
            name,
            new PersonalDetails(DateTime.UtcNow.AddYears(-25), null, null, null), // Default age, no place/gender/clan
            contactInfo,
            new BiometricData(BiometricType.Facial, Array.Empty<byte>(), "pending"),
            "Global" // Default zone
        );

        // Set Azure user ID
        identity.SetAzureUserId(azureUserId);

        await _identityRepository.AddAsync(identity, cancellationToken);

        _logger.LogInformation("Provisioned new identity for Azure user: {AzureUserId}, IdentityId: {IdentityId}",
            azureUserId, identityId.Value);

        return identity;
    }

    private (string AccessToken, string RefreshToken, DateTime ExpiresAt) GenerateJwtToken(Identity identity, Session session)
    {
        // Generate access token
        var accessToken = GenerateAccessToken(identity);

        // Generate refresh token (simple random string for now)
        var refreshToken = _securityProvider.GenerateRandomString(64);

        return (accessToken.AccessToken, refreshToken, DateTimeOffset.FromUnixTimeSeconds(accessToken.Expires).DateTime);
    }
}

