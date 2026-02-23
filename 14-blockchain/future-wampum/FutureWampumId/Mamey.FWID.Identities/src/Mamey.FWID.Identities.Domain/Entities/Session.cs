using System.Text.Json.Serialization;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents an active session for an identity.
/// </summary>
public class Session : AggregateRoot<SessionId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Session()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Session aggregate root.
    /// </summary>
    /// <param name="id">The session identifier.</param>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="accessToken">The access token.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="expiresAt">The expiration date and time.</param>
    [JsonConstructor]
    public Session(
        SessionId id,
        IdentityId identityId,
        string accessToken,
        string refreshToken,
        DateTime expiresAt,
        string? ipAddress = null,
        string? userAgent = null)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
        ExpiresAt = expiresAt;
        Status = SessionStatus.Active;
        CreatedAt = DateTime.UtcNow;
        LastAccessedAt = CreatedAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        TrustScore = CalculateInitialTrustScore(accessToken, ipAddress, userAgent);
        RiskFactors = new List<string>();

        AddEvent(new SessionCreated(Id, IdentityId, IpAddress, UserAgent, CreatedAt, ExpiresAt));
    }

    /// <summary>
    /// The identity identifier.
    /// </summary>
    public IdentityId IdentityId { get; private set; } = null!;

    /// <summary>
    /// The access token.
    /// </summary>
    public string AccessToken { get; private set; } = null!;

    /// <summary>
    /// The refresh token.
    /// </summary>
    public string RefreshToken { get; private set; } = null!;

    /// <summary>
    /// The expiration date and time.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// The status of the session.
    /// </summary>
    public SessionStatus Status { get; private set; }

    /// <summary>
    /// Date and time the session was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the session was last accessed.
    /// </summary>
    public DateTime LastAccessedAt { get; private set; }

    /// <summary>
    /// Date and time the session was revoked.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// The IP address of the client.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// The user agent of the client.
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// The trust score for this session (0-100).
    /// Based on authentication method, behavioral patterns, and risk factors.
    /// </summary>
    public int TrustScore { get; private set; }

    /// <summary>
    /// The device fingerprint for this session.
    /// </summary>
    public string? DeviceFingerprint { get; private set; }

    /// <summary>
    /// Geographic location information.
    /// </summary>
    public string? Location { get; private set; }

    /// <summary>
    /// Risk factors associated with this session.
    /// </summary>
    public List<string> RiskFactors { get; private set; } = new();

    /// <summary>
    /// Sets the IP address and user agent.
    /// </summary>
    /// <param name="ipAddress">The IP address.</param>
    /// <param name="userAgent">The user agent.</param>
    public void SetClientInfo(string? ipAddress, string? userAgent)
    {
        IpAddress = ipAddress;
        UserAgent = userAgent;
        IncrementVersion();
    }

    /// <summary>
    /// Updates the access token and refresh token.
    /// </summary>
    /// <param name="accessToken">The new access token.</param>
    /// <param name="refreshToken">The new refresh token.</param>
    public void UpdateTokens(string accessToken, string refreshToken)
    {
        AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
        LastAccessedAt = DateTime.UtcNow;
        IncrementVersion();
    }

    /// <summary>
    /// Updates the last accessed time.
    /// </summary>
    public void UpdateLastAccessed()
    {
        LastAccessedAt = DateTime.UtcNow;
        IncrementVersion();
    }

    /// <summary>
    /// Revokes the session.
    /// </summary>
    public void Revoke()
    {
        if (Status == SessionStatus.Revoked)
            return; // Already revoked
        
        Status = SessionStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        
        AddEvent(new SessionRevoked(Id, IdentityId, RevokedAt.Value));
    }

    /// <summary>
    /// Marks the session as expired.
    /// </summary>
    public void Expire()
    {
        if (Status == SessionStatus.Expired)
            return; // Already expired
        
        Status = SessionStatus.Expired;
        
        AddEvent(new SessionExpired(Id, IdentityId, DateTime.UtcNow));
    }

    /// <summary>
    /// Checks if the session is valid.
    /// </summary>
    public bool IsValid()
    {
        return Status == SessionStatus.Active &&
               ExpiresAt > DateTime.UtcNow &&
               TrustScore >= 20; // Minimum trust threshold
    }

    /// <summary>
    /// Updates the trust score based on behavioral analysis.
    /// </summary>
    /// <param name="newScore">The new trust score (0-100).</param>
    /// <param name="reason">The reason for the score change.</param>
    public void UpdateTrustScore(int newScore, string reason)
    {
        if (newScore < 0 || newScore > 100)
            throw new ArgumentException("Trust score must be between 0 and 100", nameof(newScore));

        var oldScore = TrustScore;
        TrustScore = newScore;
        LastAccessedAt = DateTime.UtcNow;

        if (newScore < 50)
        {
            RiskFactors.Add($"Low trust score: {reason}");
        }

        IncrementVersion();

        AddEvent(new SessionTrustScoreChanged(Id, oldScore, newScore, reason));
    }

    /// <summary>
    /// Records suspicious activity on the session.
    /// </summary>
    /// <param name="activity">The suspicious activity description.</param>
    /// <param name="severity">The severity level (1-10).</param>
    public void RecordSuspiciousActivity(string activity, int severity = 5)
    {
        RiskFactors.Add($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}: {activity} (severity: {severity})");

        // Reduce trust score based on severity
        var scoreReduction = Math.Min(severity * 5, TrustScore);
        UpdateTrustScore(TrustScore - scoreReduction, $"Suspicious activity: {activity}");

        AddEvent(new SessionSuspiciousActivityDetected(Id, activity, severity));
    }

    /// <summary>
    /// Sets the device fingerprint for behavioral analysis.
    /// </summary>
    /// <param name="fingerprint">The device fingerprint.</param>
    public void SetDeviceFingerprint(string fingerprint)
    {
        DeviceFingerprint = fingerprint;
        LastAccessedAt = DateTime.UtcNow;
        IncrementVersion();
    }

    /// <summary>
    /// Updates the geographic location.
    /// </summary>
    /// <param name="location">The location information.</param>
    public void UpdateLocation(string location)
    {
        if (Location != location)
        {
            var oldLocation = Location;
            Location = location;
            LastAccessedAt = DateTime.UtcNow;

            // Location change might be suspicious
            if (!string.IsNullOrEmpty(oldLocation))
            {
                RecordSuspiciousActivity($"Location changed from {oldLocation} to {location}", 3);
            }

            IncrementVersion();
        }
    }

    /// <summary>
    /// Checks if the session needs additional verification.
    /// </summary>
    /// <returns>True if additional verification is recommended.</returns>
    public bool NeedsAdditionalVerification()
    {
        return TrustScore < 50 ||
               RiskFactors.Count > 3 ||
               (DateTime.UtcNow - CreatedAt) > TimeSpan.FromDays(7); // Long-lived sessions
    }

    /// <summary>
    /// Calculates the initial trust score based on authentication factors.
    /// </summary>
    private static int CalculateInitialTrustScore(string accessToken, string? ipAddress, string? userAgent)
    {
        var score = 60; // Base score

        // Token complexity (longer tokens are generally more secure)
        if (accessToken.Length > 100)
            score += 10;

        // IP address presence
        if (!string.IsNullOrEmpty(ipAddress))
            score += 5;

        // User agent presence
        if (!string.IsNullOrEmpty(userAgent))
            score += 5;

        // Check for suspicious patterns
        if (!string.IsNullOrEmpty(userAgent) &&
            (userAgent.Contains("bot") || userAgent.Contains("crawler")))
        {
            score -= 20;
        }

        return Math.Clamp(score, 0, 100);
    }
}

