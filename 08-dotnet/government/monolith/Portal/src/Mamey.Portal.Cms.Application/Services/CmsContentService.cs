using Mamey.Portal.Cms.Application.Models;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Cms.Application.Services;

public sealed class CmsContentService : ICmsContentService
{
    private readonly ICmsContentStore _store;
    private readonly ITenantContext _tenant;
    private readonly ICurrentUserContext _user;
    private readonly ICmsHtmlSanitizer _html;

    public CmsContentService(ICmsContentStore store, ITenantContext tenant, ICurrentUserContext user, ICmsHtmlSanitizer html)
    {
        _store = store;
        _tenant = tenant;
        _user = user;
        _html = html;
    }

    public async Task<IReadOnlyList<NewsItem>> GetNewsAsync(CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var role = _user.Role ?? string.Empty;

        // Editors/Admins see everything. Others see only published.
        var canSeeAll = string.Equals(role, "ContentEditor", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

        var rows = await _store.GetNewsAsync(tenantId, includeAll: canSeeAll, ct);

        if (canSeeAll)
        {
            return rows.Select(x => Map(x, sanitizeBody: false)).ToList();
        }

        // Public-safe view: sanitize body HTML.
        return rows.Select(x => Map(x, sanitizeBody: true)).ToList();
    }

    public async Task<IReadOnlyList<NewsItem>> GetPublishedNewsAsync(int take = 10, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 50);
        var tenantId = _tenant.TenantId;

        var rows = await _store.GetPublishedNewsAsync(tenantId, take, ct);
        return rows.Select(x => Map(x, sanitizeBody: true)).ToList();
    }

    public async Task<NewsItem?> GetPublishedNewsItemAsync(Guid id, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var row = await _store.GetPublishedNewsItemAsync(tenantId, id, ct);
        return row is null ? null : Map(row, sanitizeBody: true);
    }

    public async Task<NewsItem> CreateDraftAsync(string title, string summary, string bodyHtml, CancellationToken ct = default)
    {
        EnsureCanEdit();
        title = (title ?? string.Empty).Trim();
        summary = (summary ?? string.Empty).Trim();
        bodyHtml ??= string.Empty;
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(summary))
        {
            throw new ArgumentException("Title and summary are required.");
        }

        var now = DateTimeOffset.UtcNow;
        var row = await _store.CreateDraftAsync(
            _tenant.TenantId,
            _user.UserName,
            title,
            summary,
            string.IsNullOrWhiteSpace(bodyHtml) ? "<p></p>" : bodyHtml.Trim(),
            now,
            ct);

        return Map(row, sanitizeBody: false);
    }

    public async Task<NewsItem> UpdateDraftAsync(Guid id, string title, string summary, string bodyHtml, CancellationToken ct = default)
    {
        EnsureCanEdit();
        title = (title ?? string.Empty).Trim();
        summary = (summary ?? string.Empty).Trim();
        bodyHtml ??= string.Empty;
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(summary))
        {
            throw new ArgumentException("Title and summary are required.");
        }

        var tenantId = _tenant.TenantId;
        var existing = await _store.GetNewsItemAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("News item not found.");

        if (existing.Status != CmsContentStatus.Draft && existing.Status != CmsContentStatus.Rejected)
        {
            throw new InvalidOperationException("Only Draft/Rejected items can be edited.");
        }

        var row = await _store.UpdateDraftAsync(
            tenantId,
            id,
            _user.UserName,
            title,
            summary,
            string.IsNullOrWhiteSpace(bodyHtml) ? "<p></p>" : bodyHtml.Trim(),
            DateTimeOffset.UtcNow,
            ct);

        return Map(row, sanitizeBody: false);
    }

    public async Task<NewsItem> SubmitForReviewAsync(Guid id, CancellationToken ct = default)
    {
        EnsureCanEdit();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetNewsItemAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("News item not found.");

        if (existing.Status != CmsContentStatus.Draft && existing.Status != CmsContentStatus.Rejected)
        {
            throw new InvalidOperationException("Only Draft/Rejected items can be submitted for review.");
        }

        var row = await _store.SubmitForReviewAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        return Map(row, sanitizeBody: false);
    }

    public async Task<NewsItem> ApproveAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetNewsItemAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("News item not found.");

        if (existing.Status != CmsContentStatus.InReview)
        {
            throw new InvalidOperationException("Only InReview items can be approved.");
        }

        var row = await _store.ApproveAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        return Map(row, sanitizeBody: false);
    }

    public async Task<NewsItem> RejectAsync(Guid id, string reason, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetNewsItemAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("News item not found.");

        if (existing.Status != CmsContentStatus.InReview)
        {
            throw new InvalidOperationException("Only InReview items can be rejected.");
        }

        var row = await _store.RejectAsync(
            tenantId,
            id,
            _user.UserName,
            DateTimeOffset.UtcNow,
            string.IsNullOrWhiteSpace(reason) ? "Rejected" : reason.Trim(),
            ct);
        return Map(row, sanitizeBody: false);
    }

    public async Task<NewsItem> PublishAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetNewsItemAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("News item not found.");

        if (existing.Status != CmsContentStatus.Approved)
        {
            throw new InvalidOperationException("Only Approved items can be published.");
        }

        var row = await _store.PublishAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        return Map(row, sanitizeBody: false);
    }

    public async Task<NewsItem> UnpublishAsync(Guid id, CancellationToken ct = default)
    {
        EnsureIsAdmin();
        var tenantId = _tenant.TenantId;
        var existing = await _store.GetNewsItemAsync(tenantId, id, ct)
                      ?? throw new InvalidOperationException("News item not found.");

        if (existing.Status != CmsContentStatus.Published)
        {
            throw new InvalidOperationException("Only Published items can be unpublished.");
        }

        var row = await _store.UnpublishAsync(tenantId, id, _user.UserName, DateTimeOffset.UtcNow, ct);
        return Map(row, sanitizeBody: false);
    }

    private void EnsureCanEdit()
    {
        if (!_user.IsAuthenticated) throw new InvalidOperationException("Not authenticated.");
        if (!_user.IsInRole("ContentEditor") && !_user.IsInRole("Admin"))
        {
            throw new InvalidOperationException("Not authorized.");
        }
    }

    private void EnsureIsAdmin()
    {
        if (!_user.IsAuthenticated) throw new InvalidOperationException("Not authenticated.");
        if (!_user.IsInRole("Admin"))
        {
            throw new InvalidOperationException("Admin role required.");
        }
    }

    private NewsItem Map(CmsNewsItemSnapshot x, bool sanitizeBody) => new(
        x.Id,
        x.TenantId,
        x.Title,
        x.Summary,
        sanitizeBody ? _html.SanitizeNewsBody(x.BodyHtml) : x.BodyHtml,
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
