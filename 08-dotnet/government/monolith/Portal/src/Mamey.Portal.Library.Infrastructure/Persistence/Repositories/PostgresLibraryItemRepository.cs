using Microsoft.EntityFrameworkCore;
using AppModels = Mamey.Portal.Library.Application.Models;
using Mamey.Portal.Library.Domain.Entities;
using Mamey.Portal.Library.Domain.Repositories;
using DomainValueObjects = Mamey.Portal.Library.Domain.ValueObjects;

namespace Mamey.Portal.Library.Infrastructure.Persistence.Repositories;

public sealed class PostgresLibraryItemRepository : ILibraryItemRepository
{
    private readonly LibraryDbContext _db;

    public PostgresLibraryItemRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<LibraryItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.Items.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return row is null ? null : ToDomainEntity(row);
    }

    public async Task<IReadOnlyList<LibraryItem>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.Items.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

        return rows.Select(ToDomainEntity).ToList();
    }

    public async Task SaveAsync(LibraryItem item, CancellationToken ct = default)
    {
        var row = await _db.Items.SingleOrDefaultAsync(x => x.Id == item.Id, ct);

        if (row is null)
        {
            _db.Items.Add(ToRow(item));
        }
        else
        {
            UpdateRow(row, item);
        }

        await _db.SaveChangesAsync(ct);
    }

    private static LibraryItem ToDomainEntity(LibraryItemRow row)
    {
        return LibraryItem.Rehydrate(
            row.Id,
            row.TenantId,
            row.Category,
            row.Title,
            row.Summary,
            MapVisibility(row.Visibility),
            MapStatus(row.Status),
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
    }

    private static LibraryItemRow ToRow(LibraryItem item)
    {
        return new LibraryItemRow
        {
            Id = item.Id,
            TenantId = item.TenantId,
            Category = item.Category,
            Title = item.Title,
            Summary = item.Summary,
            Visibility = MapVisibility(item.Visibility),
            Status = MapStatus(item.Status),
            FileName = item.FileName,
            ContentType = item.ContentType,
            Size = item.Size,
            StorageBucket = item.StorageBucket,
            StorageKey = item.StorageKey,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            CreatedBy = item.CreatedBy,
            UpdatedBy = item.UpdatedBy,
            PublishedAt = item.PublishedAt
        };
    }

    private static void UpdateRow(LibraryItemRow row, LibraryItem item)
    {
        row.TenantId = item.TenantId;
        row.Category = item.Category;
        row.Title = item.Title;
        row.Summary = item.Summary;
        row.Visibility = MapVisibility(item.Visibility);
        row.Status = MapStatus(item.Status);
        row.FileName = item.FileName;
        row.ContentType = item.ContentType;
        row.Size = item.Size;
        row.StorageBucket = item.StorageBucket;
        row.StorageKey = item.StorageKey;
        row.CreatedAt = item.CreatedAt;
        row.UpdatedAt = item.UpdatedAt;
        row.CreatedBy = item.CreatedBy;
        row.UpdatedBy = item.UpdatedBy;
        row.PublishedAt = item.PublishedAt;
    }

    private static DomainValueObjects.LibraryVisibility MapVisibility(AppModels.LibraryVisibility visibility)
        => (DomainValueObjects.LibraryVisibility)(int)visibility;

    private static AppModels.LibraryVisibility MapVisibility(DomainValueObjects.LibraryVisibility visibility)
        => (AppModels.LibraryVisibility)(int)visibility;

    private static DomainValueObjects.LibraryContentStatus MapStatus(AppModels.LibraryContentStatus status)
    {
        return status switch
        {
            AppModels.LibraryContentStatus.Published => DomainValueObjects.LibraryContentStatus.Published,
            AppModels.LibraryContentStatus.Unpublished => DomainValueObjects.LibraryContentStatus.Archived,
            _ => DomainValueObjects.LibraryContentStatus.Draft
        };
    }

    private static AppModels.LibraryContentStatus MapStatus(DomainValueObjects.LibraryContentStatus status)
    {
        return status switch
        {
            DomainValueObjects.LibraryContentStatus.Published => AppModels.LibraryContentStatus.Published,
            DomainValueObjects.LibraryContentStatus.Archived => AppModels.LibraryContentStatus.Unpublished,
            DomainValueObjects.LibraryContentStatus.Review => AppModels.LibraryContentStatus.Draft,
            _ => AppModels.LibraryContentStatus.Draft
        };
    }
}
