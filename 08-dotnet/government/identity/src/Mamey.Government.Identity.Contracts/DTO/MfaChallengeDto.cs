using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class MfaChallengeDto
{
    public MfaChallengeDto(Guid id, Guid multiFactorAuthId, string method, string challengeData, DateTime expiresAt, string? ipAddress, string? userAgent, string status, DateTime createdAt, DateTime? verifiedAt)
    {
        Id = id;
        MultiFactorAuthId = multiFactorAuthId;
        Method = method;
        ChallengeData = challengeData;
        ExpiresAt = expiresAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Status = status;
        CreatedAt = createdAt;
        VerifiedAt = verifiedAt;
    }

    public Guid Id { get; set; }
    public Guid MultiFactorAuthId { get; set; }
    public string Method { get; set; }
    public string ChallengeData { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
}

