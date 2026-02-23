using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Cms.Application.Models;
using Mamey.Portal.Cms.Application.Services;
using Mamey.Portal.Cms.Infrastructure.Persistence;

namespace Mamey.Portal.Cms.Infrastructure.Services;

public sealed class CmsContentStore : ICmsContentStore
{
    private readonly CmsDbContext _db;

    public CmsContentStore(CmsDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CmsNewsItemSnapshot>> GetNewsAsync(string tenantId, bool includeAll, CancellationToken ct = default)
    {
        var query = _db.News.AsNoTracking().Where(x => x.TenantId == tenantId);
        if (!includeAll)
        {
            query = query.Where(x => x.Status == CmsContentStatus.Published);
        }

        var rows = await query
            .OrderByDescending(x => x.CreatedAt)
            .Take(200)
            .ToListAsync(ct);

        return rows.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<CmsNewsItemSnapshot>> GetPublishedNewsAsync(string tenantId, int take, CancellationToken ct = default)
    {
        var rows = await _db.News.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Status == CmsContentStatus.Published)
            .OrderByDescending(x => x.PublishedAt ?? x.UpdatedAt)
            .Take(take)
            .ToListAsync(ct);

        return rows.Select(Map).ToList();
    }

    public async Task<CmsNewsItemSnapshot?> GetNewsItemAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        var row = await _db.News.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == id)
            .SingleOrDefaultAsync(ct);

        return row is null ? null : Map(row);
    }

    public async Task<CmsNewsItemSnapshot?> GetPublishedNewsItemAsync(string tenantId, Guid id, CancellationToken ct = default)
    {
        var row = await _db.News.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Status == CmsContentStatus.Published && x.Id == id)
            .SingleOrDefaultAsync(ct);

        return row is null ? null : Map(row);
    }

    public async Task<CmsNewsItemSnapshot> CreateDraftAsync(
        string tenantId,
        string? userName,
        string title,
        string summary,
        string bodyHtml,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = new CmsNewsItemRow
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = title,
            Summary = summary,
            BodyHtml = bodyHtml,
            Status = CmsContentStatus.Draft,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = userName,
            UpdatedBy = userName,
        };

        _db.News.Add(row);
        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsNewsItemSnapshot> UpdateDraftAsync(
        string tenantId,
        Guid id,
        string? userName,
        string title,
        string summary,
        string bodyHtml,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.News.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("News item not found.");

        row.Title = title;
        row.Summary = summary;
        row.BodyHtml = bodyHtml;
        row.Status = CmsContentStatus.Draft;
        row.RejectedAt = null;
        row.RejectionReason = null;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsNewsItemSnapshot> SubmitForReviewAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.News.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("News item not found.");

        row.Status = CmsContentStatus.InReview;
        row.SubmittedAt = now;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsNewsItemSnapshot> ApproveAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.News.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("News item not found.");

        row.Status = CmsContentStatus.Approved;
        row.ApprovedAt = now;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsNewsItemSnapshot> RejectAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        string reason,
        CancellationToken ct = default)
    {
        var row = await _db.News.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("News item not found.");

        row.Status = CmsContentStatus.Rejected;
        row.RejectedAt = now;
        row.RejectionReason = reason;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsNewsItemSnapshot> PublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.News.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("News item not found.");

        row.Status = CmsContentStatus.Published;
        row.PublishedAt = now;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task<CmsNewsItemSnapshot> UnpublishAsync(
        string tenantId,
        Guid id,
        string? userName,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.News.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == id, ct)
                  ?? throw new InvalidOperationException("News item not found.");

        row.Status = CmsContentStatus.Approved;
        row.PublishedAt = null;
        row.UpdatedAt = now;
        row.UpdatedBy = userName;

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    private static CmsNewsItemSnapshot Map(CmsNewsItemRow x) => new(
        x.Id,
        x.TenantId,
        x.Title,
        x.Summary,
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

