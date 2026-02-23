using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Cms.Application.Models;
using Mamey.Portal.Cms.Application.Services;
using Mamey.Portal.Cms.Infrastructure.Persistence;

namespace Mamey.Portal.Cms.Infrastructure.Services;

public sealed class CmsPageStore : ICmsPageStore
{
    private readonly CmsDbContext _db;

    public CmsPageStore(CmsDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CmsPageSnapshot>> GetPagesAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.Pages.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Slug)
            .Take(500)
            .ToListAsync(ct);

        return rows.Select(Map).ToList();
    }

    public async Task<CmsPageSnapshot?> GetPageAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        var row = await _db.Pages.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == id)
            .SingleOrDefaultAsync(ct);

        return row is null ? null : Map(row);
    }

    public async Task<CmsPageSnapshot?> GetPublishedPageBySlugAsync(string tenantId, string slug, CancellationToken ct = default)
    {
        var row = await _db.Pages.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Status == CmsContentStatus.Published && x.Slug == slug)
            .SingleOrDefaultAsync(ct);

        return row is null ? null : Map(row);
    }

    public async Task<bool> SlugExistsAsync(string tenantId, string slug, Guid? excludeId, CancellationToken ct = default)
    {
        var query = _db.Pages.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Slug == slug);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<CmsPageSnapshot> CreateDraftAsync(
        string tenantId,
        string? userName,
        string slug,
        string title,
        string bodyHtml,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = new CmsPageRow
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Slug = slug,
            Title = title,
            BodyHtml = bodyHtml,
            Status = CmsContentStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userName,
            UpdatedBy = userName,
        };

        _db.Pages.Add(row);
        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsPageSnapshot> UpdateDraftAsync(
        string tenantId,
        Guid id,
        string? userName,
        string slug,
        string title,
        string bodyHtml,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.Pages.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Page not found.");

        row.Slug = slug;
        row.Title = title;
        row.BodyHtml = bodyHtml;
        row.Status = CmsContentStatus.Draft;
        row.SubmittedAt = null;
        row.ApprovedAt = null;
        row.RejectedAt = null;
        row.RejectionReason = null;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsPageSnapshot> SubmitForReviewAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.Pages.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Page not found.");

        row.Status = CmsContentStatus.InReview;
        row.SubmittedAt = now;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsPageSnapshot> ApproveAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.Pages.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Page not found.");

        row.Status = CmsContentStatus.Approved;
        row.ApprovedAt = now;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsPageSnapshot> RejectAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        string reason,
        CancellationToken ct = default)
    {
        var row = await _db.Pages.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Page not found.");

        row.Status = CmsContentStatus.Rejected;
        row.RejectedAt = now;
        row.RejectionReason = reason;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsPageSnapshot> PublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.Pages.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Page not found.");

        row.Status = CmsContentStatus.Published;
        row.PublishedAt = now;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsPageSnapshot> UnpublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.Pages.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("Page not found.");

        row.Status = CmsContentStatus.Approved;
        row.PublishedAt = null;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    private static CmsPageSnapshot Map(CmsPageRow x) => new(
        x.Id,
        x.TenantId,
        x.Slug,
        x.Title,
        x.BodyHtml,
        x.Status,
        x.CreatedAt,
        x.UpdatedAt,
        x.CreatedBy,
        x.UpdatedBy,
        x.SubmittedAt,
        x.ApprovedAt,
        x.PublishedAt,
        x.RejectedAt,
        x.RejectionReason);
}

