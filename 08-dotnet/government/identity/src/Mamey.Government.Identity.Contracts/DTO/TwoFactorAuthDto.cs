using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class TwoFactorAuthDto
{
    public TwoFactorAuthDto(Guid id, Guid userId, string secretKey, string qrCodeUrl, IEnumerable<string> backupCodes, string status, DateTime createdAt, DateTime? activatedAt)
    {
        Id = id;

        UserId = userId;
        SecretKey = secretKey;
        QrCodeUrl = qrCodeUrl;
        BackupCodes = backupCodes;
        Status = status;
        CreatedAt = createdAt;
        ActivatedAt = activatedAt;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SecretKey { get; set; }
    public string QrCodeUrl { get; set; }
    public IEnumerable<string> BackupCodes { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
}

