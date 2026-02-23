using System.ComponentModel;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class Session : AggregateRoot<SessionId>
{
    public Session(SessionId id, UserId userId, string accessToken, string refreshToken,
        DateTime expiresAt, DateTime createdAt, string? ipAddress = null, string? userAgent = null,
        SessionStatus status = SessionStatus.Active, int version = 0)
        : base(id, version)
    {
        UserId = userId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Status = status;
    }

    #region Properties

    /// <summary>
    /// Reference to the user this session belongs to.
    /// </summary>
    [Description("Reference to the user this session belongs to")]
    public UserId UserId { get; private set; }

    /// <summary>
    /// Access token for API authentication.
    /// </summary>
    [Description("Access token for API authentication")]
    public string AccessToken { get; private set; }

    /// <summary>
    /// Refresh token for obtaining new access tokens.
    /// </summary>
    [Description("Refresh token for obtaining new access tokens")]
    public string RefreshToken { get; private set; }

    /// <summary>
    /// Date and time when the session expires.
    /// </summary>
    [Description("Date and time when the session expires")]
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Date and time the session was created.
    /// </summary>
    [Description("Date and time the session was created")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the session was last modified.
    /// </summary>
    [Description("Date and time the session was last modified")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Date and time the session was last accessed.
    /// </summary>
    [Description("Date and time the session was last accessed")]
    public DateTime? LastAccessedAt { get; private set; }

    /// <summary>
    /// IP address from which the session was created.
    /// </summary>
    [Description("IP address from which the session was created")]
    public string? IpAddress { get; private set; }

    /// <summary>
    /// User agent string from the client.
    /// </summary>
    [Description("User agent string from the client")]
    public string? UserAgent { get; private set; }

    /// <summary>
    /// Current status of the session.
    /// </summary>
    [Description("Current status of the session")]
    public SessionStatus Status { get; private set; }
    #endregion

    public static Session Create(Guid id, Guid userId, string accessToken, string refreshToken,
        DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new MissingAccessTokenException();
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new MissingRefreshTokenException();
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new InvalidSessionExpirationException();
        }

        var session = new Session(id, userId, accessToken, refreshToken, expiresAt, DateTime.UtcNow, ipAddress, userAgent);
        session.AddEvent(new SessionCreated(session));
        return session;
    }

    public void Refresh(string newAccessToken, string newRefreshToken, DateTime newExpiresAt)
    {
        if (Status != SessionStatus.Active)
        {
            throw new SessionNotActiveException();
        }

        if (string.IsNullOrWhiteSpace(newAccessToken))
        {
            throw new MissingAccessTokenException();
        }

        if (string.IsNullOrWhiteSpace(newRefreshToken))
        {
            throw new MissingRefreshTokenException();
        }

        if (newExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidSessionExpirationException();
        }

        AccessToken = newAccessToken;
        RefreshToken = newRefreshToken;
        ExpiresAt = newExpiresAt;
        LastAccessedAt = DateTime.UtcNow;
        
        AddEvent(new SessionRefreshed(this));
    }

    public void Extend(DateTime newExpiresAt)
    {
        if (Status != SessionStatus.Active)
        {
            throw new SessionNotActiveException();
        }

        if (newExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidSessionExpirationException();
        }

        ExpiresAt = newExpiresAt;
        LastAccessedAt = DateTime.UtcNow;
        
        AddEvent(new SessionExtended(this, newExpiresAt));
    }

    public void RecordAccess()
    {
        if (Status != SessionStatus.Active)
        {
            throw new SessionNotActiveException();
        }

        LastAccessedAt = DateTime.UtcNow;
        AddEvent(new SessionAccessed(this));
    }

    public void Revoke()
    {
        if (Status == SessionStatus.Revoked)
        {
            throw new SessionAlreadyRevokedException();
        }

        Status = SessionStatus.Revoked;
        AddEvent(new SessionRevoked(this));
    }

    public void Expire()
    {
        if (Status == SessionStatus.Expired)
        {
            throw new SessionAlreadyExpiredException();
        }

        Status = SessionStatus.Expired;
        AddEvent(new SessionExpired(this));
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt || Status == SessionStatus.Expired;
    }

    public bool IsActive()
    {
        return Status == SessionStatus.Active && !IsExpired();
    }

    public bool CanRefresh()
    {
        return Status == SessionStatus.Active && !IsExpired();
    }
}