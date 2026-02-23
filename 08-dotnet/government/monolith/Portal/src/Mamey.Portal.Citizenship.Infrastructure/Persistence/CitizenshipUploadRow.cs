namespace Mamey.Portal.Citizenship.Infrastructure.Persistence;

public sealed class CitizenshipUploadRow
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }

    // "PersonalDocument" | "PassportPhoto" | "SignatureImage"
    public string Kind { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }

    public string StorageBucket { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;

    public DateTimeOffset UploadedAt { get; set; }
}




