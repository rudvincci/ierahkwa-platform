using Mamey.Persistence.Redis;
using Mamey.Templates.EF;
using Mamey.Templates.Registries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Mamey.Templates.Mongo.GridFS;

public class GridFsTemplateRepository : ITemplateRepository
{
    private readonly TemplatesDbContext _pg;            // Postgres registry via EF
    private readonly IMongoDatabase _mongo;
    private readonly ICache _cache;          // Redis; or use StackExchange.Redis raw
    private readonly GridFSBucket _bucket;

    public Task<TemplateBlob> GetAsync(string templateName, int version, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetLatestVersionAsync(TemplateId id, CancellationToken ct)
        {
            return await _pg.Set<DocumentTemplate>()
                .Where(t => t.Id == id && t.Status == DocumentTemplateStatus.Approved)
                .MaxAsync(t => t.Version, ct);
        }

    public Task<int> GetLatestVersionAsync(string templateName, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<TemplateBlob> GetAsync(TemplateId id, int version, CancellationToken ct)
        {
            var cacheKey = $"tpl:{id}:{version}:docx";
            var cached = await _cache.GetAsync<TemplateBlob?>(cacheKey);
            if (cached is not null)
            {
                var sha = await _cache.GetAsync<string>(cacheKey + ":sha");
                return new(id, version, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", cached.Data, sha ?? "");
            }
    
            var row = await _pg.Set<DocumentTemplate>().FindAsync(id, version, ct);
            if (row is null) throw new FileNotFoundException($"Template {id} v{version} not found.");
    
            var objectId = ObjectId.Parse(row.StorageRef);
            using var ms = new MemoryStream();
            await _bucket.DownloadToStreamAsync(objectId, ms, cancellationToken: ct);
            var data = ms.ToArray();
    
            // Verify integrity
            var sha256 = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(data)).ToLowerInvariant();
            if (!sha256.Equals(row.Sha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Template hash mismatch.");
    
            await _cache.SetAsync(cacheKey, data, TimeSpan.FromHours(24));
            await _cache.SetAsync<string>(cacheKey + ":sha", sha256, TimeSpan.FromHours(24));
    
            return new(id, version, row.ContentType, data, sha256);
        }
}