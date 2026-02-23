using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class EmailConfirmationDto
{
    public EmailConfirmationDto(Guid id, Guid userId, string email, string confirmationCode, DateTime expiresAt, string? ipAddress, string? userAgent, string status, DateTime createdAt, DateTime? confirmedAt)
    {
        Id = id;
        UserId = userId;
        Email = email;
        ConfirmationCode = confirmationCode;
        ExpiresAt = expiresAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Status = status;
        CreatedAt = createdAt;
        ConfirmedAt = confirmedAt;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string ConfirmationCode { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

