using Mamey.Templates.Services;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Mamey.Templates.Mongo.GridFS;

public sealed class GridFsTemplateBlobStore : ITemplateBlobStore
{
    private readonly GridFSBucket _bucket;

    public GridFsTemplateBlobStore(IMongoDatabase mongo, IOptions<MameyTemplatesOptions> opts)
    {
        _bucket = new GridFSBucket(mongo, new GridFSBucketOptions
        {
            BucketName = opts.Value.GridFsBucketName,
            ChunkSizeBytes = 255 * 1024 // 255KB
        });
    }

    public async Task<byte[]> DownloadAsync(string storageRef, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        await _bucket.DownloadToStreamAsync(ObjectId.Parse(storageRef), ms, cancellationToken: ct);
        return ms.ToArray();
    }

    public async Task<string> UploadAsync(string fileName, byte[] data, string contentType, CancellationToken ct = default)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "contentType", contentType },
                { "bytes", data.Length }
            }
        };
        using var s = new MemoryStream(data, writable: false);
        var id = await _bucket.UploadFromStreamAsync(fileName, s, options, ct);
        return id.ToString();
    }
}