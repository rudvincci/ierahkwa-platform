using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Models.Requests;
using Mamey.Portal.Shared.Storage;

namespace Mamey.Portal.Citizenship.Infrastructure.Storage;

public sealed class MinioObjectStorage : IObjectStorage
{
    private readonly IBucketService _buckets;
    private readonly IObjectService _objects;

    public MinioObjectStorage(IBucketService buckets, IObjectService objects)
    {
        _buckets = buckets;
        _objects = objects;
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
        if (!await _buckets.BucketExistsAsync(bucket, ct))
        {
            await _buckets.MakeBucketAsync(bucket, ct);
        }

        await _objects.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucket,
            ObjectName = key,
            Data = content,
            Size = size,
            ContentType = contentType,
            Metadata = metadata is null ? null : new Dictionary<string, string>(metadata),
        }, ct);
    }

    public async Task<ObjectStorageReadResult> GetAsync(string bucket, string key, CancellationToken ct = default)
    {
        var meta = await _objects.StatObjectAsync(bucket, key, ct);

        var ms = new MemoryStream();
        await _objects.DownloadAsync(bucket, key, ms, cancellationToken: ct);
        ms.Position = 0;

        var fileName = Path.GetFileName(key);
        var contentType = string.IsNullOrWhiteSpace(meta.ContentType) ? "application/octet-stream" : meta.ContentType;
        var size = meta.Size;

        return new ObjectStorageReadResult(ms, contentType, size, fileName);
    }
}


