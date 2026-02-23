namespace Mamey.Templates.Services;

public interface ITemplateService
{
    /// <summary>Get a template by logical name; supports "Form-27@3" or "Form-27@latest".</summary>
    Task<TemplateBlob> GetAsync(string name, CancellationToken ct = default);

    /// <summary>Get a template with explicit access context; enforces policy.</summary>
    Task<TemplateBlob> GetAsync(string name, TemplateAccessContext context, CancellationToken ct = default);

    /// <summary>Get latest approved version number for an id; 0 if none.</summary>
    Task<int> GetLatestVersionAsync(TemplateId id, CancellationToken ct = default);
}



public interface ITemplateBlobStore
{
    /// <summary>Download raw bytes from storage reference (GridFS objectId, S3 key, etc.).</summary>
    Task<byte[]> DownloadAsync(string storageRef, CancellationToken ct = default);

    /// <summary>Upload bytes; returns storage reference string.</summary>
    Task<string> UploadAsync(string fileName, byte[] data, string contentType, CancellationToken ct = default);
}

public interface ITemplateCache
{
    Task<(byte[]? Data, string? Sha256)> TryGetAsync(TemplateId id, int version, CancellationToken ct = default);
    Task SetAsync(TemplateId id, int version, byte[] data, string sha256, TimeSpan ttl, CancellationToken ct = default);
    Task InvalidateAsync(TemplateId id, int version, CancellationToken ct = default);
}

public interface ITemplatePolicyEnforcer
{
    Task EnsureCanAccessAsync(TemplateId id, int version, TemplateAccessContext ctx, CancellationToken ct = default);
}