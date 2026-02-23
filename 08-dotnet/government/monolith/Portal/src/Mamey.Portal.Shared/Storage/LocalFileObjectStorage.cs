namespace Mamey.Portal.Shared.Storage;

public sealed class LocalFileObjectStorage : IObjectStorage
{
    private readonly string _rootPath;

    public LocalFileObjectStorage(string rootPath)
    {
        _rootPath = rootPath;
    }

    public async Task PutAsync(
        string bucket,
        string key,
        Stream content,
        long size,
        string contentType,
        IReadOnlyDictionary<string, string>? metadata = null,
        CancellationToken ct = default)
    {
        // Local adapter for UI-first development; swap for MinIO in containerized environments.
        var safeBucket = bucket.Replace("..", string.Empty, StringComparison.Ordinal).Trim('/');
        var safeKey = key.Replace("..", string.Empty, StringComparison.Ordinal).TrimStart('/');

        var fullPath = Path.Combine(_rootPath, safeBucket, safeKey.Replace('/', Path.DirectorySeparatorChar));
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(dir))
        {
            Directory.CreateDirectory(dir);
        }

        await using var fs = File.Create(fullPath);
        await content.CopyToAsync(fs, ct);

        // Optional metadata sidecar for local dev visibility.
        if (metadata is not null && metadata.Count > 0)
        {
            var metaPath = fullPath + ".meta.json";
            var json = System.Text.Json.JsonSerializer.Serialize(metadata, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metaPath, json, System.Text.Encoding.UTF8, ct);
        }
    }

    public Task<ObjectStorageReadResult> GetAsync(string bucket, string key, CancellationToken ct = default)
    {
        // NOTE: For local storage we don't persist metadata (content-type/size) separately.
        // This is strictly a UI-first dev adapter.
        var safeBucket = bucket.Replace("..", string.Empty, StringComparison.Ordinal).Trim('/');
        var safeKey = key.Replace("..", string.Empty, StringComparison.Ordinal).TrimStart('/');

        var fullPath = Path.Combine(_rootPath, safeBucket, safeKey.Replace('/', Path.DirectorySeparatorChar));
        var fi = new FileInfo(fullPath);
        if (!fi.Exists)
        {
            throw new FileNotFoundException("Object not found.", fullPath);
        }

        Stream stream = File.OpenRead(fullPath);
        var fileName = Path.GetFileName(fullPath);

        // Best-effort content-type.
        var contentType = "application/octet-stream";
        if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) contentType = "image/png";
        else if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)) contentType = "image/jpeg";
        else if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) contentType = "application/pdf";

        return Task.FromResult(new ObjectStorageReadResult(stream, contentType, fi.Length, fileName));
    }
}


