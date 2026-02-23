using Mamey.Persistence.Redis;
using Mamey.Templates.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Mamey.Templates.Caching;

public sealed class RedisTemplateCache : ITemplateCache
{
    private readonly ICache _cache;
    public RedisTemplateCache(ICache cache) => _cache = cache;

    private static string Key(TemplateId id, int ver) => $"tpl:{id.Value}:{ver}:docx";
    private static string KeySha(TemplateId id, int ver) => $"tpl:{id.Value}:{ver}:sha";

    public async Task<(byte[]? Data, string? Sha256)> TryGetAsync(TemplateId id, int version, CancellationToken ct = default)
    {
        var data = await _cache.GetAsync<byte[]>(Key(id, version));
        var sha = await _cache.GetAsync<string>(KeySha(id, version));
        return (data, sha);
    }

    public async Task SetAsync(TemplateId id, int version, byte[] data, string sha256, TimeSpan ttl, CancellationToken ct = default)
    {

        await _cache.SetAsync<byte[]>(Key(id, version), data, ttl);
        await _cache.SetAsync<string>(KeySha(id, version), sha256, ttl);
    }

    public async Task InvalidateAsync(TemplateId id, int version, CancellationToken ct = default)
    {
        await _cache.DeleteAsync<byte[]>(Key(id, version));
        await _cache.DeleteAsync<string>(KeySha(id, version));
    }
}