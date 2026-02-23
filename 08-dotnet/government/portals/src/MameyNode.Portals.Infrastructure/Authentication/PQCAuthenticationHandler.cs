using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mamey.Auth;

namespace MameyNode.Portals.Infrastructure.Authentication;

/// <summary>
/// Configuration options for post-quantum authentication within the
/// MameyNode portals multi-auth setup.
/// </summary>
public sealed class PostQuantumAuthOptions
{
    public bool Enabled { get; set; }
    public bool RequirePQC { get; set; }
    public bool AcceptHybrid { get; set; } = true;
    public DateTimeOffset? MandatoryDate { get; set; }
    public string DefaultAlgorithm { get; set; } = "ML-DSA-65";
}

/// <summary>
/// Lightweight handler that evaluates whether an incoming JWT / PQC JWT
/// should be accepted based on portal configuration.
/// It understands both classical 3-part JWTs and PQC/hybrid 4-part tokens
/// produced by <see cref="PostQuantumJwtToken"/>.
/// </summary>
public sealed class PQCAuthenticationHandler
{
    private readonly PostQuantumAuthOptions _options;
    private readonly ILogger<PQCAuthenticationHandler> _logger;

    public PQCAuthenticationHandler(IConfiguration configuration, ILogger<PQCAuthenticationHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = BindOptions(configuration ?? throw new ArgumentNullException(nameof(configuration)));
    }

    private static PostQuantumAuthOptions BindOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection("multiAuth:postQuantum");
        var options = new PostQuantumAuthOptions();

        if (!section.Exists())
        {
            return options;
        }

        options.Enabled = section.GetValue("enabled", false);
        options.RequirePQC = section.GetValue("requirePQC", false);
        options.AcceptHybrid = section.GetValue("acceptHybrid", true);

        var mandatoryDate = section["mandatoryDate"];
        if (!string.IsNullOrWhiteSpace(mandatoryDate))
        {
            if (!DateTimeOffset.TryParse(mandatoryDate, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dto))
            {
                throw new FormatException("multiAuth.postQuantum.mandatoryDate must be a valid ISO 8601 date.");
            }

            options.MandatoryDate = dto;
        }

        var algorithm = section["defaultAlgorithm"] ?? "ML-DSA-65";
        if (!IsSupportedAlgorithm(algorithm))
        {
            throw new ArgumentException($"Unsupported post-quantum algorithm '{algorithm}' in multiAuth.postQuantum.defaultAlgorithm.");
        }

        options.DefaultAlgorithm = algorithm;
        return options;
    }

    private static bool IsSupportedAlgorithm(string algorithm)
    {
        return algorithm is "ML-DSA-44" or "ML-DSA-65" or "ML-DSA-87" or "HYBRID-RSA-MLDSA65";
    }

    /// <summary>
    /// Evaluates whether the provided token is allowed under the current
    /// PQC policy. Also reports whether the token appears PQC or hybrid
    /// based on its header.
    /// </summary>
    public bool IsTokenAllowed(string token, DateTimeOffset now, out bool isPqcToken, out bool isHybridToken)
    {
        isPqcToken = false;
        isHybridToken = false;

        if (!_options.Enabled)
        {
            // PQC policy is disabled; allow token to flow to existing
            // JWT/DID/Certificate mechanisms unchanged.
            return true;
        }

        PostQuantumJwtToken parsed;
        try
        {
            parsed = PostQuantumJwtToken.Parse(token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse token as PostQuantumJwtToken.");
            return false;
        }

        string headerJson;
        try
        {
            headerJson = PostQuantumJwtToken.Base64UrlDecodeToString(parsed.Header);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to decode JWT header.");
            return false;
        }

        using var headerDoc = JsonDocument.Parse(headerJson);
        if (!headerDoc.RootElement.TryGetProperty("alg", out var algProp))
        {
            _logger.LogWarning("JWT header does not contain 'alg' field.");
            return false;
        }

        var alg = algProp.GetString() ?? string.Empty;
        isHybridToken = alg.StartsWith("HYBRID", StringComparison.OrdinalIgnoreCase);
        isPqcToken = alg.Contains("ML-DSA", StringComparison.OrdinalIgnoreCase);

        var afterMandatory = _options.RequirePQC &&
                             _options.MandatoryDate.HasValue &&
                             now >= _options.MandatoryDate.Value;

        if (!afterMandatory)
        {
            // Before the PQC mandate, classical JWTs and PQC/hybrid tokens
            // are all permitted, subject to AcceptHybrid flag.
            if (!_options.AcceptHybrid && isHybridToken)
            {
                _logger.LogInformation("Hybrid token rejected: hybrid not accepted before mandatory date.");
                return false;
            }

            return true;
        }

        // After the mandatory date with RequirePQC=true, only PQC / hybrid
        // tokens are accepted according to configuration.
        if (!isPqcToken && !isHybridToken)
        {
            _logger.LogInformation("Classical JWT rejected: PQC is required after mandatory date.");
            return false;
        }

        if (isHybridToken && !_options.AcceptHybrid)
        {
            _logger.LogInformation("Hybrid token rejected: AcceptHybrid=false after mandatory date.");
            return false;
        }

        return true;
    }
}


