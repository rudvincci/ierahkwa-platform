using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Mamey.Portal.Library.Application.Models;
using Mamey.Portal.Library.Application.Requests;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Storage;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Library.Application.Services;

public sealed class LibraryService : ILibraryService
{
    private static readonly Regex InvalidPathChars = new(@"[^a-zA-Z0-9\-\._/]+", RegexOptions.Compiled);
    private const long MaxFileBytes = 25 * 1024 * 1024;
    private static readonly JsonSerializerOptions CacheJsonOptions = new(JsonSerializerDefaults.Web);
    private const string CachePrefix = "mamey.portal.library";

    private readonly ILibraryStore _store;
    private readonly ITenantContext _tenant;
    private readonly ICurrentUserContext _user;
    private readonly IObjectStorage _storage;
    private readonly IDistributedCache _cache;

    public LibraryService(
        ILibraryStore store,
        ITenantContext tenant,
        ICurrentUserContext user,
        IObjectStorage storage,
        IDistributedCache cache)
    {
        _store = store;
        _tenant = tenant;
        _user = user;
        _storage = storage;
        _cache = cache;
    }

    public async Task<IReadOnlyList<LibraryItem>> GetPublishedPublicAsync(string? searchTerm = null, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        return await GetCachedAsync(
            tenantId,
            "public",
            searchTerm,
            () => _store.GetPublishedPublicAsync(tenantId, 500, searchTerm, ct),
            ct);
    }

    public async Task<IReadOnlyList<LibraryItem>> GetPublishedForCurrentUserAsync(string? searchTerm = null, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var maxVisibility = GetMaxVisibilityForUser();
        return await GetCachedAsync(
            tenantId,
            $"visibility:{maxVisibility}",
            searchTerm,
            () => _store.GetPublishedForVisibilityAsync(tenantId, maxVisibility, 500, searchTerm, ct),
            ct);
    }

    public async Task<IReadOnlyList<LibraryItem>> GetAllAsync(string? searchTerm = null, CancellationToken ct = default)
    {
        EnsureCanEdit();
        var tenantId = _tenant.TenantId;
        var rows = await _store.GetAllAsync(tenantId, 1000, searchTerm, ct);
        return rows.Select(Map).ToList();
    }

    public async Task<LibraryItem> CreateDraftAsync(
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        LibraryUploadFile file,
        CancellationToken ct = default)
    {
        EnsureCanEdit();

        var tenantId = _tenant.TenantId;
        category = (category ?? string.Empty).Trim();
        title = (title ?? string.Empty).Trim();
        summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim();

        ValidateMetadata(category, title);
        ValidateFile(file);

        var bucket = ObjectStorageKeys.TenantBucket(tenantId);
        var key = BuildStorageKey(tenantId, file.FileName);

        await _storage.PutAsync(
            bucket,
            key,
            file.Content,
            file.Size,
            file.ContentType,
            new Dictionary<string, string>
            {
                ["mamey-tenant"] = tenantId,
                ["mamey-domain"] = "library",
                ["mamey-title"] = title,
                ["mamey-category"] = category,
                ["mamey-visibility"] = visibility.ToString(),
                ["mamey-original-file-name"] = file.FileName,
            },
            ct);

        var now = DateTimeOffset.UtcNow;
        var row = await _store.CreateDraftAsync(
            tenantId,
            category,
            title,
            summary,
            visibility,
            SafeFileName(file.FileName),
            file.ContentType,
            file.Size,
            bucket,
            key,
            _user.UserName,
            now,
            ct);

        await BumpCacheVersionAsync(tenantId, ct);
        return Map(row);
    }

    public async Task<LibraryItem> UpdateDraftAsync(
        Guid id,
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        LibraryUploadFile? replaceFile,
        CancellationToken ct = default)
    {
        EnsureCanEdit();

        var tenantId = _tenant.TenantId;
        category = (category ?? string.Empty).Trim();
        title = (title ?? string.Empty).Trim();
        summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim();

        ValidateMetadata(category, title);

        var existing = await _store.GetAsync(tenantId, id, ct)
                       ?? throw new InvalidOperationException("Library item not found.");

        if (existing.Status != LibraryContentStatus.Draft && existing.Status != LibraryContentStatus.Unpublished)
        {
            throw new InvalidOperationException("Only Draft/Unpublished items can be edited.");
        }

        string? fileName = null;
        string? contentType = null;
        long? size = null;
        string? bucket = null;
        string? key = null;

        if (replaceFile is not null)
        {
            ValidateFile(replaceFile);
            bucket = ObjectStorageKeys.TenantBucket(tenantId);
            key = BuildStorageKey(tenantId, replaceFile.FileName);

            await _storage.PutAsync(
                bucket,
                key,
                replaceFile.Content,
                replaceFile.Size,
                replaceFile.ContentType,
                new Dictionary<string, string>
                {
                    ["mamey-tenant"] = tenantId,
                    ["mamey-domain"] = "library",
                    ["mamey-title"] = title,
                    ["mamey-category"] = category,
                    ["mamey-visibility"] = visibility.ToString(),
                    ["mamey-original-file-name"] = replaceFile.FileName,
                },
                ct);

            fileName = SafeFileName(replaceFile.FileName);
            contentType = replaceFile.ContentType;
            size = replaceFile.Size;
        }

        var row = await _store.UpdateDraftAsync(
            tenantId,
            id,
            category,
            title,
            summary,
            visibility,
            fileName,
            contentType,
            size,
            bucket,
            key,
            _user.UserName,
            DateTimeOffset.UtcNow,
            ct);

        await BumpCacheVersionAsync(tenantId, ct);
        return Map(row);
    }

    public async Task<LibraryItem> PublishAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;

        var existing = await _store.GetAsync(tenantId, id, ct)
                       ?? throw new InvalidOperationException("Library item not found.");

        if (existing.Status != LibraryContentStatus.Draft && existing.Status != LibraryContentStatus.Unpublished)
        {
            throw new InvalidOperationException("Only Draft/Unpublished items can be published.");
        }

        var row = await _store.PublishAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        await BumpCacheVersionAsync(tenantId, ct);
        return Map(row);
    }

    public async Task<LibraryItem> UnpublishAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;

        var existing = await _store.GetAsync(tenantId, id, ct)
                       ?? throw new InvalidOperationException("Library item not found.");

        if (existing.Status != LibraryContentStatus.Published)
        {
            throw new InvalidOperationException("Only Published items can be unpublished.");
        }

        var row = await _store.UnpublishAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        await BumpCacheVersionAsync(tenantId, ct);
        return Map(row);
    }

    private static void ValidateMetadata(string category, string title)
    {
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
        if (category.Length > 128) throw new ArgumentException("Category is too long.");
        if (title.Length > 256) throw new ArgumentException("Title is too long.");
    }

    private static void ValidateFile(LibraryUploadFile file)
    {
        if (file.Content is null) throw new ArgumentException("File content is required.");
        if (file.Size <= 0) throw new ArgumentException("File is empty.");
        if (file.Size > MaxFileBytes) throw new ArgumentException($"File is too large (max {MaxFileBytes / (1024 * 1024)}MB).");
        if (string.IsNullOrWhiteSpace(file.FileName)) throw new ArgumentException("FileName is required.");
        if (string.IsNullOrWhiteSpace(file.ContentType)) throw new ArgumentException("ContentType is required.");
    }

    private static string BuildStorageKey(string tenantId, string fileName)
    {
        var safe = SafeFileName(fileName);
        var unique = Guid.NewGuid().ToString("N");
        var date = DateTimeOffset.UtcNow.ToString("yyyyMMdd");
        var key = $"library/{tenantId}/{date}-{unique}-{safe}";
        key = key.Replace('\\', '/').TrimStart('/');
        return InvalidPathChars.Replace(key, "-");
    }

    private static string SafeFileName(string fileName)
    {
        fileName = string.IsNullOrWhiteSpace(fileName) ? "upload.bin" : Path.GetFileName(fileName);
        fileName = InvalidPathChars.Replace(fileName, "-");
        return fileName.Trim('-');
    }

    private LibraryVisibility GetMaxVisibilityForUser()
    {
        if (_user.IsAuthenticated && _user.IsInRole("GovernmentAgent"))
        {
            return LibraryVisibility.Government;
        }
        if (_user.IsAuthenticated && _user.IsInRole("Admin"))
        {
            return LibraryVisibility.Government;
        }
        if (_user.IsAuthenticated && _user.IsInRole("Citizen"))
        {
            return LibraryVisibility.Citizen;
        }

        return LibraryVisibility.Public;
    }

    private void EnsureCanEdit()
    {
        // Keep roles simple for now; Auth workflow will centralize this later.
        if (!_user.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Not authenticated.");
        }

        if (!_user.IsInRole("LibraryEditor")
            && !_user.IsInRole("Admin")
            && !_user.IsInRole("GovernmentAgent"))
        {
            throw new UnauthorizedAccessException("Not authorized.");
        }
    }

    private void EnsureIsAdmin()
    {
        if (!_user.IsAuthenticated || !_user.IsInRole("Admin"))
        {
            throw new UnauthorizedAccessException("Admin required.");
        }
    }

    private static LibraryItem Map(LibraryItemSnapshot row)
        => new(
            row.Id,
            row.TenantId,
            row.Category,
            row.Title,
            row.Summary,
            row.Visibility,
            row.Status,
            row.FileName,
            row.ContentType,
            row.Size,
            row.StorageBucket,
            row.StorageKey,
            row.CreatedAt,
            row.UpdatedAt,
            row.CreatedBy,
            row.UpdatedBy,
            row.PublishedAt);

    private async Task<IReadOnlyList<LibraryItem>> GetCachedAsync(
        string tenantId,
        string scope,
        string? searchTerm,
        Func<Task<IReadOnlyList<LibraryItemSnapshot>>> loader,
        CancellationToken ct)
    {
        var version = await GetCacheVersionAsync(tenantId, ct);
        var key = BuildCacheKey(tenantId, version, scope, searchTerm);
        var cached = await _cache.GetStringAsync(key, ct);

        if (!string.IsNullOrWhiteSpace(cached))
        {
            var items = JsonSerializer.Deserialize<List<LibraryItem>>(cached, CacheJsonOptions);
            if (items is not null)
            {
                return items;
            }
        }

        var rows = await loader();
        var result = rows.Select(Map).ToList();

        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(result, CacheJsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            ct);

        return result;
    }

    private async Task<string> GetCacheVersionAsync(string tenantId, CancellationToken ct)
    {
        var key = $"{CachePrefix}:{tenantId}:version";
        var version = await _cache.GetStringAsync(key, ct);
        if (!string.IsNullOrWhiteSpace(version))
        {
            return version;
        }

        version = Guid.NewGuid().ToString("N");
        await _cache.SetStringAsync(
            key,
            version,
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(6)
            },
            ct);
        return version;
    }

    private async Task BumpCacheVersionAsync(string tenantId, CancellationToken ct)
    {
        var key = $"{CachePrefix}:{tenantId}:version";
        await _cache.SetStringAsync(
            key,
            Guid.NewGuid().ToString("N"),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(6)
            },
            ct);
    }

    private static string BuildCacheKey(string tenantId, string version, string scope, string? searchTerm)
    {
        var searchHash = HashSearchTerm(searchTerm);
        return $"{CachePrefix}:{tenantId}:{version}:{scope}:{searchHash}";
    }

    private static string HashSearchTerm(string? searchTerm)
    {
        var normalized = (searchTerm ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalized))
        {
            return "all";
        }

        var bytes = Encoding.UTF8.GetBytes(normalized);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
