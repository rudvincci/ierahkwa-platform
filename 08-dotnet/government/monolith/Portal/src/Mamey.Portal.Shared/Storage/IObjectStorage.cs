namespace Mamey.Portal.Shared.Storage;

public interface IObjectStorage
{
    Task PutAsync(
        string bucket,
        string key,
        Stream content,
        long size,
        string contentType,
        IReadOnlyDictionary<string, string>? metadata = null,
        CancellationToken ct = default);

    Task<ObjectStorageReadResult> GetAsync(
        string bucket,
        string key,
        CancellationToken ct = default);
}

public sealed record ObjectStorageReadResult(
    Stream Content,
    string ContentType,
    long Size,
    string FileName);


