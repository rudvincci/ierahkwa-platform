using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class TwoFactorAuthDocument : IIdentifiable<Guid>
{
    public TwoFactorAuthDocument()
    {
    }

    public TwoFactorAuthDocument(TwoFactorAuth twoFactorAuth)
    {
        if (twoFactorAuth is null)
        {
            throw new NullReferenceException();
        }

        Id = twoFactorAuth.Id.Value;
        UserId = twoFactorAuth.UserId.Value;
        SecretKey = twoFactorAuth.SecretKey;
        QrCodeUrl = twoFactorAuth.QrCodeUrl;
        CreatedAt = twoFactorAuth.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = twoFactorAuth.ModifiedAt?.ToUnixTimeMilliseconds();
        ActivatedAt = twoFactorAuth.ActivatedAt?.ToUnixTimeMilliseconds();
        Status = twoFactorAuth.Status.ToString();
        LastUsedAt = twoFactorAuth.LastUsedAt?.ToUnixTimeMilliseconds();
        UsageCount = twoFactorAuth.UsageCount;
        FailedAttempts = twoFactorAuth.FailedAttempts;
        LastFailedAt = twoFactorAuth.LastFailedAt?.ToUnixTimeMilliseconds();
        BackupCodes = twoFactorAuth.BackupCodes.ToList();
        Version = twoFactorAuth.Version;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SecretKey { get; set; }
    public string QrCodeUrl { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public long? ActivatedAt { get; set; }
    public string Status { get; set; }
    public long? LastUsedAt { get; set; }
    public int UsageCount { get; set; }
    public int FailedAttempts { get; set; }
    public long? LastFailedAt { get; set; }
    public List<string> BackupCodes { get; set; } = new();
    public int Version { get; set; }

    public TwoFactorAuth AsEntity()
        => new TwoFactorAuth(
            Id,
            UserId,
            SecretKey,
            QrCodeUrl,
            CreatedAt.GetDate(),
            ActivatedAt?.GetDate(),
            Status.ToEnum<TwoFactorAuthStatus>(),
            Version);

    public TwoFactorAuthDto AsDto()
        => new TwoFactorAuthDto(
            Id,
            UserId,
            SecretKey,
            QrCodeUrl,
            BackupCodes,
            Status,
            CreatedAt.GetDate(),
            ActivatedAt?.GetDate());
}

