using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Library.Application.Models;
using Mamey.Portal.Library.Application.Services;
using Mamey.Portal.Library.Infrastructure.Persistence;

namespace Mamey.Portal.Library.Infrastructure.Services;

public sealed class LibraryStore : ILibraryStore
{
    private readonly LibraryDbContext _db;

    public LibraryStore(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<LibraryItemSnapshot>> GetPublishedPublicAsync(
        string tenantId,
        int take,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        var query = _db.Items.AsNoTracking()
            .Where(x => x.TenantId == tenantId
                        && x.Status == LibraryContentStatus.Published
                        && x.Visibility == LibraryVisibility.Public);
        query = ApplySearch(query, searchTerm);

        var rows = await query.OrderByDescending(x => x.PublishedAt ?? x.CreatedAt)
            .Take(take)
            .ToListAsync(ct);

        return rows.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<LibraryItemSnapshot>> GetPublishedForVisibilityAsync(
        string tenantId,
        LibraryVisibility maxVisibility,
        int take,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        var query = _db.Items.AsNoTracking()
            .Where(x => x.TenantId == tenantId
                        && x.Status == LibraryContentStatus.Published
                        && x.Visibility <= maxVisibility);
        query = ApplySearch(query, searchTerm);

        var rows = await query.OrderByDescending(x => x.PublishedAt ?? x.CreatedAt)
            .Take(take)
            .ToListAsync(ct);

        return rows.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<LibraryItemSnapshot>> GetAllAsync(
        string tenantId,
        int take,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        var query = _db.Items.AsNoTracking()
            .Where(x => x.TenantId == tenantId);
        query = ApplySearch(query, searchTerm);

        var rows = await query.OrderByDescending(x => x.UpdatedAt)
            .Take(take)
            .ToListAsync(ct);

        return rows.Select(Map).ToList();
    }

    public async Task<LibraryItemSnapshot?> GetAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        var row = await _db.Items.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == id)
            .SingleOrDefaultAsync(ct);

        return row is null ? null : Map(row);
    }

    public async Task<LibraryItemSnapshot> CreateDraftAsync(
        string tenantId,
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        string fileName,
        string contentType,
        long size,
        string storageBucket,
        string storageKey,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = new LibraryItemRow
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Category = category,
            Title = title,
            Summary = summary,
            Visibility = visibility,
            Status = LibraryContentStatus.Draft,
            FileName = fileName,
            ContentType = contentType,
            Size = size,
            StorageBucket = storageBucket,
            StorageKey = storageKey,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userName,
            UpdatedBy = userName,
        };

        _db.Items.Add(row);
        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<LibraryItemSnapshot> UpdateDraftAsync(
        string tenantId,
        Guid id,
        string category,
        string title,
        string? summary,
        LibraryVisibility visibility,
        string? fileName,
        string? contentType,
        long? size,
        string? storageBucket,
        string? storageKey,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.Items.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Library item not found.");

        if (!string.IsNullOrWhiteSpace(fileName)
            && !string.IsNullOrWhiteSpace(contentType)
            && size.HasValue
            && !string.IsNullOrWhiteSpace(storageBucket)
            && !string.IsNullOrWhiteSpace(storageKey))
        {
            row.StorageBucket = storageBucket;
            row.StorageKey = storageKey;
            row.FileName = fileName;
            row.ContentType = contentType;
            row.Size = size.Value;
        }

        row.Category = category;
        row.Title = title;
        row.Summary = summary;
        row.Visibility = visibility;
        row.Status = LibraryContentStatus.Draft;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;
        row.PublishedAt = null;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<LibraryItemSnapshot> PublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.Items.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Library item not found.");

        row.Status = LibraryContentStatus.Published;
        row.PublishedAt = now;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<LibraryItemSnapshot> UnpublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.Items.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Library item not found.");

        row.Status = LibraryContentStatus.Unpublished;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    private static LibraryItemSnapshot Map(LibraryItemRow row)
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

    private static IQueryable<LibraryItemRow> ApplySearch(IQueryable<LibraryItemRow> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        var term = $"%{searchTerm.Trim()}%";
        return query.Where(x =>
            EF.Functions.ILike(x.Title, term)
            || EF.Functions.ILike(x.Category, term)
            || (x.Summary != null && EF.Functions.ILike(x.Summary, term)));
    }
}
