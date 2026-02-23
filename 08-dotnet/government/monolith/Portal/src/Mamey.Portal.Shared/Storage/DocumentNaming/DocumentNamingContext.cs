namespace Mamey.Portal.Shared.Storage.DocumentNaming;

public sealed record DocumentNamingContext(
    string TenantId,
    string ApplicationNumber,
    Guid ApplicationId,
    string Kind,
    string OriginalFileName,
    DateTimeOffset NowUtc);




