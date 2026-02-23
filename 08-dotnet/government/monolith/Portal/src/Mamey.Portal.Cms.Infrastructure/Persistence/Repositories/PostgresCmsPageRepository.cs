using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Cms.Domain.Entities;
using Mamey.Portal.Cms.Domain.Repositories;
using Mamey.Portal.Cms.Domain.ValueObjects;

namespace Mamey.Portal.Cms.Infrastructure.Persistence.Repositories;

public sealed class PostgresCmsPageRepository : ICmsPageRepository
{
    private readonly CmsDbContext _db;

    public PostgresCmsPageRepository(CmsDbContext db)
    {
        _db = db;
    }

    public async Task<CmsPage?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.Pages.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return row is null ? null : ToDomainEntity(row);
    }

    public async Task<CmsPage?> GetBySlugAsync(string tenantId, string slug, CancellationToken ct = default)
    {
        var row = await _db.Pages.AsNoTracking()
            .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Slug == slug, ct);

        return row is null ? null : ToDomainEntity(row);
    }

    public async Task<IReadOnlyList<CmsPage>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.Pages.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Slug)
            .ToListAsync(ct);

        return rows.Select(ToDomainEntity).ToList();
    }

    public async Task SaveAsync(CmsPage page, CancellationToken ct = default)
    {
        var row = await _db.Pages.SingleOrDefaultAsync(x => x.Id == page.Id, ct);

        if (row is null)
        {
            _db.Pages.Add(ToRow(page));
        }
        else
        {
            UpdateRow(row, page);
        }

        await _db.SaveChangesAsync(ct);
    }

    private static CmsPage ToDomainEntity(CmsPageRow row)
    {
        return CmsPage.Rehydrate(
            row.Id,
            row.TenantId,
            new CmsSlug(row.Slug),
            row.Title,
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

    private static CmsPageRow ToRow(CmsPage page)
    {
        return new CmsPageRow
        {
            Id = page.Id,
            TenantId = page.TenantId,
            Slug = page.Slug.Value,
            Title = page.Title,
            BodyHtml = page.BodyHtml,
            Status = MapStatus(page.Status),
            CreatedAt = page.CreatedAt,
            UpdatedAt = page.UpdatedAt,
            CreatedBy = page.CreatedBy,
            UpdatedBy = page.UpdatedBy,
            SubmittedAt = page.SubmittedAt,
            ApprovedAt = page.ApprovedAt,
            PublishedAt = page.PublishedAt,
            RejectedAt = page.RejectedAt,
            RejectionReason = page.RejectionReason
        };
    }

    private static void UpdateRow(CmsPageRow row, CmsPage page)
    {
        row.TenantId = page.TenantId;
        row.Slug = page.Slug.Value;
        row.Title = page.Title;
        row.BodyHtml = page.BodyHtml;
        row.Status = MapStatus(page.Status);
        row.CreatedAt = page.CreatedAt;
        row.UpdatedAt = page.UpdatedAt;
        row.CreatedBy = page.CreatedBy;
        row.UpdatedBy = page.UpdatedBy;
        row.SubmittedAt = page.SubmittedAt;
        row.ApprovedAt = page.ApprovedAt;
        row.PublishedAt = page.PublishedAt;
        row.RejectedAt = page.RejectedAt;
        row.RejectionReason = page.RejectionReason;
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
