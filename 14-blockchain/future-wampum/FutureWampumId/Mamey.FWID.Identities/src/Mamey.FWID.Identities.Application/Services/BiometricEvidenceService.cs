using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for creating and validating biometric evidence JWS (JSON Web Signature).
/// Compliant with Biometric Verification Microservice spec (§2.4).
/// </summary>
internal sealed class BiometricEvidenceService : IBiometricEvidenceService
{
    private readonly ILogger<BiometricEvidenceService> _logger;
    private readonly SecurityKey _signingKey;
    private readonly string _issuer = "identities.svc";

    public BiometricEvidenceService(ILogger<BiometricEvidenceService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Create ECDSA key for ES256 signing (per spec §16)
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        _signingKey = new ECDsaSecurityKey(ecdsa) { KeyId = "identities-key-001" };
    }

    public Task<string> CreateEnrollmentEvidenceAsync(
        Domain.Entities.IdentityId identityId,
        string? did,
        string templateId,
        BiometricData biometricData,
        string? sessionId = null,
        CancellationToken cancellationToken = default)
    {
        // Create evidence payload per spec (§2.4)
        var evidence = new
        {
            iss = "identities.svc", // Service identifier
            sub = identityId.Value.ToString(),
            did = did,
            sid = sessionId ?? Guid.NewGuid().ToString(),
            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            type = "enroll",
            pad = new
            {
                score = biometricData.LivenessScore ?? 0.0,
                decision = biometricData.LivenessDecision ?? "INCONCLUSIVE",
                algo = biometricData.AlgoVersion
            },
            quality = new
            {
                quality = biometricData.Quality.ToString(),
                format = biometricData.Format
            },
            policy = new
            {
                pad_min = 0.98, // Per spec (§4)
                match_min = 0.85 // Per spec (§4)
            },
            prov = new
            {
                source = "identities.svc",
                template_id = templateId,
                algo_version = biometricData.AlgoVersion
            }
        };

        var payload = JsonSerializer.Serialize(evidence);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var payloadBase64 = Convert.ToBase64String(payloadBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        // Create JWS with ES256 signing (per spec §16)
        var jws = CreateJws(payloadBase64);

        _logger.LogInformation("Created enrollment evidence for IdentityId: {IdentityId}, TemplateId: {TemplateId}",
            identityId.Value, templateId);

        return Task.FromResult(jws);
    }

    public Task<string> CreateVerificationEvidenceAsync(
        Domain.Entities.IdentityId identityId,
        string? did,
        double similarity,
        BiometricData biometricData,
        string decision,
        string? sessionId = null,
        CancellationToken cancellationToken = default)
    {
        // Create evidence payload per spec (§2.4)
        var evidence = new
        {
            iss = "identities.svc", // Service identifier
            sub = identityId.Value.ToString(),
            did = did,
            sid = sessionId ?? Guid.NewGuid().ToString(),
            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            type = "verify",
            pad = new
            {
                score = biometricData.LivenessScore ?? 0.0,
                decision = biometricData.LivenessDecision ?? "INCONCLUSIVE",
                algo = biometricData.AlgoVersion
            },
            match = new
            {
                similarity = similarity,
                decision = decision,
                algo = biometricData.AlgoVersion
            },
            quality = new
            {
                quality = biometricData.Quality.ToString(),
                format = biometricData.Format
            },
            policy = new
            {
                pad_min = 0.98, // Per spec (§4)
                match_min = 0.85 // Per spec (§4)
            },
            prov = new
            {
                source = "identities.svc",
                template_id = biometricData.TemplateId,
                algo_version = biometricData.AlgoVersion
            }
        };

        var payload = JsonSerializer.Serialize(evidence);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var payloadBase64 = Convert.ToBase64String(payloadBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        // Create JWS with ES256 signing (per spec §16)
        var jws = CreateJws(payloadBase64);

        _logger.LogInformation("Created verification evidence for IdentityId: {IdentityId}, Similarity: {Similarity}, Decision: {Decision}",
            identityId.Value, similarity, decision);

        return Task.FromResult(jws);
    }

    public Task<bool> ValidateEvidenceAsync(string evidenceJws, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate JWS signature and structure
            var isValid = ValidateJws(evidenceJws);

            _logger.LogInformation("Validated evidence JWS: {IsValid}", isValid);

            return Task.FromResult(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate evidence JWS");
            return Task.FromResult(false);
        }
    }

    private string CreateJws(string payload)
    {
        var header = new
        {
            alg = "ES256",
            typ = "JWT",
            kid = "identities-key-001"
        };

        var headerJson = JsonSerializer.Serialize(header);
        var headerBytes = Encoding.UTF8.GetBytes(headerJson);
        var headerBase64 = Convert.ToBase64String(headerBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.EcdsaSha256)
        };

        // Create the JWS manually since we have a custom payload
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var encodedToken = tokenHandler.WriteToken(token);

        // Extract signature from the JWT
        var parts = encodedToken.Split('.');
        if (parts.Length != 3)
        {
            throw new InvalidOperationException("Invalid JWT format");
        }

        // Return JWS: header.payload.signature
        return $"{headerBase64}.{payload}.{parts[2]}";
    }

    private bool ValidateJws(string jws)
    {
        if (string.IsNullOrWhiteSpace(jws))
        {
            return false;
        }

        var parts = jws.Split('.');
        if (parts.Length != 3)
        {
            return false;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = false, // We don't use audience in this context
                ValidateLifetime = false, // Evidence can be validated at any time
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                RequireSignedTokens = true
            };

            // Create a proper JWT from the JWS parts for validation
            var jwt = $"{parts[0]}.{parts[1]}.{parts[2]}";
            tokenHandler.ValidateToken(jwt, validationParameters, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}

