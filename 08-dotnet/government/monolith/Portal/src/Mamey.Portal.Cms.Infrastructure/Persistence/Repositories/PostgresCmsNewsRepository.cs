using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Cms.Domain.Entities;
using Mamey.Portal.Cms.Domain.Repositories;
using Mamey.Portal.Cms.Domain.ValueObjects;

namespace Mamey.Portal.Cms.Infrastructure.Persistence.Repositories;

public sealed class PostgresCmsNewsRepository : ICmsNewsRepository
{
    private readonly CmsDbContext _db;

    public PostgresCmsNewsRepository(CmsDbContext db)
    {
        _db = db;
    }

    public async Task<CmsNewsItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.News.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return row is null ? null : ToDomainEntity(row);
    }

    public async Task<IReadOnlyList<CmsNewsItem>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.News.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return rows.Select(ToDomainEntity).ToList();
    }

    public async Task SaveAsync(CmsNewsItem item, CancellationToken ct = default)
    {
        var row = await _db.News.SingleOrDefaultAsync(x => x.Id == item.Id, ct);

        if (row is null)
        {
            _db.News.Add(ToRow(item));
        }
        else
        {
            UpdateRow(row, item);
        }

        await _db.SaveChangesAsync(ct);
    }

    private static CmsNewsItem ToDomainEntity(CmsNewsItemRow row)
    {
        return CmsNewsItem.Rehydrate(
            row.Id,
            row.TenantId,
            row.Title,
            row.Summary,
            row.BodyHtml,
            ParseStatus(row.Status),
            row.CreatedAt,
            row.UpdatedAt,
            row.CreatedBy,
            row.UpdatedBy,
            row.SubmittedAt,
            row.ApprovedAt,
            row.PublishedAt,
            row.RejectedAt,
            row.RejectionReason);
    }

    private static CmsNewsItemRow ToRow(CmsNewsItem item)
    {
        return new CmsNewsItemRow
        {
            Id = item.Id,
            TenantId = item.TenantId,
            Title = item.Title,
            Summary = item.Summary,
            BodyHtml = item.BodyHtml,
            Status = MapStatus(item.Status),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            CreatedBy = item.CreatedBy,
            UpdatedBy = item.UpdatedBy,
            SubmittedAt = item.SubmittedAt,
            ApprovedAt = item.ApprovedAt,
            PublishedAt = item.PublishedAt,
            RejectedAt = item.RejectedAt,
            RejectionReason = item.RejectionReason
        };
    }

    private static void UpdateRow(CmsNewsItemRow row, CmsNewsItem item)
    {
        row.TenantId = item.TenantId;
        row.Title = item.Title;
        row.Summary = item.Summary;
        row.BodyHtml = item.BodyHtml;
        row.Status = MapStatus(item.Status);
        row.CreatedAt = item.CreatedAt;
        row.UpdatedAt = item.UpdatedAt;
        row.CreatedBy = item.CreatedBy;
        row.UpdatedBy = item.UpdatedBy;
        row.SubmittedAt = item.SubmittedAt;
        row.ApprovedAt = item.ApprovedAt;
        row.PublishedAt = item.PublishedAt;
        row.RejectedAt = item.RejectedAt;
        row.RejectionReason = item.RejectionReason;
    }

    private static CmsContentStatus ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return CmsContentStatus.Draft;
        }

        if (string.Equals(status, "InReview", StringComparison.OrdinalIgnoreCase))
        {
            return CmsContentStatus.Review;
        }

        return Enum.TryParse(status, true, out CmsContentStatus parsed)
            ? parsed
            : CmsContentStatus.Draft;
    }

    private static string MapStatus(CmsContentStatus status)
    {
        return status == CmsContentStatus.Review
            ? "InReview"
            : status.ToString();
    }
}
