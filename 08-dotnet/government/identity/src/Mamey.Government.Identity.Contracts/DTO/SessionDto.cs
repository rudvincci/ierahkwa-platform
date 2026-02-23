using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class SessionDto
{
    public SessionDto(Guid id, Guid userId, string accessToken, string refreshToken, DateTime expiresAt, string status, string? ipAddress = null, string? userAgent = null, DateTime? lastAccessedAt = null, DateTime createdAt = default, DateTime? modifiedAt = null)
    {
        Id = id;
        UserId = userId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        Status = status;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        LastAccessedAt = lastAccessedAt;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Status { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}









