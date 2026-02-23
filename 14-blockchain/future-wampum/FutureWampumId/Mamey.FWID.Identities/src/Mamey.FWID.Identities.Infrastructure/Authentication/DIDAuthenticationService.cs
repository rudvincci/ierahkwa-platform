using System.Security.Cryptography;
using System.Text;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prometheus;
using PrometheusMetrics = Prometheus.Metrics;

namespace Mamey.FWID.Identities.Infrastructure.Authentication;

/// <summary>
/// Service implementation for DID-based authentication.
/// Handles challenge-response flow and identity resolution.
/// 
/// TDD Reference: Lines 1594-1703 (Identity Service)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
internal sealed class DIDAuthenticationService : IDIDAuthenticationService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DIDAuthenticationService> _logger;
    private readonly DIDAuthenticationOptions _options;

    // Prometheus metrics
    private static readonly Counter DIDAuthAttempts = PrometheusMetrics
        .CreateCounter(
            "identity_did_auth_attempts_total",
            "Total DID authentication attempts",
            new CounterConfiguration
            {
                LabelNames = new[] { "result", "method" }
            });

    private static readonly Counter DIDChallengesIssued = PrometheusMetrics
        .CreateCounter(
            "identity_did_challenges_issued_total",
            "Total DID authentication challenges issued");

    private static readonly Histogram DIDAuthDuration = PrometheusMetrics
        .CreateHistogram(
            "identity_did_auth_duration_seconds",
            "DID authentication duration",
            new HistogramConfiguration
            {
                Buckets = new[] { 0.05, 0.1, 0.25, 0.5, 1.0, 2.5 }
            });

    public DIDAuthenticationService(
        IIdentityRepository identityRepository,
        IMemoryCache cache,
        ILogger<DIDAuthenticationService> logger,
        IConfiguration configuration)
    {
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _options = new DIDAuthenticationOptions();
        configuration.GetSection("didAuthentication").Bind(_options);
    }

    /// <inheritdoc />
    public async Task<DIDAuthChallenge> CreateChallengeAsync(
        string did,
        string domain,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating DID authentication challenge: DID={DID}", did);

        var challenge = new DIDAuthChallenge
        {
            ChallengeId = Guid.NewGuid().ToString(),
            DID = did,
            Nonce = GenerateNonce(),
            Domain = domain,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddSeconds(_options.ChallengeExpirationSeconds)
        };

        // Cache challenge
        var cacheKey = $"did_auth_challenge:{challenge.ChallengeId}";
        _cache.Set(cacheKey, challenge, TimeSpan.FromSeconds(_options.ChallengeExpirationSeconds));

        DIDChallengesIssued.Inc();

        _logger.LogDebug(
            "DID challenge created: ChallengeId={ChallengeId}, ExpiresAt={ExpiresAt}",
            challenge.ChallengeId, challenge.ExpiresAt);

        return challenge;
    }

    /// <inheritdoc />
    public async Task<DIDAuthenticationResult> AuthenticateAsync(
        AuthenticateWithDID command,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var didMethod = ExtractDIDMethod(command.DID);

        _logger.LogInformation(
            "Processing DID authentication: DID={DID}, ChallengeId={ChallengeId}",
            command.DID, command.ChallengeId);

        try
        {
            // Validate challenge exists and is not expired
            var cacheKey = $"did_auth_challenge:{command.ChallengeId}";
            if (!_cache.TryGetValue(cacheKey, out DIDAuthChallenge? challenge) || challenge == null)
            {
                DIDAuthAttempts.WithLabels("challenge_not_found", didMethod).Inc();
                return new DIDAuthenticationResult
                {
                    Success = false,
                    DID = command.DID,
                    ErrorMessage = "Challenge not found or expired",
                    ErrorCode = "CHALLENGE_NOT_FOUND"
                };
            }

            if (challenge.ExpiresAt < DateTime.UtcNow)
            {
                DIDAuthAttempts.WithLabels("challenge_expired", didMethod).Inc();
                return new DIDAuthenticationResult
                {
                    Success = false,
                    DID = command.DID,
                    ErrorMessage = "Challenge has expired",
                    ErrorCode = "CHALLENGE_EXPIRED"
                };
            }

            // Verify DID matches challenge
            if (!challenge.DID.Equals(command.DID, StringComparison.OrdinalIgnoreCase))
            {
                DIDAuthAttempts.WithLabels("did_mismatch", didMethod).Inc();
                return new DIDAuthenticationResult
                {
                    Success = false,
                    DID = command.DID,
                    ErrorMessage = "DID does not match challenge",
                    ErrorCode = "DID_MISMATCH"
                };
            }

            // Verify nonce matches
            if (!challenge.Nonce.Equals(command.Nonce, StringComparison.Ordinal))
            {
                DIDAuthAttempts.WithLabels("nonce_mismatch", didMethod).Inc();
                return new DIDAuthenticationResult
                {
                    Success = false,
                    DID = command.DID,
                    ErrorMessage = "Nonce does not match",
                    ErrorCode = "NONCE_MISMATCH"
                };
            }

            // Verify signature
            var signatureValid = await VerifySignatureAsync(
                command.DID,
                challenge.MessageToSign,
                command.SignedChallenge,
                command.VerificationMethodId,
                command.Algorithm,
                cancellationToken);

            if (!signatureValid)
            {
                DIDAuthAttempts.WithLabels("invalid_signature", didMethod).Inc();
                return new DIDAuthenticationResult
                {
                    Success = false,
                    DID = command.DID,
                    ErrorMessage = "Invalid signature",
                    ErrorCode = "INVALID_SIGNATURE"
                };
            }

            // Resolve identity by DID
            var identityId = await ResolveIdentityByDIDAsync(command.DID, cancellationToken);
            if (identityId == null)
            {
                DIDAuthAttempts.WithLabels("identity_not_found", didMethod).Inc();
                return new DIDAuthenticationResult
                {
                    Success = false,
                    DID = command.DID,
                    ErrorMessage = "No identity linked to this DID",
                    ErrorCode = "IDENTITY_NOT_FOUND"
                };
            }

            // Get identity for token generation
            var identity = await _identityRepository.GetAsync(new Domain.Entities.IdentityId(identityId.Value), cancellationToken);
            if (identity == null)
            {
                DIDAuthAttempts.WithLabels("identity_not_found", didMethod).Inc();
                return new DIDAuthenticationResult
                {
                    Success = false,
                    DID = command.DID,
                    ErrorMessage = "Identity not found",
                    ErrorCode = "IDENTITY_NOT_FOUND"
                };
            }

            // Invalidate used challenge
            _cache.Remove(cacheKey);

            // Generate JWT token
            var (accessToken, refreshToken, expiresAt) = await GenerateTokensAsync(identity, command.DID, cancellationToken);

            var durationSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
            DIDAuthAttempts.WithLabels("success", didMethod).Inc();
            DIDAuthDuration.Observe(durationSeconds);

            _logger.LogInformation(
                "DID authentication successful: DID={DID}, IdentityId={IdentityId}",
                command.DID, identityId);

            return new DIDAuthenticationResult
            {
                Success = true,
                IdentityId = identityId,
                DID = command.DID,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                Roles = GetRoles(identity)
            };
        }
        catch (Exception ex)
        {
            var durationSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
            DIDAuthAttempts.WithLabels("error", didMethod).Inc();
            DIDAuthDuration.Observe(durationSeconds);

            _logger.LogError(ex, "Error during DID authentication: DID={DID}", command.DID);

            return new DIDAuthenticationResult
            {
                Success = false,
                DID = command.DID,
                ErrorMessage = $"Authentication error: {ex.Message}",
                ErrorCode = "AUTH_ERROR"
            };
        }
    }

    /// <inheritdoc />
    public async Task<Guid?> ResolveIdentityByDIDAsync(
        string did,
        CancellationToken cancellationToken = default)
    {
        // Check cache first
        var cacheKey = $"did_identity:{did}";
        if (_cache.TryGetValue(cacheKey, out Guid cachedIdentityId))
        {
            return cachedIdentityId;
        }

        // Query repository for identity linked to this DID
        // This would query a DID-to-Identity mapping table
        // For now, this is a placeholder - the actual implementation would
        // query the database for the identity that has this DID linked

        _logger.LogDebug("Resolving identity for DID: {DID}", did);

        // TODO: Implement actual lookup via repository
        // var identityId = await _identityRepository.GetByDIDAsync(did, cancellationToken);

        return null; // Placeholder - implement actual lookup
    }

    /// <inheritdoc />
    public async Task<bool> LinkDIDToIdentityAsync(
        Guid identityId,
        string did,
        string verificationProof,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Linking DID to identity: IdentityId={IdentityId}, DID={DID}",
            identityId, did);

        // Verify DID ownership via proof
        var proofValid = await VerifyDIDOwnershipProofAsync(did, verificationProof, cancellationToken);
        if (!proofValid)
        {
            _logger.LogWarning("DID ownership proof invalid: DID={DID}", did);
            return false;
        }

        // Check if DID is already linked to another identity
        var existingIdentity = await ResolveIdentityByDIDAsync(did, cancellationToken);
        if (existingIdentity != null && existingIdentity != identityId)
        {
            _logger.LogWarning("DID already linked to another identity: DID={DID}", did);
            return false;
        }

        // TODO: Store DID-Identity link in database
        // await _identityRepository.LinkDIDAsync(identityId, did, cancellationToken);

        // Cache the link
        var cacheKey = $"did_identity:{did}";
        _cache.Set(cacheKey, identityId, TimeSpan.FromHours(1));

        _logger.LogInformation("DID linked successfully: IdentityId={IdentityId}, DID={DID}", identityId, did);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> UnlinkDIDFromIdentityAsync(
        Guid identityId,
        string did,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Unlinking DID from identity: IdentityId={IdentityId}, DID={DID}",
            identityId, did);

        // TODO: Remove DID-Identity link from database
        // await _identityRepository.UnlinkDIDAsync(identityId, did, cancellationToken);

        // Remove from cache
        var cacheKey = $"did_identity:{did}";
        _cache.Remove(cacheKey);

        return true;
    }

    /// <inheritdoc />
    public async Task<List<LinkedDIDInfo>> GetLinkedDIDsAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        // TODO: Query database for all DIDs linked to this identity
        // var linkedDids = await _identityRepository.GetLinkedDIDsAsync(identityId, cancellationToken);

        return new List<LinkedDIDInfo>(); // Placeholder
    }

    private async Task<bool> VerifySignatureAsync(
        string did,
        string message,
        string signature,
        string? verificationMethodId,
        string? algorithm,
        CancellationToken cancellationToken)
    {
        // In production, this would:
        // 1. Resolve the DID Document
        // 2. Find the verification method
        // 3. Verify the signature using the public key

        try
        {
            var signatureBytes = Convert.FromBase64String(signature);
            if (signatureBytes.Length < 64)
            {
                return false;
            }

            // TODO: Implement actual signature verification using Mamey.Auth.DecentralizedIdentifiers
            // var didDocument = await _didResolver.ResolveAsync(did);
            // var verificationMethod = didDocument.GetVerificationMethod(verificationMethodId);
            // return CryptoUtils.VerifySignature(message, signature, verificationMethod.PublicKey, algorithm);

            _logger.LogDebug(
                "Signature verification (simplified): DID={DID}, Algorithm={Algorithm}",
                did, algorithm ?? "default");

            return true; // Placeholder - implement actual verification
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying signature for DID {DID}", did);
            return false;
        }
    }

    private async Task<bool> VerifyDIDOwnershipProofAsync(
        string did,
        string proof,
        CancellationToken cancellationToken)
    {
        // Verify that the user owns the DID by checking the proof
        // This could be a signed message or a verification challenge response

        try
        {
            var proofBytes = Convert.FromBase64String(proof);
            return proofBytes.Length >= 64;
        }
        catch
        {
            return false;
        }
    }

    private async Task<(string accessToken, string refreshToken, DateTime expiresAt)> GenerateTokensAsync(
        Domain.Entities.Identity identity,
        string did,
        CancellationToken cancellationToken)
    {
        // Generate JWT tokens with DID claims
        // In production, this would use IJwtTokenService

        var accessToken = GenerateToken();
        var refreshToken = GenerateToken();
        var expiresAt = DateTime.UtcNow.AddHours(_options.AccessTokenExpirationHours);

        return (accessToken, refreshToken, expiresAt);
    }

    private static List<string> GetRoles(Domain.Entities.Identity identity)
    {
        // Extract roles from identity
        return new List<string> { "User" }; // Placeholder
    }

    private static string GenerateNonce()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string GenerateToken()
    {
        var bytes = new byte[48];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string ExtractDIDMethod(string did)
    {
        var parts = did.Split(':');
        return parts.Length >= 2 ? parts[1] : "unknown";
    }
}

/// <summary>
/// Configuration options for DID authentication.
/// </summary>
public class DIDAuthenticationOptions
{
    /// <summary>Challenge expiration time in seconds.</summary>
    public int ChallengeExpirationSeconds { get; set; } = 300;

    /// <summary>Access token expiration time in hours.</summary>
    public int AccessTokenExpirationHours { get; set; } = 1;

    /// <summary>Refresh token expiration time in days.</summary>
    public int RefreshTokenExpirationDays { get; set; } = 30;

    /// <summary>Whether to verify DID on blockchain.</summary>
    public bool VerifyOnBlockchain { get; set; } = true;

    /// <summary>Maximum failed authentication attempts before lockout.</summary>
    public int MaxFailedAttempts { get; set; } = 5;

    /// <summary>Lockout duration in minutes.</summary>
    public int LockoutDurationMinutes { get; set; } = 30;
}
